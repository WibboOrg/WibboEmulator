using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class MoveAvatarKeyboardEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            int targetX = Packet.PopInt();
            int targetY = Packet.PopInt();

            if (targetX > 1 || targetX < -1)
            {
                targetX = 0;
            }

            if (targetY > 1 || targetY < -1)
            {
                targetY = 0;
            }

            if(Session == null || Session.GetUser() == null)
            {
                return;
            }

            Room currentRoom = Session.GetUser().CurrentRoom;
            if (currentRoom == null)
            {
                return;
            }

            RoomUser User = currentRoom.GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);

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
}