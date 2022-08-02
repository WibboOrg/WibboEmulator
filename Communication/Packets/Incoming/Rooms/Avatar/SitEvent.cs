using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
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

            if (roomUserByUserId.ContainStatus("sit") || roomUserByUserId.ContainStatus("lay"))
            {
                return;
            }

            if (roomUserByUserId.RotBody % 2 == 0)            {                roomUserByUserId.SetStatus("sit", "0.5");                roomUserByUserId.IsSit = true;                roomUserByUserId.UpdateNeeded = true;            }
        }
    }
}
