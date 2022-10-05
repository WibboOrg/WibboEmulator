using System.Data;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Furni;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class OpenGiftEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetUser() == null || !Session.GetUser().InRoom)
            {
                return;
            }

            Room Room = Session.GetUser().CurrentRoom;
            if (Room == null)
            {
                return;
            }

            int PresentId = Packet.PopInt();
            Item Present = Room.GetRoomItemHandler().GetItem(PresentId);
            if (Present == null)
            {
                return;
            }

            if (!Room.CheckRights(Session, true))
            {
                return;
            }

            if (Present.GetBaseItem().InteractionType == InteractionType.GIFT)
            {
                DataRow Data = null;
                using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    Data = UserPresentDao.GetOne(dbClient, Present.Id);
                }

                if (Data == null)
                {
                    Room.GetRoomItemHandler().RemoveFurniture(null, Present.Id);

                    using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        ItemDao.Delete(dbClient, Present.Id);
                        UserPresentDao.Delete(dbClient, Present.Id);
                    }

                    Session.GetUser().GetInventoryComponent().RemoveItem(Present.Id);
                    return;
                }

                if (!WibboEnvironment.GetGame().GetItemManager().GetItem(Convert.ToInt32(Data["base_id"]), out ItemData BaseItem))
                {
                    Room.GetRoomItemHandler().RemoveFurniture(null, Present.Id);

                    using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        ItemDao.Delete(dbClient, Present.Id);
                        UserPresentDao.Delete(dbClient, Present.Id);
                    }

                    Session.GetUser().GetInventoryComponent().RemoveItem(Present.Id);
                    return;
                }

                this.FinishOpenGift(Session, BaseItem, Present, Room, Data);
            }
            else if (Present.GetBaseItem().InteractionType == InteractionType.EXTRABOX)
            {
                ItemLootBox.OpenExtrabox(Session, Present, Room);
            }
            else if (Present.GetBaseItem().InteractionType == InteractionType.DELUXEBOX)
            {
                ItemLootBox.OpenDeluxeBox(Session, Present, Room);
            }
            else if (Present.GetBaseItem().InteractionType == InteractionType.LOOTBOX2022)
            {
                ItemLootBox.OpenLootBox2022(Session, Present, Room);
            }
            else if (Present.GetBaseItem().InteractionType == InteractionType.LEGENDBOX)
            {
                ItemLootBox.OpenLegendBox(Session, Present, Room);
            }
            else if (Present.GetBaseItem().InteractionType == InteractionType.BADGEBOX)
            {
                ItemLootBox.OpenBadgeBox(Session, Present, Room);
            }
        }

        private void FinishOpenGift(GameClient Session, ItemData BaseItem, Item Present, Room Room, DataRow Row)
        {
            bool ItemIsInRoom = true;

            Room.GetRoomItemHandler().RemoveFurniture(Session, Present.Id);

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                ItemDao.UpdateBaseItemAndExtraData(dbClient, Present.Id, Convert.ToInt32(Row["base_id"]), Row["extra_data"].ToString());

                UserPresentDao.Delete(dbClient, Present.Id);
            }

            Present.BaseItem = Convert.ToInt32(Row["base_id"]);
            Present.ResetBaseItem();
            Present.ExtraData = (!string.IsNullOrEmpty(Convert.ToString(Row["extra_data"])) ? Convert.ToString(Row["extra_data"]) : "");

            if (Present.Data.Type == 's')
            {
                if (!Room.GetRoomItemHandler().SetFloorItem(Session, Present, Present.X, Present.Y, Present.Rotation, true, false, true))
                {
                    using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        ItemDao.UpdateResetRoomId(dbClient, Present.Id);
                    }

                    Session.GetUser().GetInventoryComponent().TryAddItem(Present);

                    ItemIsInRoom = false;
                }
            }
            else
            {
                using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    ItemDao.UpdateResetRoomId(dbClient, Present.Id);
                }

                Session.GetUser().GetInventoryComponent().TryAddItem(Present);

                ItemIsInRoom = false;
            }

            Session.SendPacket(new OpenGiftComposer(Present.Data, Present.ExtraData, Present, ItemIsInRoom));
        }
    }
}
