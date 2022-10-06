namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Engine;
using WibboEmulator.Games.GameClients;

internal class MoveAvatarEvent : IPacketEvent
{
    public double Delay => 100;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        var currentRoom = session.GetUser().CurrentRoom;
        if (currentRoom == null)
        {
            return;
        }

        var User = currentRoom.GetRoomUserManager().GetRoomUserByUserId(session.GetUser().ControlUserId == 0 ? session.GetUser().Id : session.GetUser().ControlUserId);
        if (User == null || !User.CanWalk && !User.TeleportEnabled)
        {
            return;
        }

        var targetX = Packet.PopInt();
        var targetY = Packet.PopInt();

        if (User.ReverseWalk)
        {
            targetX = User.SetX + (User.SetX - targetX);
            targetY = User.SetY + (User.SetY - targetY);
        }

        User.MoveTo(targetX, targetY, User.AllowOverride || User.TeleportEnabled || User.ReverseWalk);
    }
}
