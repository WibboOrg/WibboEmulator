using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class SitEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Room room = Session.GetUser().CurrentRoom;            if (room == null)
            {
                return;
            }

            RoomUser roomUserByUserId = room.GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);            if (roomUserByUserId == null)
            {
                return;
            }

            if (roomUserByUserId.Statusses.ContainsKey("sit") || roomUserByUserId.Statusses.ContainsKey("lay"))
            {
                return;
            }

            if (roomUserByUserId.RotBody % 2 == 0)            {                roomUserByUserId.SetStatus("sit", "0.5");                roomUserByUserId.IsSit = true;                roomUserByUserId.UpdateNeeded = true;            }
        }
    }
}
