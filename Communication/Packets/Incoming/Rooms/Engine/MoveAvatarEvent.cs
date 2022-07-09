using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class MoveAvatarEvent : IPacketEvent
    {
        public double Delay => 100;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Room currentRoom = Session.GetUser().CurrentRoom;
            if (currentRoom == null)
            {
                return;
            }

            RoomUser User = currentRoom.GetRoomUserManager().GetRoomUserByUserId((Session.GetUser().ControlUserId == 0) ? Session.GetUser().Id : Session.GetUser().ControlUserId);
            if (User == null || (!User.CanWalk && !User.TeleportEnabled))
            {
                return;
            }

            int targetX = Packet.PopInt();
            int targetY = Packet.PopInt();

            if (User.ReverseWalk)
            {
                targetX = User.SetX + (User.SetX - targetX);
                targetY = User.SetY + (User.SetY - targetY);
            }

            User.MoveTo(targetX, targetY, (User.AllowOverride || User.TeleportEnabled || User.ReverseWalk));
        }
    }
}
