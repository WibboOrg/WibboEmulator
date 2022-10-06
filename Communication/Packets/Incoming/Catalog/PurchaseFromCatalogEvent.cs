namespace WibboEmulator.Communication.Packets.Incoming.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Badges;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Bots;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Pets;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Core;
using WibboEmulator.Games.Catalog.Utilities;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Users.Inventory.Bots;
using WibboEmulator.Games.Groups;
using WibboEmulator.Games.Items;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Database.Daos.Catalog;
using WibboEmulator.Database.Daos.Item;

internal class PurchaseFromCatalogEvent : IPacketEvent
{
    public double Delay => 1000;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        var PageId = Packet.PopInt();
        var ItemId = Packet.PopInt();
        var ExtraData = Packet.PopString();
        var Amount = Packet.PopInt();

        if (!WibboEnvironment.GetGame().GetCatalog().TryGetPage(PageId, out var Page))
        {
            return;
        }

        if (!Page.Enabled || Page.MinimumRank > session.GetUser().Rank)
        {
            return;
        }

        if (!Page.Items.TryGetValue(ItemId, out var Item))
        {
            if (Page.ItemOffers.ContainsKey(ItemId))
            {
                Item = Page.ItemOffers[ItemId];
                if (Item == null)
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }

        if (Amount < 1 || Amount > 100 || !ItemUtility.CanSelectAmount(Item))
        {
            Amount = 1;
        }

        var AmountPurchase = Item.Amount > 1 ? Item.Amount : Amount;

        var TotalCreditsCost = Amount > 1 ? Item.CostCredits * Amount - (int)Math.Floor((double)Amount / 6) * Item.CostCredits : Item.CostCredits;
        var TotalPixelCost = Amount > 1 ? Item.CostDuckets * Amount - (int)Math.Floor((double)Amount / 6) * Item.CostDuckets : Item.CostDuckets;
        var TotalDiamondCost = Amount > 1 ? Item.CostWibboPoints * Amount - (int)Math.Floor((double)Amount / 6) * Item.CostWibboPoints : Item.CostWibboPoints;
        var TotalLimitCoinCost = Amount > 1 ? Item.CostLimitCoins * Amount - (int)Math.Floor((double)Amount / 6) * Item.CostLimitCoins : Item.CostLimitCoins;

        if (session.GetUser().Credits < TotalCreditsCost ||
            session.GetUser().Duckets < TotalPixelCost ||
            session.GetUser().WibboPoints < TotalDiamondCost ||
            session.GetUser().LimitCoins < TotalLimitCoinCost)
        {
            return;
        }

        var LimitedEditionSells = 0;
        var LimitedEditionStack = 0;

        switch (Item.Data.InteractionType)
        {
            case InteractionType.NONE:
                ExtraData = "";
                break;

            case InteractionType.GUILD_ITEM:
            case InteractionType.GUILD_GATE:
                int GroupId;
                if (!int.TryParse(ExtraData, out GroupId))
                {
                    return;
                }

                if (GroupId == 0)
                {
                    return;
                }

                Group Group;
                if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group))
                {
                    return;
                }

                ExtraData = "0;" + Group.Id;
                break;

            case InteractionType.PET:
                var Bits = ExtraData.Split('\n');
                var PetName = Bits[0];
                var Race = Bits[1];
                var Color = Bits[2];

                if (!int.TryParse(Race, out var result))
                {
                    return;
                }

                if (!PetUtility.CheckPetName(PetName))
                {
                    return;
                }

                if (Race.Length > 2)
                {
                    return;
                }

                if (Color.Length != 6)
                {
                    return;
                }

                WibboEnvironment.GetGame().GetAchievementManager().ProgressAchievement(session, "ACH_PetLover", 1);

                break;

            case InteractionType.FLOOR:
            case InteractionType.WALLPAPER:
            case InteractionType.LANDSCAPE:

                double Number = 0;

                try
                {
                    if (string.IsNullOrEmpty(ExtraData))
                    {
                        Number = 0;
                    }
                    else
                    {
                        Number = double.Parse(ExtraData);
                    }
                }
                catch (Exception e)
                {
                    ExceptionLogger.LogException(e.ToString());
                }

