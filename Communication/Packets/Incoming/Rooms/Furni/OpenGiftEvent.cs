namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using System.Data;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Furni;
using WibboEmulator.Database.Daos;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms;

internal class OpenGiftEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (session == null || session.GetUser() == null || !session.GetUser().InRoom)
        {
            return;
        }

        var Room = session.GetUser().CurrentRoom;
        if (Room == null)
        {
            return;
        }

        var PresentId = Packet.PopInt();
        var Present = Room.GetRoomItemHandler().GetItem(PresentId);
        if (Present == null)
        {
            return;
        }

        if (!Room.CheckRights(session, true))
        {
            return;
        }

        if (Present.GetBaseItem().InteractionType == InteractionType.GIFT)
        {
            DataRow Data = null;
            using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                Data = UserPresentDao.GetOne(dbClient, Present.Id);
            }

            if (Data == null)
            {
                Room.GetRoomItemHandler().RemoveFurniture(null, Present.Id);

                using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    ItemDao.Delete(dbClient, Present.Id);
                    UserPresentDao.Delete(dbClient, Present.Id);
                }

                session.GetUser().GetInventoryComponent().RemoveItem(Present.Id);
                return;
            }

            if (!WibboEnvironment.GetGame().GetItemManager().GetItem(Convert.ToInt32(Data["base_id"]), out var BaseItem))
            {
                Room.GetRoomItemHandler().RemoveFurniture(null, Present.Id);

                using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    ItemDao.Delete(dbClient, Present.Id);
                    UserPresentDao.Delete(dbClient, Present.Id);
                }

                session.GetUser().GetInventoryComponent().RemoveItem(Present.Id);
                return;
            }

            this.FinishOpenGift(session, BaseItem, Present, Room, Data);
        }
        else if (Present.GetBaseItem().InteractionType == InteractionType.EXTRABOX)
        {
            ItemLootBox.OpenExtrabox(session, Present, Room);
        }
        else if (Present.GetBaseItem().InteractionType == InteractionType.DELUXEBOX)
        {
            ItemLootBox.OpenDeluxeBox(session, Present, Room);
        }
        else if (Present.GetBaseItem().InteractionType == InteractionType.LOOTBOX2022)
        {
            ItemLootBox.OpenLootBox2022(session, Present, Room);
        }
        else if (Present.GetBaseItem().InteractionType == InteractionType.LEGENDBOX)
        {
            ItemLootBox.OpenLegendBox(session, Present, Room);
        }
        else if (Present.GetBaseItem().InteractionType == InteractionType.BADGEBOX)
        {
            ItemLootBox.OpenBadgeBox(session, Present, Room);
        }
    }

    private void FinishOpenGift(GameClient session, ItemData BaseItem, Item Present, Room Room, DataRow Row)
    {
        var ItemIsInRoom = true;

        Room.GetRoomItemHandler().RemoveFurniture(session, Present.Id);

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            ItemDao.UpdateBaseItemAndExtraData(dbClient, Present.Id, Convert.ToInt32(Row["base_id"]), Row["extra_data"].ToString());

            UserPresentDao.Delete(dbClient, Present.Id);
        }

        Present.BaseItem = Convert.ToInt32(Row["base_id"]);
        Present.ResetBaseItem();
        Present.ExtraData = !string.IsNullOrEmpty(Convert.ToString(Row["extra_data"])) ? Convert.ToString(Row["extra_data"]) : "";

        if (Present.Data.Type == 's')
        {
            if (!Room.GetRoomItemHandler().SetFloorItem(session, Present, Present.X, Present.Y, Present.Rotation, true, false, true))
            {
                using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    ItemDao.UpdateResetRoomId(dbClient, Present.Id);
                }

                session.GetUser().GetInventoryComponent().TryAddItem(Present);

                ItemIsInRoom = false;
            }
        }
        else
        {
            using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                ItemDao.UpdateResetRoomId(dbClient, Present.Id);
            }

            session.GetUser().GetInventoryComponent().TryAddItem(Present);

            ItemIsInRoom = false;
        }

        session.SendPacket(new OpenGiftComposer(Present.Data, Present.ExtraData, Present, ItemIsInRoom));
    }
}
