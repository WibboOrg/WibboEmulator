namespace WibboEmulator.Communication.Packets.Incoming.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Badges;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Bots;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Pets;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Database.Daos.Catalog;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.Catalog.Utilities;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;
using WibboEmulator.Games.Items;

internal class PurchaseFromCatalogEvent : IPacketEvent
{
    public double Delay => 1000;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var pageId = packet.PopInt();
        var itemId = packet.PopInt();
        var extraData = packet.PopString();
        var amount = packet.PopInt();

        if (!WibboEnvironment.GetGame().GetCatalog().TryGetPage(pageId, out var page))
        {
            return;
        }

        if (!page.Enabled || page.MinimumRank > session.User.Rank)
        {
            return;
        }

        if (!page.Items.TryGetValue(itemId, out var item))
        {
            if (page.ItemOffers.ContainsKey(itemId))
            {
                item = page.ItemOffers[itemId];
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

        if (amount < 1 || amount > 100 || !ItemUtility.CanSelectAmount(item))
        {
            amount = 1;
        }

        var amountPurchase = item.Amount > 1 ? item.Amount : amount;

        var totalCreditsCost = amount > 1 ? (item.CostCredits * amount) - ((int)Math.Floor((double)amount / 6) * item.CostCredits) : item.CostCredits;
        var totalPixelCost = amount > 1 ? (item.CostDuckets * amount) - ((int)Math.Floor((double)amount / 6) * item.CostDuckets) : item.CostDuckets;
        var totalDiamondCost = amount > 1 ? (item.CostWibboPoints * amount) - ((int)Math.Floor((double)amount / 6) * item.CostWibboPoints) : item.CostWibboPoints;
        var totalLimitCoinCost = amount > 1 ? (item.CostLimitCoins * amount) - ((int)Math.Floor((double)amount / 6) * item.CostLimitCoins) : item.CostLimitCoins;

        if (session.User.Credits < totalCreditsCost ||
            session.User.Duckets < totalPixelCost ||
            session.User.WibboPoints < totalDiamondCost ||
            session.User.LimitCoins < totalLimitCoinCost)
        {
            return;
        }

        var limitedEditionSells = 0;
        var limitedEditionStack = 0;

        switch (item.Data.InteractionType)
        {
            case InteractionType.NONE:
                extraData = "";
                break;

            case InteractionType.GUILD_ITEM:
            case InteractionType.GUILD_GATE:
                int groupId;
                if (!int.TryParse(extraData, out groupId))
                {
                    return;
                }

                if (groupId == 0)
                {
                    return;
                }

                Group group;
                if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(groupId, out group))
                {
                    return;
                }

                extraData = "0;" + group.Id;
                break;

            case InteractionType.PET:
                var bits = extraData.Split('\n');
                var petName = bits[0];
                var race = bits[1];
                var color = bits[2];

                if (!int.TryParse(race, out _))
                {
                    return;
                }

                if (!PetUtility.CheckPetName(petName))
                {
                    return;
                }

                if (race.Length > 2)
                {
                    return;
                }

                if (color.Length != 6)
                {
                    return;
                }

                _ = WibboEnvironment.GetGame().GetAchievementManager().ProgressAchievement(session, "ACH_PetLover", 1);

                break;

            case InteractionType.FLOOR:
            case InteractionType.WALLPAPER:
            case InteractionType.LANDSCAPE:

                double number;
                if (string.IsNullOrEmpty(extraData))
                {
                    number = 0;
                }
                else
                {
                    _ = double.TryParse(extraData, out number);
                }

                extraData = number.ToString().Replace(',', '.');
                break; // maintain extra data // todo: validate

            case InteractionType.POSTIT:
                extraData = "FFFF33";
                break;

            case InteractionType.MOODLIGHT:
                extraData = "1,1,1,#000000,255";
                break;

            case InteractionType.TROPHY:
                extraData = session.User.Username + Convert.ToChar(9) + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + Convert.ToChar(9) + extraData;
                break;

            case InteractionType.MANNEQUIN:
                extraData = "m;ch-210-1321.lg-285-92;Mannequin";
                break;

            case InteractionType.BADGE_TROC:
            {
                if (WibboEnvironment.GetGame().GetBadgeManager().HaveNotAllowed(extraData) || !WibboEnvironment.GetGame().GetCatalog().HasBadge(extraData))
                {
                    session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.buybadgedisplay.error", session.Langue));
                    session.SendPacket(new PurchaseOKComposer());
                    return;
                }

                if (!extraData.StartsWith("perso_"))
                {
                    session.User.BadgeComponent.RemoveBadge(extraData);
                }

                session.SendPacket(new BadgesComposer(session.User.BadgeComponent.BadgeList));

                break;
            }

            case InteractionType.BADGE_DISPLAY:
                if (WibboEnvironment.GetGame().GetBadgeManager().HaveNotAllowed(extraData) || !session.User.BadgeComponent.HasBadge(extraData))
                {
                    session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.buybadgedisplay.error", session.Langue));
                    session.SendPacket(new PurchaseOKComposer());
                    return;
                }

                extraData = extraData + Convert.ToChar(9) + session.User.Username + Convert.ToChar(9) + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
                break;

            case InteractionType.BADGE:
            {
                if (session.User.BadgeComponent.HasBadge(item.Badge))
                {
                    session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.buybadge.error", session.Langue));
                    session.SendPacket(new PurchaseOKComposer());
                    return;
                }
                break;
            }
            default:
                extraData = "";
                break;
        }


        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();

        if (item.IsLimited)
        {
            if (item.LimitedEditionStack <= item.LimitedEditionSells)
            {
                session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.buyltd.error", session.Langue));
                session.SendPacket(new PurchaseOKComposer());
                return;
            }

            item.LimitedEditionSells++;

            CatalogItemLimitedDao.Update(dbClient, item.Id, item.LimitedEditionSells);

            limitedEditionSells = item.LimitedEditionSells;
            limitedEditionStack = item.LimitedEditionStack;
        }

        if (item.CostCredits > 0)
        {
            session.User.Credits -= totalCreditsCost;
            session.SendPacket(new CreditBalanceComposer(session.User.Credits));
        }

        if (item.CostDuckets > 0)
        {
            session.User.Duckets -= totalPixelCost;
            session.SendPacket(new ActivityPointNotificationComposer(session.User.Duckets, session.User.Duckets));
        }

        if (item.CostWibboPoints > 0)
        {
            session.User.WibboPoints -= totalDiamondCost;
            session.SendPacket(new ActivityPointNotificationComposer(session.User.WibboPoints, 0, 105));

            UserDao.UpdateRemovePoints(dbClient, session.User.Id, totalDiamondCost);
        }

        if (item.CostLimitCoins > 0)
        {
            session.User.LimitCoins -= totalLimitCoinCost;
            session.SendPacket(new ActivityPointNotificationComposer(session.User.LimitCoins, 0, 55));

            UserDao.UpdateRemoveLimitCoins(dbClient, session.User.Id, totalLimitCoinCost);
        }

        switch (item.Data.Type.ToString().ToLower())
        {
            default:
                var generatedGenericItems = new List<Item>();


                Item newItem;
                switch (item.Data.InteractionType)
                {
                    default:
                        if (amountPurchase > 1)
                        {
                            var items = ItemFactory.CreateMultipleItems(item.Data, session.User, extraData, amountPurchase);

                            if (items != null)
                            {
                                generatedGenericItems.AddRange(items);
                            }
                        }
                        else
                        {
                            newItem = ItemFactory.CreateSingleItemNullable(item.Data, session.User, extraData, limitedEditionSells, limitedEditionStack);

                            if (newItem != null)
                            {
                                generatedGenericItems.Add(newItem);
                            }
                        }
                        break;

                    case InteractionType.TELEPORT:
                    case InteractionType.ARROW:
                        for (var i = 0; i < amountPurchase; i++)
                        {
                            var teleItems = ItemFactory.CreateTeleporterItems(item.Data, session.User);

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
                            var items = ItemFactory.CreateMultipleItems(item.Data, session.User, extraData, amountPurchase);

                            if (items != null)
                            {
                                generatedGenericItems.AddRange(items);
                                foreach (var itemMoodlight in items)
                                {
                                    ItemFactory.CreateMoodlightData(itemMoodlight);
                                }
                            }
                        }
                        else
                        {
                            newItem = ItemFactory.CreateSingleItemNullable(item.Data, session.User, extraData);

                            if (newItem != null)
                            {
                                generatedGenericItems.Add(newItem);
                                ItemFactory.CreateMoodlightData(newItem);
                            }
                        }
                    }
                    break;
                }

                foreach (var purchasedItem in generatedGenericItems)
                {
                    if (session.User.InventoryComponent.TryAddItem(purchasedItem))
                    {
                        session.SendPacket(new FurniListNotificationComposer(purchasedItem.Id, 1));
                    }
                }

                if (item.Data.Amount >= 0)
                {
                    item.Data.Amount += generatedGenericItems.Count;
                    ItemStatDao.UpdateAdd(dbClient, item.Data.Id, generatedGenericItems.Count);
                }
                break;

            case "r":
                var bot = BotUtility.CreateBot(item.Data, session.User.Id);
                if (bot != null)
                {
                    if (!session.User.InventoryComponent.TryAddBot(bot))
                    {
                        break;
                    }

                    session.SendPacket(new BotInventoryComposer(session.User.InventoryComponent.GetBots()));
                    session.SendPacket(new FurniListNotificationComposer(bot.Id, 5));
                }
                break;

            case "p":
            {
                var petData = extraData.Split('\n');

                var generatedPet = PetUtility.CreatePet(session.User.Id, petData[0], item.Data.SpriteId, petData[1], petData[2]);
                if (generatedPet != null)
                {
                    if (!session.User.InventoryComponent.TryAddPet(generatedPet))
                    {
                        break;
                    }

                    session.SendPacket(new FurniListNotificationComposer(generatedPet.PetId, 3));
                    session.SendPacket(new PetInventoryComposer(session.User.InventoryComponent.GetPets()));
                }
                break;
            }

            case "b":
            {
                break;
            }
        }

        if (!string.IsNullOrEmpty(item.Badge) && !session.User.BadgeComponent.HasBadge(item.Badge))
        {
            session.User.BadgeComponent.GiveBadge(item.Badge, true);
            session.SendPacket(new ReceiveBadgeComposer(item.Badge));

            session.SendPacket(new FurniListNotificationComposer(0, 4));
        }

        session.SendPacket(new PurchaseOKComposer(item, item.Data));
    }
}
