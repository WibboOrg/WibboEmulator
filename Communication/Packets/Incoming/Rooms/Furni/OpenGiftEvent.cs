namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Furni;
using System.Data;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Furni;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Database.Daos.User;
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

        var room = session.User.Room;
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

        if (present.ItemData.InteractionType is InteractionType.GIFT_BANNER or InteractionType.PREMIUM_CLASSIC or InteractionType.PREMIUM_EPIC or InteractionType.PREMIUM_LEGEND)
        {
            present.Interactor.OnTrigger(session, present, -1, true, false);
        }
        else if (present.ItemData.InteractionType == InteractionType.GIFT)
        {
            using var dbClient = DatabaseManager.Connection;

            var itemPresent = ItemPresentDao.GetOne(dbClient, present.Id);

            if (itemPresent == null)
            {
                room.RoomItemHandling.RemoveFurniture(null, present.Id);

                ItemDao.DeleteById(dbClient, present.Id);
                ItemPresentDao.Delete(dbClient, present.Id);

                session.User.InventoryComponent.RemoveItem(present.Id);
                return;
            }

            if (!ItemManager.GetItem(itemPresent.BaseId, out _))
            {
                room.RoomItemHandling.RemoveFurniture(null, present.Id);

                ItemDao.DeleteById(dbClient, present.Id);
                ItemPresentDao.Delete(dbClient, present.Id);

                session.User.InventoryComponent.RemoveItem(present.Id);
                return;
            }

            FinishOpenGift(dbClient, session, present, room, itemPresent);
        }
        else if (present.ItemData.InteractionType == InteractionType.EXTRA_BOX)
        {
            ItemLootBox.OpenExtrabox(session, present, room);
        }
        else if (present.ItemData.InteractionType == InteractionType.DELUXE_BOX)
        {
            ItemLootBox.OpenDeluxeBox(session, present, room);
        }
        else if (present.ItemData.InteractionType == InteractionType.LOOTBOX_2022)
        {
            ItemLootBox.OpenLootBox2022(session, present, room);
        }
        else if (present.ItemData.InteractionType == InteractionType.LEGEND_BOX)
        {
            ItemLootBox.OpenLegendBox(session, present, room);
        }
        else if (present.ItemData.InteractionType == InteractionType.BADGE_BOX)
        {
            ItemLootBox.OpenBadgeBox(session, present, room);
        }
        else if (present.ItemData.InteractionType is InteractionType.CASE_MIEL or InteractionType.CASE_ATHENA or InteractionType.BAG_SAKURA or InteractionType.BAG_ATLANTA or InteractionType.BAG_KYOTO or InteractionType.BAG_NOEL)
        {
            ItemLootBox.OpenCaseOrBag(session, present, room);
        }
    }

    private static void FinishOpenGift(IDbConnection dbClient, GameClient session, Item present, Room room, ItemPresentEntity itemPresent)
    {
        var itemIsInRoom = true;

        room.RoomItemHandling.RemoveFurniture(session, present.Id);

        ItemDao.UpdateBaseItemAndExtraData(dbClient, present.Id, itemPresent.BaseId, itemPresent.ExtraData);

        ItemPresentDao.Delete(dbClient, present.Id);

        present.BaseItemId = itemPresent.BaseId;
        present.ResetBaseItem(room);
        present.ExtraData = !string.IsNullOrEmpty(itemPresent.ExtraData) ? itemPresent.ExtraData : "";

        if (present.Data.Type == ItemType.S)
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
