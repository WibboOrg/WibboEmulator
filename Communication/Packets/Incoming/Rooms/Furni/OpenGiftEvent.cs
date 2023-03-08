namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Furni;
using System.Data;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Furni;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms;

internal sealed class OpenGiftEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session == null || session.User == null || !session.User.InRoom)
        {
            return;
        }

        var room = session.User.CurrentRoom;
        if (room == null)
        {
            return;
        }

        var presentId = packet.PopInt();
        var present = room.RoomItemHandling.GetItem(presentId);
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
            using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();

            var data = UserPresentDao.GetOne(dbClient, present.Id);

            if (data == null)
            {
                room.RoomItemHandling.RemoveFurniture(null, present.Id);

                ItemDao.DeleteById(dbClient, present.Id);
                UserPresentDao.Delete(dbClient, present.Id);

                session.User.InventoryComponent.RemoveItem(present.Id);
                return;
            }

            if (!WibboEnvironment.GetGame().GetItemManager().GetItem(Convert.ToInt32(data["base_id"]), out _))
            {
                room.RoomItemHandling.RemoveFurniture(null, present.Id);

                ItemDao.DeleteById(dbClient, present.Id);
                UserPresentDao.Delete(dbClient, present.Id);

                session.User.InventoryComponent.RemoveItem(present.Id);
                return;
            }

            FinishOpenGift(dbClient, session, present, room, data);
        }
        else if (present.GetBaseItem().InteractionType == InteractionType.EXTRA_BOX)
        {
            ItemLootBox.OpenExtrabox(session, present, room);
        }
        else if (present.GetBaseItem().InteractionType == InteractionType.DELUXE_BOX)
        {
            ItemLootBox.OpenDeluxeBox(session, present, room);
        }
        else if (present.GetBaseItem().InteractionType == InteractionType.LOOTBOX_2022)
        {
            ItemLootBox.OpenLootBox2022(session, present, room);
        }
        else if (present.GetBaseItem().InteractionType == InteractionType.LEGEND_BOX)
        {
            ItemLootBox.OpenLegendBox(session, present, room);
        }
        else if (present.GetBaseItem().InteractionType == InteractionType.BADGE_BOX)
        {
            ItemLootBox.OpenBadgeBox(session, present, room);
        }
    }

    private static void FinishOpenGift(IQueryAdapter dbClient, GameClient session, Item present, Room room, DataRow row)
    {
        var itemIsInRoom = true;

        room.RoomItemHandling.RemoveFurniture(session, present.Id);

        ItemDao.UpdateBaseItemAndExtraData(dbClient, present.Id, Convert.ToInt32(row["base_id"]), row["extra_data"].ToString());

        UserPresentDao.Delete(dbClient, present.Id);

        present.BaseItem = Convert.ToInt32(row["base_id"]);
        present.ResetBaseItem();
        present.ExtraData = !string.IsNullOrEmpty(Convert.ToString(row["extra_data"])) ? Convert.ToString(row["extra_data"]) : "";

        if (present.Data.Type == 's')
        {
            if (!room.RoomItemHandling.SetFloorItem(session, present, present.X, present.Y, present.Rotation, true, false, true))
            {
                ItemDao.UpdateResetRoomId(dbClient, present.Id);

                session.User.InventoryComponent.TryAddItem(present);

                itemIsInRoom = false;
            }
        }
        else
        {
            ItemDao.UpdateResetRoomId(dbClient, present.Id);

            session.User.InventoryComponent.TryAddItem(present);

            itemIsInRoom = false;
        }

        session.SendPacket(new OpenGiftComposer(present.Data, present.ExtraData, present, itemIsInRoom));
    }
}
