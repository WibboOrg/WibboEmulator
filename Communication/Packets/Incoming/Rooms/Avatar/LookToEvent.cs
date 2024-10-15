namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Avatar;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.PathFinding;

internal sealed class LookToEvent : IPacketEvent
{
    public double Delay => 50;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!RoomManager.TryGetRoom(session.User.RoomId, out var room))
        {
            return;
        }

        var targetX = packet.PopInt();
        var targetY = packet.PopInt();
        var targetUserId = packet.PopInt();

        var user = room.RoomUserManager.GetRoomUserByUserId(session.User.Id);
        if (user == null)
        {
            return;
        }

        var userTarget = room.RoomUserManager.GetRoomUserByVirtualId(targetUserId);
        if (userTarget == null)
        {
            return;
        }

        if (user != userTarget)
        {
            room.UserClick(user, userTarget);
        }

        if (user.RidingHorse)
        {
            return;
        }

        user.Unidle();
        if (targetX == user.X && targetY == user.Y)
        {
            if (user.SetStep)
            {
                var rotation = Rotation.RotationInverse(user.RotBody);
                user.SetRot(rotation, false, true);
            }
            return;
        }

        var rotation2 = Rotation.Calculate(user.X, user.Y, targetX, targetY);
        user.SetRot(rotation2, false);
    }
}
