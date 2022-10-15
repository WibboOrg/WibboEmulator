namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Action;
using WibboEmulator.Games.GameClients;

internal class GiveHandItemEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session.User == null)
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.User.CurrentRoomId, out var room))
        {
            return;
        }

        var roomUserByUserId = room.RoomUserManager.GetRoomUserByUserId(session.User.Id);
        if (roomUserByUserId == null || roomUserByUserId.CarryItemID <= 0 || roomUserByUserId.CarryTimer <= 0)
        {
            return;
        }

        var roomUserByUserIdTarget = room.RoomUserManager.GetRoomUserByUserId(packet.PopInt());
        if (roomUserByUserIdTarget == null)
        {
            return;
        }

        if (Math.Abs(roomUserByUserId.X - roomUserByUserIdTarget.X) >= 3 || Math.Abs(roomUserByUserId.Y - roomUserByUserIdTarget.Y) >= 3)
        {
            roomUserByUserId.MoveTo(roomUserByUserIdTarget.X, roomUserByUserIdTarget.Y);
            return;
        }

        roomUserByUserIdTarget.CarryItem(roomUserByUserId.CarryItemID);
        roomUserByUserId.CarryItem(0);
    }
}