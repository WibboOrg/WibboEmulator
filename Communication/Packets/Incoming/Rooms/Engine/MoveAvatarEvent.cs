namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Engine;
using WibboEmulator.Games.GameClients;

internal class MoveAvatarEvent : IPacketEvent
{
    public double Delay => 100;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var currentRoom = session.GetUser().CurrentRoom;
        if (currentRoom == null)
        {
            return;
        }

        var user = currentRoom.GetRoomUserManager().GetRoomUserByUserId(session.GetUser().ControlUserId == 0 ? session.GetUser().Id : session.GetUser().ControlUserId);
        if (user == null || (!user.CanWalk && !user.TeleportEnabled))
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
