using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Custom
{
    internal class RpBotChooseEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            string Message = Packet.PopString();

            if (Session == null || Session.GetUser() == null)
            {
                return;
            }

            Room Room = Session.GetUser().CurrentRoom;
            if (Room == null)
            {
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);
            if (User == null)
            {
                return;
            }

            Room.AllowsShous(User, Message);
        }
    }
}