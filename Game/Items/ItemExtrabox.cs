using Butterfly.Communication.Packets.Outgoing.Inventory.Achievements;
using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Communication.Packets.Outgoing.Rooms.Furni;
using Butterfly.Communication.Packets.Outgoing.Users;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Catalog;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Butterfly.Game.Items
{
    internal static class ItemExtrabox
    {
        public static void OpenExtrabox(Client Session, Item Present, Rooms.Room Room)
        {
            int PageId;

            if (ButterflyEnvironment.GetRandomNumber(1, 750) == 750) //Ultra rare
            {
                PageId = 84641;
            }
            else if (ButterflyEnvironment.GetRandomNumber(1, 100) == 100) //Extra rare
            {
                PageId = 98747;
            }
            else if (ButterflyEnvironment.GetRandomNumber(1, 75) == 75) //WibboPoint
            {
                PageId = 15987;
            }
            else if (ButterflyEnvironment.GetRandomNumber(1, 25) == 25) //Pièce Win-win
            {
                PageId = 456465;
            }
            else
            {
                PageId = 894948; //Rare
            }

            ButterflyEnvironment.GetGame().GetCatalog().TryGetPage(PageId, out CatalogPage Page);
            if (Page == null)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.error", Session.Langue));
                return;
            }

            ItemData LotData = Page.Items.ElementAt(ButterflyEnvironment.GetRandomNumber(0, Page.Items.Count - 1)).Value.Data;
            if (LotData == null)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.error", Session.Langue));
                return;
            }

            Room.GetRoomItemHandler().RemoveFurniture(Session, Present.Id);

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                ItemDao.UpdateBaseItem(dbClient, Present.Id, LotData.Id);
            }

            string FurniType = Present.GetBaseItem().Type.ToString().ToLower();
            Present.BaseItem = LotData.Id;
            Present.ResetBaseItem();

            bool ItemIsInRoom = true;

            if (Present.Data.Type == 's')
            {
                if (!Room.GetRoomItemHandler().SetFloorItem(Session, Present, Present.GetX, Present.GetY, Present.Rotation, true, false, true))
                {
                    using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        ItemDao.UpdateResetRoomId(dbClient, Present.Id);
                    }

                    ItemIsInRoom = false;
                }
            }
            else
            {
                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    ItemDao.UpdateResetRoomId(dbClient, Present.Id);
                }

                ItemIsInRoom = false;
            }

            Session.SendPacket(new OpenGiftComposer(Present.Data, Present.ExtraData, Present, ItemIsInRoom));

            if (!ItemIsInRoom)
            {
                Session.GetHabbo().GetInventoryComponent().TryAddItem(Present);
            }
        }

        public static void OpenDeluxeBox(Client Session, Item Present, Rooms.Room Room)
        {
            int PageId;

            if (ButterflyEnvironment.GetRandomNumber(1, 200) == 200) //Epique
            {
                PageId = 1635463617;
            }
            else if (ButterflyEnvironment.GetRandomNumber(1, 20) == 20) //Commun
            {
                PageId = 1635463616;
            }
            else
            {
                PageId = 91700214; //Basique
            }

            ButterflyEnvironment.GetGame().GetCatalog().TryGetPage(PageId, out CatalogPage Page);
            if (Page == null)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.error", Session.Langue));
                return;
            }

            ItemData LotData = Page.Items.ElementAt(ButterflyEnvironment.GetRandomNumber(0, Page.Items.Count - 1)).Value.Data;
            if (LotData == null)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.error", Session.Langue));
                return;
            }

            Room.GetRoomItemHandler().RemoveFurniture(Session, Present.Id);

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                ItemDao.UpdateBaseItem(dbClient, Present.Id, LotData.Id);
            }

            string FurniType = Present.GetBaseItem().Type.ToString().ToLower();
            Present.BaseItem = LotData.Id;
            Present.ResetBaseItem();

            bool ItemIsInRoom = true;

            if (Present.Data.Type == 's')
            {
                if (!Room.GetRoomItemHandler().SetFloorItem(Session, Present, Present.GetX, Present.GetY, Present.Rotation, true, false, true))
                {
                    using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        ItemDao.UpdateResetRoomId(dbClient, Present.Id);
                    }

                    ItemIsInRoom = false;
                }
            }
            else
            {
                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    ItemDao.UpdateResetRoomId(dbClient, Present.Id);
                }

                ItemIsInRoom = false;
            }

            Session.SendPacket(new OpenGiftComposer(Present.Data, Present.ExtraData, Present, ItemIsInRoom));

            if (!ItemIsInRoom)
            {
                Session.GetHabbo().GetInventoryComponent().TryAddItem(Present);
            }
        }

        public static void OpenBadgeBox(Client Session, Item Present, Rooms.Room Room)
        {
            int PageId = 0;
            string BadgeCode = "";

            //Présentoir et badge
            PageId = 987987;

            List<int> PageBadgeList = new List<int>(new int[] { 8948, 18171, 18172, 18173, 18174, 18175, 18176, 18177, 18178, 18179, 18180, 18181, 18182, 18183 });
            int PageBadgeId = PageBadgeList[ButterflyEnvironment.GetRandomNumber(0, PageBadgeList.Count - 1)];
            ButterflyEnvironment.GetGame().GetCatalog().TryGetPage(PageBadgeId, out CatalogPage PageBadge);
            if (PageBadge == null)
            {
                return;
            }

            BadgeCode = PageBadge.Items.ElementAt(ButterflyEnvironment.GetRandomNumber(0, PageBadge.Items.Count - 1)).Value.Badge;


            ButterflyEnvironment.GetGame().GetCatalog().TryGetPage(PageId, out CatalogPage Page);
            if (Page == null)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.error", Session.Langue));
                return;
            }

            ItemData LotData = Page.Items.ElementAt(ButterflyEnvironment.GetRandomNumber(0, Page.Items.Count - 1)).Value.Data;
            if (LotData == null)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.error", Session.Langue));
                return;
            }

            Room.GetRoomItemHandler().RemoveFurniture(Session, Present.Id);

            string ExtraData = BadgeCode + Convert.ToChar(9) + Session.GetHabbo().Username + Convert.ToChar(9) + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                ItemDao.UpdateBaseItemAndExtraData(dbClient, Present.Id, LotData.Id, ExtraData);
            }

            Present.ExtraData = ExtraData;

            Present.BaseItem = LotData.Id;
            Present.ResetBaseItem();

            bool ItemIsInRoom = true;

            if (Present.Data.Type == 's')
            {
                if (!Room.GetRoomItemHandler().SetFloorItem(Session, Present, Present.GetX, Present.GetY, Present.Rotation, true, false, true))
                {
                    using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        ItemDao.UpdateResetRoomId(dbClient, Present.Id);
                    }

                    ItemIsInRoom = false;
                }
            }
            else
            {
                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    ItemDao.UpdateResetRoomId(dbClient, Present.Id);
                }

                ItemIsInRoom = false;
            }

            Session.SendPacket(new OpenGiftComposer(Present.Data, Present.ExtraData, Present, ItemIsInRoom));

            if (!ItemIsInRoom)
            {
                Session.GetHabbo().GetInventoryComponent().TryAddItem(Present);
            }

            if (!string.IsNullOrEmpty(BadgeCode) && !Session.GetHabbo().GetBadgeComponent().HasBadge(BadgeCode))
            {
                Session.GetHabbo().GetBadgeComponent().GiveBadge(BadgeCode, true);
                Session.SendPacket(new ReceiveBadgeComposer(BadgeCode));

                RoomUser roomUserByHabbo = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
                if (roomUserByHabbo != null)
                {
                    roomUserByHabbo.SendWhisperChat("Tu as reçu le badge: " + BadgeCode);
                }
            }
        }

        public static void OpenLegendBox(Client Session, Item Present, Rooms.Room Room)
        {
            int PageId = 0;
            string BadgeCode = "";
            string LotType = "";
            int ForceItem = 0;

            if (ButterflyEnvironment.GetRandomNumber(1, 100) == 100) //Legendaire
            {
                PageId = 14514;
                LotType = "Légendaire";
            }
            else if (ButterflyEnvironment.GetRandomNumber(1, 75) == 75) //Royal
            {
                PageId = 584545;
                LotType = "Royal";
                ForceItem = 37951979;
            }
            else if (ButterflyEnvironment.GetRandomNumber(1, 30) == 30) //Royal
            {
                PageId = 584545;
                LotType = "Royal";
                ForceItem = 70223722;
            }
            else if (ButterflyEnvironment.GetRandomNumber(1, 15) == 15) //Epique
            {
                PageId = 84641;
                LotType = "épique";
            }
            else if (ButterflyEnvironment.GetRandomNumber(1, 5) == 5) //Royal
            {
                PageId = 584545;
                LotType = "Royal";
                ForceItem = 52394359;
            }
            else
            {
                PageId = 98747;
                LotType = "commun";
            }


            int PageBadgeId = 841878;

            if (!ButterflyEnvironment.GetGame().GetCatalog().TryGetPage(PageBadgeId, out CatalogPage PageBadge))
            {
                return;
            }

            foreach (KeyValuePair<int, CatalogItem> Item in PageBadge.Items.OrderBy(a => Guid.NewGuid()).ToList())
            {
                if (Session.GetHabbo().GetBadgeComponent().HasBadge(Item.Value.Badge))
                {
                    continue;
                }

                BadgeCode = Item.Value.Badge;
                break;

            }

            ButterflyEnvironment.GetGame().GetCatalog().TryGetPage(PageId, out CatalogPage Page);
            if (Page == null)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.error", Session.Langue));
                return;
            }

            ItemData LotData = null;

            if (ForceItem == 0)
            {
                LotData = Page.Items.ElementAt(ButterflyEnvironment.GetRandomNumber(0, Page.Items.Count - 1)).Value.Data;
            }
            else
            {
                LotData = Page.GetItem(ForceItem).Data;
            }

            if (LotData == null)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.error", Session.Langue));
                return;
            }

            int Credits = ButterflyEnvironment.GetRandomNumber(100, 10000) * 1000;
            Session.GetHabbo().Credits += Credits;
            Session.GetHabbo().UpdateCreditsBalance();

            int WinWin = ButterflyEnvironment.GetRandomNumber(100, 1000);
            Session.GetHabbo().AchievementPoints += WinWin;

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                UserStatsDao.UpdateAchievementScore(dbClient, Session.GetHabbo().Id, WinWin);
            }

            Session.SendPacket(new AchievementScoreComposer(Session.GetHabbo().AchievementPoints));
            RoomUser roomUserByHabbo = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (roomUserByHabbo != null)
            {
                Session.SendPacket(new UserChangeComposer(roomUserByHabbo, true));
                Room.SendPacket(new UserChangeComposer(roomUserByHabbo, false));
            }

            if (!string.IsNullOrEmpty(BadgeCode))
            {
                Session.GetHabbo().GetBadgeComponent().GiveBadge(BadgeCode, true);
                Session.SendPacket(new ReceiveBadgeComposer(BadgeCode));
            }

            if (roomUserByHabbo != null)
            {
                roomUserByHabbo.SendWhisperChat(string.Format(ButterflyEnvironment.GetLanguageManager().TryGetValue("item.legendboxlot", Session.Langue), Credits, WinWin, BadgeCode, LotType));
            }

            Room.GetRoomItemHandler().RemoveFurniture(Session, Present.Id);

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                ItemDao.UpdateBaseItem(dbClient, Present.Id, LotData.Id);
            }
            string FurniType = Present.GetBaseItem().Type.ToString().ToLower();
            Present.BaseItem = LotData.Id;
            Present.ResetBaseItem();

            bool ItemIsInRoom = true;

            if (Present.Data.Type == 's')
            {
                if (!Room.GetRoomItemHandler().SetFloorItem(Session, Present, Present.GetX, Present.GetY, Present.Rotation, true, false, true))
                {
                    using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        ItemDao.UpdateResetRoomId(dbClient, Present.Id);
                    }

                    ItemIsInRoom = false;
                }
            }
            else
            {
                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    ItemDao.UpdateResetRoomId(dbClient, Present.Id);
                }

                ItemIsInRoom = false;
            }

            Session.SendPacket(new OpenGiftComposer(Present.Data, Present.ExtraData, Present, ItemIsInRoom));

            if (!ItemIsInRoom)
            {
                Session.GetHabbo().GetInventoryComponent().TryAddItem(Present);
            }
        }
    }
}
