using Wibbo.Communication.Packets.Outgoing.Inventory.Achievements;
using Wibbo.Communication.Packets.Outgoing.Inventory.Purse;
using Wibbo.Communication.Packets.Outgoing.Rooms.Engine;
using Wibbo.Communication.Packets.Outgoing.Rooms.Furni;
using Wibbo.Communication.Packets.Outgoing.Users;
using Wibbo.Database.Daos;
using Wibbo.Database.Interfaces;
using Wibbo.Game.Catalog;
using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Game.Items
{
    internal static class ItemLotBox
    {
        public static void OpenExtrabox(Client Session, Item Present, Room Room)
        {
            int PageId;

            if (WibboEnvironment.GetRandomNumber(1, 750) == 750) //Epic rare
            {
                PageId = 84641;
            }
            else if (WibboEnvironment.GetRandomNumber(1, 100) == 100) //Commun rare
            {
                PageId = 98747;
            }
            else if (WibboEnvironment.GetRandomNumber(1, 75) == 75) //WibboPoint
            {
                PageId = 15987;
            }
            else if (WibboEnvironment.GetRandomNumber(1, 25) == 25) //Pièce Win-win
            {
                PageId = 456465;
            }
            else
            {
                PageId = 894948; //Rare
            }

            WibboEnvironment.GetGame().GetCatalog().TryGetPage(PageId, out CatalogPage Page);
            if (Page == null)
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.error", Session.Langue));
                return;
            }

            ItemData LotData = Page.Items.ElementAt(WibboEnvironment.GetRandomNumber(0, Page.Items.Count - 1)).Value.Data;
            if (LotData == null)
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.error", Session.Langue));
                return;
            }

            Room.GetRoomItemHandler().RemoveFurniture(Session, Present.Id);

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                ItemDao.UpdateBaseItem(dbClient, Present.Id, LotData.Id);
            }

            string FurniType = Present.GetBaseItem().Type.ToString().ToLower();
            Present.BaseItem = LotData.Id;
            Present.ResetBaseItem();

            bool ItemIsInRoom = true;

            if (Present.Data.Type == 's')
            {
                if (!Room.GetRoomItemHandler().SetFloorItem(Session, Present, Present.X, Present.Y, Present.Rotation, true, false, true))
                {
                    using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        ItemDao.UpdateResetRoomId(dbClient, Present.Id);
                    }

                    ItemIsInRoom = false;
                }
            }
            else
            {
                using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    ItemDao.UpdateResetRoomId(dbClient, Present.Id);
                }

                ItemIsInRoom = false;
            }

            Session.SendPacket(new OpenGiftComposer(Present.Data, Present.ExtraData, Present, ItemIsInRoom));

            if (!ItemIsInRoom)
            {
                Session.GetUser().GetInventoryComponent().TryAddItem(Present);
            }
        }

        public static void OpenDeluxeBox(Client Session, Item Present, Room Room)
        {
            int PageId;

            if (WibboEnvironment.GetRandomNumber(1, 200) == 200) //Epique
            {
                PageId = 1635463617;
            }
            else if (WibboEnvironment.GetRandomNumber(1, 20) == 20) //Commun
            {
                PageId = 1635463616;
            }
            else
            {
                PageId = 91700214; //Basique
            }

            WibboEnvironment.GetGame().GetCatalog().TryGetPage(PageId, out CatalogPage Page);
            if (Page == null)
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.error", Session.Langue));
                return;
            }

            ItemData LotData = Page.Items.ElementAt(WibboEnvironment.GetRandomNumber(0, Page.Items.Count - 1)).Value.Data;
            if (LotData == null)
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.error", Session.Langue));
                return;
            }

            Room.GetRoomItemHandler().RemoveFurniture(Session, Present.Id);

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                ItemDao.UpdateBaseItem(dbClient, Present.Id, LotData.Id);
            }

            string FurniType = Present.GetBaseItem().Type.ToString().ToLower();
            Present.BaseItem = LotData.Id;
            Present.ResetBaseItem();

            bool ItemIsInRoom = true;

            if (Present.Data.Type == 's')
            {
                if (!Room.GetRoomItemHandler().SetFloorItem(Session, Present, Present.X, Present.Y, Present.Rotation, true, false, true))
                {
                    using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        ItemDao.UpdateResetRoomId(dbClient, Present.Id);
                    }

                    ItemIsInRoom = false;
                }
            }
            else
            {
                using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    ItemDao.UpdateResetRoomId(dbClient, Present.Id);
                }

                ItemIsInRoom = false;
            }

            Session.SendPacket(new OpenGiftComposer(Present.Data, Present.ExtraData, Present, ItemIsInRoom));

            if (!ItemIsInRoom)
            {
                Session.GetUser().GetInventoryComponent().TryAddItem(Present);
            }
        }

        public static void OpenBadgeBox(Client Session, Item Present, Room Room)
        {

            //Présentoir et badge
            int PageId = 987987;

            List<int> PageBadgeList = new List<int>(new int[] { 8948, 18171, 18172, 18173, 18174, 18175, 18176, 18177, 18178, 18179, 18180, 18181, 18182, 18183 });
            int PageBadgeId = PageBadgeList[WibboEnvironment.GetRandomNumber(0, PageBadgeList.Count - 1)];
            WibboEnvironment.GetGame().GetCatalog().TryGetPage(PageBadgeId, out CatalogPage PageBadge);
            if (PageBadge == null)
            {
                return;
            }

            string BadgeCode = PageBadge.Items.ElementAt(WibboEnvironment.GetRandomNumber(0, PageBadge.Items.Count - 1)).Value.Badge;


            WibboEnvironment.GetGame().GetCatalog().TryGetPage(PageId, out CatalogPage Page);
            if (Page == null)
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.error", Session.Langue));
                return;
            }

            ItemData LotData = Page.Items.ElementAt(WibboEnvironment.GetRandomNumber(0, Page.Items.Count - 1)).Value.Data;
            if (LotData == null)
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.error", Session.Langue));
                return;
            }

            Room.GetRoomItemHandler().RemoveFurniture(Session, Present.Id);

            string ExtraData = BadgeCode + Convert.ToChar(9) + Session.GetUser().Username + Convert.ToChar(9) + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                ItemDao.UpdateBaseItemAndExtraData(dbClient, Present.Id, LotData.Id, ExtraData);
            }

            Present.ExtraData = ExtraData;

            Present.BaseItem = LotData.Id;
            Present.ResetBaseItem();

            bool ItemIsInRoom = true;

            if (Present.Data.Type == 's')
            {
                if (!Room.GetRoomItemHandler().SetFloorItem(Session, Present, Present.X, Present.Y, Present.Rotation, true, false, true))
                {
                    using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        ItemDao.UpdateResetRoomId(dbClient, Present.Id);
                    }

                    ItemIsInRoom = false;
                }
            }
            else
            {
                using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    ItemDao.UpdateResetRoomId(dbClient, Present.Id);
                }

                ItemIsInRoom = false;
            }

            Session.SendPacket(new OpenGiftComposer(Present.Data, Present.ExtraData, Present, ItemIsInRoom));

            if (!ItemIsInRoom)
            {
                Session.GetUser().GetInventoryComponent().TryAddItem(Present);
            }

            if (!string.IsNullOrEmpty(BadgeCode) && !Session.GetUser().GetBadgeComponent().HasBadge(BadgeCode))
            {
                Session.GetUser().GetBadgeComponent().GiveBadge(BadgeCode, true);
                Session.SendPacket(new ReceiveBadgeComposer(BadgeCode));

                RoomUser roomUserByUserId = Room.GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);
                if (roomUserByUserId != null)
                {
                    roomUserByUserId.SendWhisperChat("Tu as reçu le badge: " + BadgeCode);
                }
            }
        }

        public static void OpenLegendBox(Client session, Item present, Room room)
        {
            int pageId = 0;
            string badgeCode = "";
            string lotType = "";
            int forceItem = 0;

            if (WibboEnvironment.GetRandomNumber(1, 100) == 100) //Legendaire
            {
                pageId = 14514;
                lotType = "Légendaire";
            }
            else if (WibboEnvironment.GetRandomNumber(1, 75) == 75) //Royal
            {
                pageId = 584545;
                lotType = "Royal";
                forceItem = 37951979;
            }
            else if (WibboEnvironment.GetRandomNumber(1, 30) == 30) //Royal
            {
                pageId = 584545;
                lotType = "Royal";
                forceItem = 70223722;
            }
            else if (WibboEnvironment.GetRandomNumber(1, 15) == 15) //Epique
            {
                pageId = 84641;
                lotType = "épique";
            }
            else if (WibboEnvironment.GetRandomNumber(1, 5) == 5) //Royal
            {
                pageId = 584545;
                lotType = "Royal";
                forceItem = 52394359;
            }
            else
            {
                pageId = 98747;
                lotType = "commun";
            }

            int pageBadgeId = 841878;

            if (!WibboEnvironment.GetGame().GetCatalog().TryGetPage(pageBadgeId, out CatalogPage pageBadge))
            {
                session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.error", session.Langue));
                return;
            }

            foreach (KeyValuePair<int, CatalogItem> Item in pageBadge.Items.OrderBy(a => Guid.NewGuid()).ToList())
            {
                if (session.GetUser().GetBadgeComponent().HasBadge(Item.Value.Badge))
                {
                    continue;
                }

                badgeCode = Item.Value.Badge;
                break;
            }

            
            if (!WibboEnvironment.GetGame().GetCatalog().TryGetPage(pageId, out CatalogPage page))
            {
                session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.error", session.Langue));
                return;
            }

            ItemData lotData = null;

            if (forceItem == 0)
            {
                lotData = page.Items.ElementAt(WibboEnvironment.GetRandomNumber(0, page.Items.Count - 1)).Value.Data;
            }
            else
            {
                lotData = page.GetItem(forceItem).Data;
            }

            if (lotData == null)
            {
                session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.error", session.Langue));
                return;
            }

            int credits = WibboEnvironment.GetRandomNumber(100, 10000) * 1000;
            session.GetUser().Credits += credits;
            session.SendPacket(new CreditBalanceComposer(session.GetUser().Credits));

            int winwin = WibboEnvironment.GetRandomNumber(100, 1000);
            session.GetUser().AchievementPoints += winwin;

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                UserStatsDao.UpdateAchievementScore(dbClient, session.GetUser().Id, winwin);
            }

            session.SendPacket(new AchievementScoreComposer(session.GetUser().AchievementPoints));

            RoomUser roomUserByUserId = room.GetRoomUserManager().GetRoomUserByUserId(session.GetUser().Id);
            if (roomUserByUserId != null)
            {
                session.SendPacket(new UserChangeComposer(roomUserByUserId, true));
                room.SendPacket(new UserChangeComposer(roomUserByUserId, false));

                roomUserByUserId.SendWhisperChat(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("item.legendboxlot", session.Langue), credits, winwin, badgeCode, lotType));
            }

            if (!string.IsNullOrEmpty(badgeCode))
            {
                session.GetUser().GetBadgeComponent().GiveBadge(badgeCode, true);
                session.SendPacket(new ReceiveBadgeComposer(badgeCode));
            }

            room.GetRoomItemHandler().RemoveFurniture(session, present.Id);

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                ItemDao.UpdateBaseItem(dbClient, present.Id, lotData.Id);
            }

            string furniType = present.GetBaseItem().Type.ToString().ToLower();
            present.BaseItem = lotData.Id;
            present.ResetBaseItem();

            bool itemIsInRoom = true;

            if (present.Data.Type == 's')
            {
                if (!room.GetRoomItemHandler().SetFloorItem(session, present, present.X, present.Y, present.Rotation, true, false, true))
                {
                    using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        ItemDao.UpdateResetRoomId(dbClient, present.Id);
                    }

                    itemIsInRoom = false;
                }
            }
            else
            {
                using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    ItemDao.UpdateResetRoomId(dbClient, present.Id);
                }

                itemIsInRoom = false;
            }

            session.SendPacket(new OpenGiftComposer(present.Data, present.ExtraData, present, itemIsInRoom));

            if (!itemIsInRoom)
            {
                session.GetUser().GetInventoryComponent().TryAddItem(present);
            }
        }
    }
}
