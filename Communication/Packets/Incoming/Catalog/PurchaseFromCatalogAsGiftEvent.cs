using WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Badges;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Catalog;
using WibboEmulator.Games.Catalog.Utilities;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;
using WibboEmulator.Games.Items;
using WibboEmulator.Utilities;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class PurchaseFromCatalogAsGiftEvent : IPacketEvent
    {
        public double Delay => 1000;

        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int PageId = Packet.PopInt();
            int ItemId = Packet.PopInt();
            string Data = Packet.PopString();
            string GiftUser = StringCharFilter.Escape(Packet.PopString());
            string GiftMessage = StringCharFilter.Escape(Packet.PopString().Replace(Convert.ToChar(5), ' '));
            int SpriteId = Packet.PopInt();
            int Ribbon = Packet.PopInt();
            int Colour = Packet.PopInt();
            bool dnow = Packet.PopBoolean();

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
                return;
            }

            if (!ItemUtility.CanGiftItem(Item))
            {
                return;
            }

            if (!WibboEnvironment.GetGame().GetItemManager().GetGift(SpriteId, out ItemData PresentData) || PresentData.InteractionType != InteractionType.GIFT)
            {
                return;
            }

            int TotalCreditsCost = Item.CostCredits;
            int TotalPixelCost = Item.CostDuckets;
            int TotalDiamondCost = Item.CostWibboPoints;
            int TotalLimitCoinCost = Item.CostLimitCoins;

            if (Session.GetUser().Credits < TotalCreditsCost ||
                Session.GetUser().Duckets < TotalPixelCost ||
                Session.GetUser().WibboPoints < TotalDiamondCost ||
                Session.GetUser().LimitCoins < TotalLimitCoinCost)
            {
                return;
            }

            User user = WibboEnvironment.GetUserByUsername(GiftUser);
            if (user == null)
            {
                //Session.SendPacket(new GiftWrappingErrorComposer());
                return;
            }

            if ((DateTime.Now - Session.GetUser().LastGiftPurchaseTime).TotalSeconds <= 15.0)
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.buygift.flood", Session.Langue));

                Session.GetUser().GiftPurchasingWarnings += 1;
                if (Session.GetUser().GiftPurchasingWarnings >= 25)
                {
                    Session.GetUser().SessionGiftBlocked = true;
                }

                return;
            }

            if (Session.GetUser().SessionGiftBlocked)
            {
                return;
            }

            string ED = Session.GetUser().Id + ";" + GiftMessage + Convert.ToChar(5) + Ribbon + Convert.ToChar(5) + Colour;

            int NewItemId = 0;
            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                NewItemId = ItemDao.Insert(dbClient, PresentData.Id, user.Id, ED);

                string ItemExtraData = "";
                switch (Item.Data.InteractionType)
                {
                    case InteractionType.NONE:
                        ItemExtraData = "";
                        break;

                    case InteractionType.GUILD_ITEM:
                    case InteractionType.GUILD_GATE:
                        int Groupid = 0;
                        if (!int.TryParse(Data, out Groupid))
                        {
                            return;
                        }

                        if (Groupid == 0)
                        {
                            return;
                        }

                        Group groupItem;
                        if (WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(Groupid, out groupItem))
                        {
                            ItemExtraData = "0;" + groupItem.Id;
                        }

                        break;

                    case InteractionType.PET:

                        try
                        {
                            string[] Bits = Data.Split('\n');
                            string PetName = Bits[0];
                            string Race = Bits[1];
                            string Color = Bits[2];

                            int.Parse(Race); // to trigger any possible errors

                            if (PetUtility.CheckPetName(PetName))
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
                        }
                        catch
                        {
                            return;
                        }

                        break;

                    case InteractionType.FLOOR:
                    case InteractionType.WALLPAPER:
                    case InteractionType.LANDSCAPE:

                        double Number = 0;
                        try
                        {
                            if (string.IsNullOrEmpty(Data))
                            {
                                Number = 0;
                            }
                            else
                            {
                                Number = double.Parse(Data);
                            }
                        }
                        catch
                        {

                        }

                        ItemExtraData = Number.ToString().Replace(',', '.');
                        break; // maintain extra data // todo: validate

                    case InteractionType.POSTIT:
                        ItemExtraData = "FFFF33";
                        break;

                    case InteractionType.MOODLIGHT:
                        ItemExtraData = "1,1,1,#000000,255";
                        break;

                    case InteractionType.TROPHY:
                        ItemExtraData = Session.GetUser().Username + Convert.ToChar(9) + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + Convert.ToChar(9) + Data;
                        break;

                    case InteractionType.MANNEQUIN:
                        ItemExtraData = "m;ch-210-1321.lg-285-92;Mannequin";
                        break;

                    case InteractionType.BADGE_TROC:
                        {
                            if (WibboEnvironment.GetGame().GetBadgeManager().HaveNotAllowed(Data) || !WibboEnvironment.GetGame().GetCatalog().HasBadge(Data))
                            {
                                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.buybadgedisplay.error", Session.Langue));
                                return;
                            }

                            if (!Data.StartsWith("perso_"))
                            {
                                Session.GetUser().GetBadgeComponent().RemoveBadge(Data);
                            }

                            Session.SendPacket(new BadgesComposer(Session.GetUser().GetBadgeComponent().BadgeList));

                            ItemExtraData = Data;
                            break;
                        }

                    case InteractionType.BADGE_DISPLAY:
                        if (WibboEnvironment.GetGame().GetBadgeManager().HaveNotAllowed(Data) || !Session.GetUser().GetBadgeComponent().HasBadge(Data))
                        {
                            Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.buybadgedisplay.error", Session.Langue));
                            Session.SendPacket(new PurchaseOKComposer());
                            return;
                        }

                        ItemExtraData = Data + Convert.ToChar(9) + Session.GetUser().Username + Convert.ToChar(9) + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
                        break;

                    default:
                        ItemExtraData = Data;
                        break;
                }

                UserPresentDao.Insert(dbClient, NewItemId, Item.Data.Id, ItemExtraData);

                ItemDao.Delete(dbClient, NewItemId);
            }


            Item GiveItem = ItemFactory.CreateSingleItem(PresentData, user, ED, NewItemId);
            if (GiveItem != null)
            {
                GameClient Receiver = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(user.Id);
                if (Receiver != null)
                {
                    Receiver.GetUser().GetInventoryComponent().TryAddItem(GiveItem);
                    Receiver.SendPacket(new FurniListNotificationComposer(GiveItem.Id, 1));
                    Receiver.SendPacket(new PurchaseOKComposer());
                    //Receiver.SendPacket(new FurniListUpdateComposer());
                }

                if (user.Id != Session.GetUser().Id && !string.IsNullOrWhiteSpace(GiftMessage))
                {
                    WibboEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_GiftGiver", 1);
                    if (Receiver != null)
                    {
                        WibboEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Receiver, "ACH_GiftReceiver", 1);
                    }
                }
            }

            Session.SendPacket(new PurchaseOKComposer(Item, PresentData));

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
                Session.GetUser().LimitCoins -= TotalLimitCoinCost;
                Session.SendPacket(new ActivityPointNotificationComposer(Session.GetUser().LimitCoins, 0, 55));

                using IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
                UserDao.UpdateRemoveLimitCoins(dbClient, Session.GetUser().Id, TotalLimitCoinCost);
            }

            Session.GetUser().LastGiftPurchaseTime = DateTime.Now;
        }
    }
}
