namespace WibboEmulator.Communication.Packets.Incoming.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Achievements;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Bots;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Pets;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Notifications;
using WibboEmulator.Database.Daos.Catalog;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Database.Daos.Log;
using WibboEmulator.Database.Daos.User;
using System.Data;
using WibboEmulator.Games.Catalogs.Utilities;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Users.Premium;
using WibboEmulator.Database;
using WibboEmulator.Core.Language;
using WibboEmulator.Games.Catalogs;

internal sealed class PurchaseFromCatalogEvent : IPacketEvent
{
    public double Delay => 500;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var pageId = packet.PopInt();
        var itemId = packet.PopInt();
        var extraData = packet.PopString(512);
        var amount = packet.PopInt();

        if (!CatalogManager.TryGetPage(pageId, out var page))
        {
            return;
        }

        if (!page.Enabled || !page.HavePermission(Session.User))
        {
            return;
        }

        if (!page.Items.TryGetValue(itemId, out var item))
        {
            if (page.ItemOffers.TryGetValue(itemId, out var value))
            {
                item = value;
                if (item == null)
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }

        if (page.IsPremium && Session.User.Rank < 2)
        {
            Session.SendNotification("Vous devez être membre du premium club pour pouvoir acheter ce mobilier");
            Session.SendPacket(new PurchaseErrorComposer());
            return;
        }

        if (amount < 1 || amount > 100 || !ItemUtility.CanSelectAmount(item))
        {
            amount = 1;
        }

        if (item.IsLimited)
        {
            var leftTimeLTD = DateTime.Now - Session.User.LastLTDPurchaseTime;
            if (leftTimeLTD.TotalSeconds <= 10)
            {
                Session.SendNotification($"Vous devez attendre encore {10 - (int)leftTimeLTD.TotalSeconds} secondes avant de pouvoir racheter un LTD");
                Session.SendPacket(new PurchaseErrorComposer());
                return;
            }

            Session.User.LastLTDPurchaseTime = DateTime.Now;
        }

        var amountPurchase = item.Amount > 1 ? item.Amount : amount;

        var totalCreditsCost = amount > 1 ? item.CostCredits * amount : item.CostCredits;
        var totalPixelCost = amount > 1 ? item.CostDuckets * amount : item.CostDuckets;
        var totalWibboPointCost = amount > 1 ? item.CostWibboPoints * amount : item.CostWibboPoints;
        var totalLimitCoinCost = amount > 1 ? item.CostLimitCoins * amount : item.CostLimitCoins;

        if (Session.User.Credits < totalCreditsCost ||
            Session.User.Duckets < totalPixelCost ||
            Session.User.WibboPoints < totalWibboPointCost ||
            Session.User.LimitCoins < totalLimitCoinCost)
        {
            Session.SendPacket(new PurchaseErrorComposer());
            return;
        }

        if (!ItemUtility.TryProcessExtraData(item, Session, ref extraData))
        {
            Session.SendPacket(new PurchaseErrorComposer());
            return;
        }

        if (Session.User.InventoryComponent.IsOverlowLimit(amountPurchase, item.Data.Type))
        {
            Session.SendNotification(LanguageManager.TryGetValue("catalog.purchase.limit", Session.Language));
            Session.SendPacket(new PurchaseErrorComposer());
            return;
        }

        using var dbClient = DatabaseManager.Connection;

        var limitedEditionSells = 0;
        var limitedEditionStack = 0;

        if (item.IsLimited)
        {
            if (item.LimitedEditionStack <= item.LimitedEditionSells)
            {
                Session.SendNotification(LanguageManager.TryGetValue("notif.buyltd.error", Session.Language));
                Session.SendPacket(new PurchaseErrorComposer());
                return;
            }

            item.LimitedEditionSells++;

            CatalogItemLimitedDao.Update(dbClient, item.Id, item.LimitedEditionSells);

            limitedEditionSells = item.LimitedEditionSells;
            limitedEditionStack = item.LimitedEditionStack;
        }

        if (item.CostCredits > 0)
        {
            Session.User.Credits -= totalCreditsCost;
            Session.SendPacket(new CreditBalanceComposer(Session.User.Credits));
        }

        if (item.CostDuckets > 0)
        {
            Session.User.Duckets -= totalPixelCost;
            Session.SendPacket(new ActivityPointNotificationComposer(Session.User.Duckets, Session.User.Duckets));
        }

        if (item.CostWibboPoints > 0)
        {
            Session.User.WibboPoints -= totalWibboPointCost;
            Session.SendPacket(new ActivityPointNotificationComposer(Session.User.WibboPoints, 0, 105));

            UserDao.UpdateRemovePoints(dbClient, Session.User.Id, totalWibboPointCost);
        }

        if (item.CostLimitCoins > 0)
        {
            Session.User.LimitCoins -= totalLimitCoinCost;
            Session.SendPacket(new ActivityPointNotificationComposer(Session.User.LimitCoins, 0, 55));

            LimitCoinsPrime(dbClient, Session, totalLimitCoinCost);

            UserDao.UpdateRemoveLimitCoins(dbClient, Session.User.Id, totalLimitCoinCost);
            LogShopDao.Insert(dbClient, Session.User.Id, totalLimitCoinCost, $"Achat de {item.Name} (x{item.Amount})", item.Id);
        }

        switch (item.Data.Type)
        {
            default:
                var generatedGenericItems = new List<Item>();

                Item newItem;
                switch (item.Data.InteractionType)
                {
                    default:
                        if (amountPurchase > 1)
                        {
                            var items = ItemFactory.CreateMultipleItems(dbClient, item.Data, Session.User, extraData, amountPurchase);

                            if (items != null)
                            {
                                generatedGenericItems.AddRange(items);
                            }
                        }
                        else
                        {
                            newItem = ItemFactory.CreateSingleItemNullable(dbClient, item.Data, Session.User, extraData, limitedEditionSells, limitedEditionStack);

                            if (newItem != null)
                            {
                                generatedGenericItems.Add(newItem);
                            }
                        }
                        break;

                    case InteractionType.TELEPORT:
                    case InteractionType.TELEPORT_ARROW:
                        for (var i = 0; i < amountPurchase; i++)
                        {
                            var teleItems = ItemFactory.CreateTeleporterItems(dbClient, item.Data, Session.User);

                            if (teleItems != null)
                            {
                                generatedGenericItems.AddRange(teleItems);
                            }
                        }
                        break;

                    case InteractionType.MOODLIGHT:
                    {
                        if (amountPurchase > 1)
                        {
                            var items = ItemFactory.CreateMultipleItems(dbClient, item.Data, Session.User, extraData, amountPurchase);

                            if (items != null)
                            {
                                generatedGenericItems.AddRange(items);
                                foreach (var itemMoodlight in items)
                                {
                                    ItemMoodlightDao.Insert(dbClient, itemMoodlight.Id);
                                }
                            }
                        }
                        else
                        {
                            newItem = ItemFactory.CreateSingleItemNullable(dbClient, item.Data, Session.User, extraData);

                            if (newItem != null)
                            {
                                generatedGenericItems.Add(newItem);
                                ItemMoodlightDao.Insert(dbClient, newItem.Id);
                            }
                        }
                    }
                    break;
                }

                foreach (var purchasedItem in generatedGenericItems)
                {
                    Session.User.InventoryComponent.TryAddItem(purchasedItem);
                }

                if (item.Data.Amount >= 0)
                {
                    item.Data.Amount += generatedGenericItems.Count;
                    ItemStatDao.UpdateAdd(dbClient, item.Data.Id, generatedGenericItems.Count);
                }
                break;

            case ItemType.R:
                var bot = BotUtility.CreateBot(dbClient, item.Data, Session.User.Id);
                if (bot != null)
                {
                    if (!Session.User.InventoryComponent.TryAddBot(bot))
                    {
                        break;
                    }

                    Session.SendPacket(new UnseenItemsComposer(bot.Id, UnseenItemsType.Bot));
                    Session.SendPacket(new BotInventoryComposer(Session.User.InventoryComponent.Bots));
                }
                break;

            case ItemType.P:
            {
                var bits = extraData.Split('\n');

                var petName = bits[0];
                var race = bits[1];
                var color = bits[2];

                var generatedPet = PetUtility.CreatePet(Session.User.Id, petName, item.Data.SpriteId, race, color);
                if (generatedPet != null)
                {
                    if (!Session.User.InventoryComponent.TryAddPet(generatedPet))
                    {
                        break;
                    }

                    Session.SendPacket(new UnseenItemsComposer(generatedPet.PetId, UnseenItemsType.Pet));
                    Session.SendPacket(new PetInventoryComposer(Session.User.InventoryComponent.Pets));
                }
                break;
            }

            case ItemType.B:
            {
                break;
            }

            case ItemType.C:
            {
                if (item.Name == "premium_club_3") //Legend
                {
                    Session.User.Premium.AddPremiumDays(dbClient, 31, PremiumClubLevel.LEGEND);
                }
                else if (item.Name == "premium_club_2") //Epic
                {
                    Session.User.Premium.AddPremiumDays(dbClient, 31, PremiumClubLevel.EPIC);
                }
                else if (item.Name == "premium_club_1") //Classic
                {
                    Session.User.Premium.AddPremiumDays(dbClient, 31, PremiumClubLevel.CLASSIC);
                }
                else
                {
                    break;
                }

                Session.User.Premium.SendPackets();
                break;
            }
        }

        if (!string.IsNullOrEmpty(item.Badge) && !Session.User.BadgeComponent.HasBadge(item.Badge))
        {
            Session.User.BadgeComponent.GiveBadge(item.Badge);
        }

        Session.SendPacket(new PurchaseOKComposer(item, item.Data));
    }

