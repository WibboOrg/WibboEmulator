using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Outgoing.Rooms.Avatar
{
    internal class SleepComposer : ServerPacket
    {
        public SleepComposer(RoomUser User, bool IsSleeping)
            : base(ServerPacketHeader.UNIT_IDLE)
        {
            this.WriteInteger(User.VirtualId);
            this.WriteBoolean(IsSleeping);
        }
    }
}
