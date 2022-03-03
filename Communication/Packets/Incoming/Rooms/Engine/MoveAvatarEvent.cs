using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
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

            int pX = Packet.PopInt();
            int pY = Packet.PopInt();

            if (User.ReverseWalk)
            {
                pX = User.SetX + (User.SetX - pX);
                pY = User.SetY + (User.SetY - pY);
            }

            User.MoveTo(pX, pY, (User.AllowOverride || User.TeleportEnabled || User.ReverseWalk));
        }
    }
}
