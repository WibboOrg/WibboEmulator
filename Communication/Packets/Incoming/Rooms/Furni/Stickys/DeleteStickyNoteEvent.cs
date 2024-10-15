namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Furni.Stickys;

using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms;

internal sealed class DeleteStickyNoteEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!RoomManager.TryGetRoom(session.User.RoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(session, true))
        {
            return;
        }

        var itemId = packet.PopInt();
        var roomItem = room.RoomItemHandling.GetItem(itemId);
        if (roomItem == null || (roomItem.ItemData.InteractionType != InteractionType.POSTIT && roomItem.ItemData.InteractionType != InteractionType.PHOTO))
        {
            return;
        }

        room.RoomItemHandling.RemoveFurniture(session, roomItem.Id);
        using var dbClient = DatabaseManager.Connection;
        ItemDao.DeleteById(dbClient, roomItem.Id);
    }
}
