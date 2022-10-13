namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Action;
using WibboEmulator.Games.GameClients;

internal class WhiperGroupEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var room))
        {
            return;
        }

        var roomUserByUserId = room.RoomUserManager.GetRoomUserByUserId(session.GetUser().Id);
        if (roomUserByUserId == null)
        {
            return;
        }

        var name = packet.PopString();

        var roomUserByUserTarget = room.RoomUserManager.GetRoomUserByName(name);
        if (roomUserByUserTarget == null)
        {
            return;
        }

        if (!roomUserByUserId.WhiperGroupUsers.Contains(roomUserByUserTarget.GetUsername()))
        {
            if (roomUserByUserId.WhiperGroupUsers.Count >= 5)
            {
                return;
            }

            roomUserByUserId.WhiperGroupUsers.Add(roomUserByUserTarget.GetUsername());
        }
        else
        {
            _ = roomUserByUserId.WhiperGroupUsers.Remove(roomUserByUserTarget.GetUsername());
        }
    }
}