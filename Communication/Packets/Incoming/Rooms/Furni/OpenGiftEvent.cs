namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Furni;
using System.Data;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Furni;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms;

internal class OpenGiftEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session == null || session.GetUser() == null || !session.GetUser().InRoom)
        {
            return;
        }

        var room = session.GetUser().CurrentRoom;
        if (room == null)
        {
            return;
        }

        var presentId = packet.PopInt();
        var present = room.GetRoomItemHandler().GetItem(presentId);
        if (present == null)
        {
            return;
        }

        if (!room.CheckRights(session, true))
        {
            return;
        }

        if (present.GetBaseItem().InteractionType == InteractionType.GIFT)
        {
            DataRow data = null;
            using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                data = UserPresentDao.GetOne(dbClient, present.Id);
            }

            if (data == null)
            {
                room.GetRoomItemHandler().RemoveFurniture(null, present.Id);

                using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    ItemDao.Delete(dbClient, present.Id);
                    UserPresentDao.Delete(dbClient, present.Id);
                }

                session.GetUser().GetInventoryComponent().RemoveItem(present.Id);
                return;
            }

            if (!WibboEnvironment.GetGame().GetItemManager().GetItem(Convert.ToInt32(data["base_id"]), out _))
            {
                room.GetRoomItemHandler().RemoveFurniture(null, present.Id);

                using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    ItemDao.Delete(dbClient, present.Id);
                    UserPresentDao.Delete(dbClient, present.Id);
                }

                session.GetUser().GetInventoryComponent().RemoveItem(present.Id);
                return;
            }

            FinishOpenGift(session, present, room, data);
        }
        else if (present.GetBaseItem().InteractionType == InteractionType.EXTRABOX)
        {
            ItemLootBox.OpenExtrabox(session, present, room);
        }
        else if (present.GetBaseItem().InteractionType == InteractionType.DELUXEBOX)
        {
            ItemLootBox.OpenDeluxeBox(session, present, room);
        }
        else if (present.GetBaseItem().InteractionType == InteractionType.LOOTBOX2022)
        {
            ItemLootBox.OpenLootBox2022(session, present, room);
        }
        else if (present.GetBaseItem().InteractionType == InteractionType.LEGENDBOX)
        {
            ItemLootBox.OpenLegendBox(session, present, room);
        }
        else if (present.GetBaseItem().InteractionType == InteractionType.BADGEBOX)
        {
            ItemLootBox.OpenBadgeBox(session, present, room);
        }
    }

    private static void FinishOpenGift(GameClient session, Item present, Room room, DataRow row)
    {
        var itemIsInRoom = true;

        room.GetRoomItemHandler().RemoveFurniture(session, present.Id);

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            ItemDao.UpdateBaseItemAndExtraData(dbClient, present.Id, Convert.ToInt32(row["base_id"]), row["extra_data"].ToString());

            UserPresentDao.Delete(dbClient, present.Id);
        }

        present.BaseItem = Convert.ToInt32(row["base_id"]);
        present.ResetBaseItem();
        present.ExtraData = !string.IsNullOrEmpty(Convert.ToString(row["extra_data"])) ? Convert.ToString(row["extra_data"]) : "";

        if (present.Data.Type == 's')
        {
            if (!room.GetRoomItemHandler().SetFloorItem(session, present, present.X, present.Y, present.Rotation, true, false, true))
            {
                using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    ItemDao.UpdateResetRoomId(dbClient, present.Id);
                }

                _ = session.GetUser().GetInventoryComponent().TryAddItem(present);

                itemIsInRoom = false;
            }
        }
        else
        {
            using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                ItemDao.UpdateResetRoomId(dbClient, present.Id);
            }

            _ = session.GetUser().GetInventoryComponent().TryAddItem(present);

            itemIsInRoom = false;
        }

        session.SendPacket(new OpenGiftComposer(present.Data, present.ExtraData, present, itemIsInRoom));
    }
}
