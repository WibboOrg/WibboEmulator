namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Action;
using WibboEmulator.Games.GameClients;

internal sealed class WhiperGroupEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.User.CurrentRoomId, out var room))
        {
            return;
        }

        var roomUserByUserId = room.RoomUserManager.GetRoomUserByUserId(session.User.Id);
        if (roomUserByUserId == null)
        {
            return;
        }

        var userId = packet.PopInt();

        var roomUserByUserTarget = room.RoomUserManager.GetRoomUserByUserId(userId);
        if (roomUserByUserTarget == null)
        {
            return;
        }

        if (!roomUserByUserId.WhiperGroupUsers.Contains(roomUserByUserTarget.GetUsername()))
        {
            if (roomUserByUserId.WhiperGroupUsers.Count >= 5)
            {
                roomUserByUserId.WhiperGroupUsers.RemoveAt(0);
            }

            roomUserByUserId.WhiperGroupUsers.Add(roomUserByUserTarget.GetUsername());
        }
        else
        {
            _ = roomUserByUserId.WhiperGroupUsers.Remove(roomUserByUserTarget.GetUsername());
        }
    }
}
