using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;
using Wibbo.Game.Rooms.PathFinding;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class LookToEvent : IPacketEvent
    {
        public double Delay => 100;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Room room = WibboEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
            if (room == null)
            {
                return;
            }

            RoomUser User = room.GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);
            if (User == null || User.RidingHorse)
            {
                return;
            }

            User.Unidle();
            int X2 = Packet.PopInt();
            int Y2 = Packet.PopInt();
            if (X2 == User.X && Y2 == User.Y)
            {
                if (User.SetStep)
                {
                    int rotation = Rotation.RotationIverse(User.RotBody);
                    User.SetRot(rotation, false, true);
                }
                return;
            }

            int Rotation2 = Rotation.Calculate(User.X, User.Y, X2, Y2);
            User.SetRot(Rotation2, false);
        }
    }
}