namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Engine;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.GameClients;

internal sealed class MoveWallItemEvent : IPacketEvent
{
    public double Delay => 200;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.User.CurrentRoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(session))
        {
            return;
        }

        var id = packet.PopInt();
        var str = packet.PopString();

        var roomItem = room.RoomItemHandling.GetItem(id);
        if (roomItem == null)
        {
            return;
        }

        if (room.RoomData.SellPrice > 0)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("roomsell.error.7", session.Langue));
            return;
        }

        var wallCoordinate = ItemWallUtility.WallPositionCheck(":" + str.Split(':')[1]);
        roomItem.WallCoord = wallCoordinate;
        room.RoomItemHandling.UpdateItem(roomItem);

        room.SendPacket(new ItemUpdateComposer(roomItem, room.RoomData.OwnerId));

    }
}