    private static void LimitCoinsPrime(IDbConnection dbClient, GameClient Session, int totalLimitCoinCost)
    {
        var notifImage = "";
        var wibboPointCount = 0;
        var winwinCount = totalLimitCoinCost * 10;

        if (Session.User.HasPermission("premium_legend"))
        {
            notifImage = "premium_legend";
            wibboPointCount = totalLimitCoinCost * 3;
            winwinCount += (int)Math.Floor(winwinCount * 1.5);
        }
        else if (Session.User.HasPermission("premium_epic"))
        {
            notifImage = "premium_epic";
            wibboPointCount = totalLimitCoinCost * 2;
            winwinCount += winwinCount;
        }
        else if (Session.User.HasPermission("premium_classic"))
        {
            notifImage = "premium_classic";
            wibboPointCount = totalLimitCoinCost;
            winwinCount += (int)Math.Floor(winwinCount * 0.5);
        }

        if (wibboPointCount > 0)
        {
            Session.User.WibboPoints += wibboPointCount;
            Session.SendPacket(new ActivityPointNotificationComposer(Session.User.WibboPoints, 0, 105));

            UserDao.UpdateAddPoints(dbClient, Session.User.Id, wibboPointCount);
        }

        if (winwinCount > 0)
        {
            UserStatsDao.UpdateAchievementScore(dbClient, Session.User.Id, winwinCount);

            Session.User.AchievementPoints += winwinCount;
            Session.SendPacket(new AchievementScoreComposer(Session.User.AchievementPoints));
        }

        if (winwinCount > 0 && wibboPointCount > 0)
        {
            Session.SendPacket(RoomNotificationComposer.SendBubble(notifImage, $"Vous avez reçu {wibboPointCount} WibboPoints ainsi que {winwinCount} Win-wins!"));
        }
        else
        {
            Session.SendPacket(RoomNotificationComposer.SendBubble(notifImage, $"Vous avez reçu {winwinCount} Win-wins!"));
        }
    }
}
