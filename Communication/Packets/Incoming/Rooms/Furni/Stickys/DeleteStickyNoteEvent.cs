namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Furni.Stickys;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;

internal class DeleteStickyNoteEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(session, true))
        {
            return;
        }

        var ItemId = Packet.PopInt();
        var roomItem = room.GetRoomItemHandler().GetItem(ItemId);
        if (roomItem == null || roomItem.GetBaseItem().InteractionType != InteractionType.POSTIT && roomItem.GetBaseItem().InteractionType != InteractionType.PHOTO)
        {
            return;
        }

        room.GetRoomItemHandler().RemoveFurniture(session, roomItem.Id);
        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        ItemDao.Delete(dbClient, roomItem.Id);
    }
}