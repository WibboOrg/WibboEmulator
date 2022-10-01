using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.PathFinding;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class LookToEvent : IPacketEvent
    {
        public double Delay => 100;

        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetUser().CurrentRoomId, out Room room))
                return;

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