                ExtraData = Number.ToString().Replace(',', '.');
                break; // maintain extra data // todo: validate

            case InteractionType.POSTIT:
                ExtraData = "FFFF33";
                break;

            case InteractionType.MOODLIGHT:
                ExtraData = "1,1,1,#000000,255";
                break;

            case InteractionType.TROPHY:
                ExtraData = session.GetUser().Username + Convert.ToChar(9) + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + Convert.ToChar(9) + ExtraData;
                break;

            case InteractionType.MANNEQUIN:
                ExtraData = "m;ch-210-1321.lg-285-92;Mannequin";
                break;

            case InteractionType.BADGE_TROC:
            {
                if (WibboEnvironment.GetGame().GetBadgeManager().HaveNotAllowed(ExtraData) || !WibboEnvironment.GetGame().GetCatalog().HasBadge(ExtraData))
                {
                    session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.buybadgedisplay.error", session.Langue));
                    session.SendPacket(new PurchaseOKComposer());
                    return;
                }

                if (!ExtraData.StartsWith("perso_"))
                {
                    session.GetUser().GetBadgeComponent().RemoveBadge(ExtraData);
                }

                session.SendPacket(new BadgesComposer(session.GetUser().GetBadgeComponent().BadgeList));

                break;
            }

            case InteractionType.BADGE_DISPLAY:
                if (WibboEnvironment.GetGame().GetBadgeManager().HaveNotAllowed(ExtraData) || !session.GetUser().GetBadgeComponent().HasBadge(ExtraData))
                {
                    session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.buybadgedisplay.error", session.Langue));
                    session.SendPacket(new PurchaseOKComposer());
                    return;
                }

                ExtraData = ExtraData + Convert.ToChar(9) + session.GetUser().Username + Convert.ToChar(9) + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
                break;

