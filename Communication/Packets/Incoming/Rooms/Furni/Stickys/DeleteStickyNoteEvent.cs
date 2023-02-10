namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Furni.Stickys;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;

internal sealed class DeleteStickyNoteEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.User.CurrentRoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(session, true))
        {
            return;
        }

        var itemId = packet.PopInt();
        var roomItem = room.RoomItemHandling.GetItem(itemId);
        if (roomItem == null || (roomItem.GetBaseItem().InteractionType != InteractionType.POSTIT && roomItem.GetBaseItem().InteractionType != InteractionType.PHOTO))
        {
            return;
        }

        room.RoomItemHandling.RemoveFurniture(session, roomItem.Id);
        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        ItemDao.Delete(dbClient, roomItem.Id);
    }
}
