namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Games.GameClients;

internal class GiveHandItemEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (session.GetUser() == null)
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var room))
        {
            return;
        }

        var roomUserByUserId = room.GetRoomUserManager().GetRoomUserByUserId(session.GetUser().Id);
        if (roomUserByUserId == null || roomUserByUserId.CarryItemID <= 0 || roomUserByUserId.CarryTimer <= 0)
        {
            return;
        }

        var roomUserByUserIdTarget = room.GetRoomUserManager().GetRoomUserByUserId(Packet.PopInt());
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