namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Engine;
using WibboEmulator.Games.GameClients;

internal class MoveAvatarKeyboardEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var targetX = packet.PopInt();
        var targetY = packet.PopInt();

        if (targetX is > 1 or < (-1))
        {
            targetX = 0;
        }

        if (targetY is > 1 or < (-1))
        {
            targetY = 0;
        }

        if (session == null || session.GetUser() == null)
        {
            return;
        }

        var currentRoom = session.GetUser().CurrentRoom;
        if (currentRoom == null)
        {
            return;
        }

        var User = currentRoom.GetRoomUserManager().GetRoomUserByUserId(session.GetUser().Id);

        if (User == null || (!User.CanWalk && !User.TeleportEnabled))
        {
            return;
        }

        if (!User.AllowMoveTo)
        {
            return;
        }

        User.Unidle();

        User.IsWalking = true;

        if (User.ReverseWalk)
        {
            User.GoalX = User.SetX + (-targetX * 1000);
            User.GoalY = User.SetY + (-targetY * 1000);
        }
        else
        {
            User.GoalX = User.SetX + (targetX * 1000);
            User.GoalY = User.SetY + (targetY * 1000);
        }

    }
}