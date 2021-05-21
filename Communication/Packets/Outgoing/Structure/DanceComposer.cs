using Butterfly.HabboHotel.Rooms;

namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class DanceComposer : ServerPacket
    {
        public DanceComposer(RoomUser Avatar, int Dance)
            : base(ServerPacketHeader.UNIT_DANCE)
        {
            this.WriteInteger(Avatar.VirtualId);
            this.WriteInteger(Dance);
        }
    }
}
