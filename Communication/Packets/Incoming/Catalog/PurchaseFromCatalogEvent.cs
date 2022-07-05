using WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Badges;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Bots;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Pets;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Communication.Packets.Outgoing.Users;

using WibboEmulator.Core;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Game.Catalog;
using WibboEmulator.Game.Catalog.Utilities;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Groups;
using WibboEmulator.Game.Items;
using WibboEmulator.Game.Pets;
using WibboEmulator.Game.Users.Inventory.Bots;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class PurchaseFromCatalogEvent : IPacketEvent
    {
        public double Delay => 1000;

        public void Parse(Client Session, ClientPacket Packet)
        {
            int PageId = Packet.PopInt();
            int ItemId = Packet.PopInt();
            string ExtraData = Packet.PopString();
            int Amount = Packet.PopInt();

            if (!WibboEnvironment.GetGame().GetCatalog().TryGetPage(PageId, out CatalogPage Page))
            {
                return;
            }

            if (!Page.Enabled || Page.MinimumRank > Session.GetUser().Rank)
            {
                return;
            }

            if (!Page.Items.TryGetValue(ItemId, out CatalogItem Item))
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

            int AmountPurchase = Item.Amount > 1 ? Item.Amount : Amount;

            int TotalCreditsCost = Amount > 1 ? ((Item.CostCredits * Amount) - ((int)Math.Floor((double)Amount / 6) * Item.CostCredits)) : Item.CostCredits;
            int TotalPixelCost = Amount > 1 ? ((Item.CostDuckets * Amount) - ((int)Math.Floor((double)Amount / 6) * Item.CostDuckets)) : Item.CostDuckets;
            int TotalDiamondCost = Amount > 1 ? ((Item.CostWibboPoints * Amount) - ((int)Math.Floor((double)Amount / 6) * Item.CostWibboPoints)) : Item.CostWibboPoints;
            int TotalLimitCoinCost = Amount > 1 ? ((Item.CostLimitCoins * Amount) - ((int)Math.Floor((double)Amount / 6) * Item.CostLimitCoins)) : Item.CostLimitCoins;

            if (Session.GetUser().Credits < TotalCreditsCost || 
                Session.GetUser().Duckets < TotalPixelCost || 
                Session.GetUser().WibboPoints < TotalDiamondCost ||
                Session.GetUser().LimitCoins < TotalLimitCoinCost)
            {
                return;
            }

            if (AmountPurchase > 1)
            {
                Session.SendPacket(new PurchaseOKComposer(Item, Item.Data));
            }

            int LimitedEditionSells = 0;
            int LimitedEditionStack = 0;

            #region Create the extradata
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

                #region Pet handling

                case InteractionType.PET:
                    string[] Bits = ExtraData.Split('\n');
                    string PetName = Bits[0];
                    string Race = Bits[1];
                    string Color = Bits[2];

                    if (!int.TryParse(Race, out int result))
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

                    WibboEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_PetLover", 1);

                    break;

                #endregion

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
                        ExceptionLogger.LogException((e).ToString());
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
                    ExtraData = Session.GetUser().Username + Convert.ToChar(9) + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + Convert.ToChar(9) + ExtraData;
                    break;

                case InteractionType.MANNEQUIN:
                    ExtraData = "m;ch-210-1321.lg-285-92;Mannequin";
                    break;

                case InteractionType.BADGE_TROC:
                    {
                        if (WibboEnvironment.GetGame().GetBadgeManager().HaveNotAllowed(ExtraData) || !WibboEnvironment.GetGame().GetCatalog().HasBadge(ExtraData))
                        {
                            Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.buybadgedisplay.error", Session.Langue));
                            Session.SendPacket(new PurchaseOKComposer());
                            return;
                        }

                        if (!ExtraData.StartsWith("perso_"))
                        {
                            Session.GetUser().GetBadgeComponent().RemoveBadge(ExtraData);
                        }

                        Session.SendPacket(new BadgesComposer(Session.GetUser().GetBadgeComponent().BadgeList));

                        break;
                    }

                case InteractionType.BADGE_DISPLAY:
                    if (WibboEnvironment.GetGame().GetBadgeManager().HaveNotAllowed(ExtraData) || !Session.GetUser().GetBadgeComponent().HasBadge(ExtraData))
                    {
                        Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.buybadgedisplay.error", Session.Langue));
                        Session.SendPacket(new PurchaseOKComposer());
                        return;
                    }

                    ExtraData = ExtraData + Convert.ToChar(9) + Session.GetUser().Username + Convert.ToChar(9) + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
                    break;

                case InteractionType.BADGE:
                    {
                        if (Session.GetUser().GetBadgeComponent().HasBadge(Item.Badge))
                        {
                            Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.buybadge.error", Session.Langue));
                            Session.SendPacket(new PurchaseOKComposer());
                            return;
                        }
                        break;
                    }
                default:
                    ExtraData = "";
                    break;
            }
            #endregion


            if (Item.IsLimited)
            {
                if (Item.LimitedEditionStack <= Item.LimitedEditionSells)
                {
                    Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.buyltd.error", Session.Langue));
                    Session.SendPacket(new PurchaseOKComposer());
                    return;
                }

                Interlocked.Increment(ref Item.LimitedEditionSells);
                using IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
                CatalogItemDao.UpdateLimited(dbClient, Item.Id, Item.LimitedEditionSells);

                LimitedEditionSells = Item.LimitedEditionSells;
                LimitedEditionStack = Item.LimitedEditionStack;
            }

            if (Item.CostCredits > 0)
            {
                Session.GetUser().Credits -= TotalCreditsCost;
                Session.SendPacket(new CreditBalanceComposer(Session.GetUser().Credits));
            }

            if (Item.CostDuckets > 0)
            {
                Session.GetUser().Duckets -= TotalPixelCost;
                Session.SendPacket(new ActivityPointNotificationComposer(Session.GetUser().Duckets, Session.GetUser().Duckets));
            }

            if (Item.CostWibboPoints > 0)
            {
                Session.GetUser().WibboPoints -= TotalDiamondCost;
                Session.SendPacket(new ActivityPointNotificationComposer(Session.GetUser().WibboPoints, 0, 105));

                using IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
                UserDao.UpdateRemovePoints(dbClient, Session.GetUser().Id, TotalDiamondCost);
            }

            if (Item.CostLimitCoins > 0)
            {
                Session.GetUser().LimitCoins -= TotalDiamondCost;
                Session.SendPacket(new ActivityPointNotificationComposer(Session.GetUser().LimitCoins, 0, 55));

                using IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
                UserDao.UpdateRemoveLimitCoins(dbClient, Session.GetUser().Id, TotalDiamondCost);
            }

            switch (Item.Data.Type.ToString().ToLower())
            {
                default:
                    List<Item> GeneratedGenericItems = new List<Item>();


                    Item NewItem;
                    switch (Item.Data.InteractionType)
                    {
                        default:
                            if (AmountPurchase > 1)
                            {
                                List<Item> Items = ItemFactory.CreateMultipleItems(Item.Data, Session.GetUser(), ExtraData, AmountPurchase);

                                if (Items != null)
                                {
                                    GeneratedGenericItems.AddRange(Items);
                                }
                            }
                            else
                            {
                                NewItem = ItemFactory.CreateSingleItemNullable(Item.Data, Session.GetUser(), ExtraData, LimitedEditionSells, LimitedEditionStack);

                                if (NewItem != null)
                                {
                                    GeneratedGenericItems.Add(NewItem);
                                }
                            }
                            break;

                        case InteractionType.TELEPORT:
                        case InteractionType.ARROW:
                            for (int i = 0; i < AmountPurchase; i++)
                            {
                                List<Item> TeleItems = ItemFactory.CreateTeleporterItems(Item.Data, Session.GetUser());

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
                                    List<Item> Items = ItemFactory.CreateMultipleItems(Item.Data, Session.GetUser(), ExtraData, AmountPurchase);

                                    if (Items != null)
                                    {
                                        GeneratedGenericItems.AddRange(Items);
                                        foreach (Item I in Items)
                                        {
                                            ItemFactory.CreateMoodlightData(I);
                                        }
                                    }
                                }
                                else
                                {
                                    NewItem = ItemFactory.CreateSingleItemNullable(Item.Data, Session.GetUser(), ExtraData);

                                    if (NewItem != null)
                                    {
                                        GeneratedGenericItems.Add(NewItem);
                                        ItemFactory.CreateMoodlightData(NewItem);
                                    }
                                }
                            }
                            break;
                    }

                    foreach (Item PurchasedItem in GeneratedGenericItems)
                    {
                        if (Session.GetUser().GetInventoryComponent().TryAddItem(PurchasedItem))
                        {
                            Session.SendPacket(new FurniListNotificationComposer(PurchasedItem.Id, 1));
                        }
                    }
                    break;

                case "r":
                    Bot Bot = BotUtility.CreateBot(Item.Data, Session.GetUser().Id);
                    if (Bot != null)
                    {
                        Session.GetUser().GetInventoryComponent().TryAddBot(Bot);
                        Session.SendPacket(new BotInventoryComposer(Session.GetUser().GetInventoryComponent().GetBots()));
                        Session.SendPacket(new FurniListNotificationComposer(Bot.Id, 5));
                    }
                    break;

                case "p":
                    {
                        string[] PetData = ExtraData.Split('\n');

                        Pet GeneratedPet = PetUtility.CreatePet(Session.GetUser().Id, PetData[0], Item.Data.SpriteId, PetData[1], PetData[2]);
                        if (GeneratedPet != null)
                        {
                            Session.GetUser().GetInventoryComponent().TryAddPet(GeneratedPet);

                            Session.SendPacket(new FurniListNotificationComposer(GeneratedPet.PetId, 3));
                            Session.SendPacket(new PetInventoryComposer(Session.GetUser().GetInventoryComponent().GetPets()));
                        }
                        break;
                    }

                case "b":
                    {
                        break;
                    }
            }

            if (!string.IsNullOrEmpty(Item.Badge) && !Session.GetUser().GetBadgeComponent().HasBadge(Item.Badge))
            {
                Session.GetUser().GetBadgeComponent().GiveBadge(Item.Badge, true);
                Session.SendPacket(new ReceiveBadgeComposer(Item.Badge));

                Session.SendPacket(new FurniListNotificationComposer(0, 4));
            }

            Session.SendPacket(new PurchaseOKComposer(Item, Item.Data));
        }
    }
}