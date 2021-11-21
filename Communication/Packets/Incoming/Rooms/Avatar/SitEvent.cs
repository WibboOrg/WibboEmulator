using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class SitEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            Room room = Session.GetHabbo().CurrentRoom;            if (room == null)
            {
                return;
            }

            RoomUser roomUserByHabbo = room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);            if (roomUserByHabbo == null)
            {
                return;
            }

            if (roomUserByHabbo.Statusses.ContainsKey("sit") || roomUserByHabbo.Statusses.ContainsKey("lay"))
            {
                return;
            }

            if (roomUserByHabbo.RotBody % 2 == 0)            {                roomUserByHabbo.SetStatus("sit", "0.5");                roomUserByHabbo.IsSit = true;                roomUserByHabbo.UpdateNeeded = true;            }
        }
    }
}