            case InteractionType.BADGE:
            {
                if (session.GetUser().GetBadgeComponent().HasBadge(Item.Badge))
                {
                    session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.buybadge.error", session.Langue));
                    session.SendPacket(new PurchaseOKComposer());
                    return;
                }
                break;
            }
            default:
                ExtraData = "";
                break;
        }


        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();

        if (Item.IsLimited)
        {
            if (Item.LimitedEditionStack <= Item.LimitedEditionSells)
            {
                session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.buyltd.error", session.Langue));
                session.SendPacket(new PurchaseOKComposer());
                return;
            }

            Interlocked.Increment(ref Item.LimitedEditionSells);

            CatalogItemLimitedDao.Update(dbClient, Item.Id, Item.LimitedEditionSells);

            LimitedEditionSells = Item.LimitedEditionSells;
            LimitedEditionStack = Item.LimitedEditionStack;
        }

        if (Item.CostCredits > 0)
        {
            session.GetUser().Credits -= TotalCreditsCost;
            session.SendPacket(new CreditBalanceComposer(session.GetUser().Credits));
        }

        if (Item.CostDuckets > 0)
        {
            session.GetUser().Duckets -= TotalPixelCost;
            session.SendPacket(new ActivityPointNotificationComposer(session.GetUser().Duckets, session.GetUser().Duckets));
        }

        if (Item.CostWibboPoints > 0)
        {
            session.GetUser().WibboPoints -= TotalDiamondCost;
            session.SendPacket(new ActivityPointNotificationComposer(session.GetUser().WibboPoints, 0, 105));

            UserDao.UpdateRemovePoints(dbClient, session.GetUser().Id, TotalDiamondCost);
        }

        if (Item.CostLimitCoins > 0)
        {
            session.GetUser().LimitCoins -= TotalLimitCoinCost;
            session.SendPacket(new ActivityPointNotificationComposer(session.GetUser().LimitCoins, 0, 55));

            UserDao.UpdateRemoveLimitCoins(dbClient, session.GetUser().Id, TotalLimitCoinCost);
        }

        switch (Item.Data.Type.ToString().ToLower())
        {
            default:
                var GeneratedGenericItems = new List<Item>();


                Item NewItem;
                switch (Item.Data.InteractionType)
                {
                    default:
                        if (AmountPurchase > 1)
                        {
                            var Items = ItemFactory.CreateMultipleItems(Item.Data, session.GetUser(), ExtraData, AmountPurchase);

                            if (Items != null)
                            {
                                GeneratedGenericItems.AddRange(Items);
                            }
                        }
                        else
                        {
                            NewItem = ItemFactory.CreateSingleItemNullable(Item.Data, session.GetUser(), ExtraData, LimitedEditionSells, LimitedEditionStack);

                            if (NewItem != null)
                            {
                                GeneratedGenericItems.Add(NewItem);
                            }
                        }
                        break;

                    case InteractionType.TELEPORT:
                    case InteractionType.ARROW:
                        for (var i = 0; i < AmountPurchase; i++)
                        {
                            var TeleItems = ItemFactory.CreateTeleporterItems(Item.Data, session.GetUser());

                            if (TeleItems != null)
                            {
                                GeneratedGenericItems.AddRange(TeleItems);
                            }
                        }
                        break;

                    case InteractionType.MOODLIGHT:
                    {
                        if (AmountPurchase > 1)
                        {
                            var Items = ItemFactory.CreateMultipleItems(Item.Data, session.GetUser(), ExtraData, AmountPurchase);

                            if (Items != null)
                            {
                                GeneratedGenericItems.AddRange(Items);
                                foreach (var item in Items)
                                {
                                    ItemFactory.CreateMoodlightData(item);
                                }
                            }
                        }
                        else
                        {
                            NewItem = ItemFactory.CreateSingleItemNullable(Item.Data, session.GetUser(), ExtraData);

                            if (NewItem != null)
                            {
                                GeneratedGenericItems.Add(NewItem);
                                ItemFactory.CreateMoodlightData(NewItem);
                            }
                        }
                    }
                    break;
                }

                foreach (var PurchasedItem in GeneratedGenericItems)
                {
                    if (session.GetUser().GetInventoryComponent().TryAddItem(PurchasedItem))
                    {
                        session.SendPacket(new FurniListNotificationComposer(PurchasedItem.Id, 1));
                    }
                }

                if (Item.Data.Amount >= 0)
                {
                    Item.Data.Amount += GeneratedGenericItems.Count;
                    ItemStatDao.UpdateAdd(dbClient, Item.Data.Id, GeneratedGenericItems.Count);
                }
                break;

            case "r":
                var Bot = BotUtility.CreateBot(Item.Data, session.GetUser().Id);
                if (Bot != null)
                {
                    session.GetUser().GetInventoryComponent().TryAddBot(Bot);
                    session.SendPacket(new BotInventoryComposer(session.GetUser().GetInventoryComponent().GetBots()));
                    session.SendPacket(new FurniListNotificationComposer(Bot.Id, 5));
                }
                break;

            case "p":
            {
                var PetData = ExtraData.Split('\n');

                var GeneratedPet = PetUtility.CreatePet(session.GetUser().Id, PetData[0], Item.Data.SpriteId, PetData[1], PetData[2]);
                if (GeneratedPet != null)
                {
                    session.GetUser().GetInventoryComponent().TryAddPet(GeneratedPet);

                    session.SendPacket(new FurniListNotificationComposer(GeneratedPet.PetId, 3));
                    session.SendPacket(new PetInventoryComposer(session.GetUser().GetInventoryComponent().GetPets()));
                }
                break;
            }

            case "b":
            {
                break;
            }
        }

        if (!string.IsNullOrEmpty(Item.Badge) && !session.GetUser().GetBadgeComponent().HasBadge(Item.Badge))
        {
            session.GetUser().GetBadgeComponent().GiveBadge(Item.Badge, true);
            session.SendPacket(new ReceiveBadgeComposer(Item.Badge));

            session.SendPacket(new FurniListNotificationComposer(0, 4));
        }

        session.SendPacket(new PurchaseOKComposer(Item, Item.Data));
    }
}
