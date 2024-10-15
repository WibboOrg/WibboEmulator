namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Engine;
using WibboEmulator.Games.GameClients;

internal sealed class MoveAvatarEvent : IPacketEvent
{
    public double Delay => 50;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var room = Session.User.Room;
        if (room == null)
        {
            return;
        }

        var user = room.RoomUserManager.GetRoomUserByUserId(Session.User.ControlUserId == 0 ? Session.User.Id : Session.User.ControlUserId);
        if (user == null || (!user.CanWalk && !user.TeleportEnabled) || !user.AllowMouseMove)
        {
            return;
        }

        var targetX = packet.PopInt();
        var targetY = packet.PopInt();

        if (user.ReverseWalk)
        {
            targetX = user.SetX + (user.SetX - targetX);
            targetY = user.SetY + (user.SetY - targetY);
        }

        user.MoveTo(targetX, targetY, user.AllowOverride || user.TeleportEnabled || user.ReverseWalk);
    }
}
