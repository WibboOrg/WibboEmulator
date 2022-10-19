namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Avatar;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms.PathFinding;

internal class LookToEvent : IPacketEvent
{
    public double Delay => 50;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.User.CurrentRoomId, out var room))
        {
            return;
        }

        var user = room.RoomUserManager.GetRoomUserByUserId(session.User.Id);
        if (user == null || user.RidingHorse)
        {
            return;
        }

        user.Unidle();
        var x2 = packet.PopInt();
        var y2 = packet.PopInt();
        if (x2 == user.X && y2 == user.Y)
        {
            if (user.SetStep)
            {
                var rotation = Rotation.RotationInverse(user.RotBody);
                user.SetRot(rotation, false, true);
            }
            return;
        }

        var rotation2 = Rotation.Calculate(user.X, user.Y, x2, y2);
        user.SetRot(rotation2, false);
    }
}
