namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Avatar;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class DropHandItemEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (!RoomManager.TryGetRoom(Session.User.RoomId, out var room))
        {
            return;
        }

        var roomUserByUserId = room.RoomUserManager.GetRoomUserByUserId(Session.User.Id);
        if (roomUserByUserId == null || roomUserByUserId.CarryItemId <= 0 || roomUserByUserId.CarryTimer <= 0)
        {
            return;
        }

        roomUserByUserId.CarryItem(0);
    }
}
