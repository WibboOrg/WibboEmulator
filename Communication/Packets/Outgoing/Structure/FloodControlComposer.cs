namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class FloodControlComposer : ServerPacket
    {
        public FloodControlComposer(int FloodTime)
            : base(ServerPacketHeader.UNIT_FLOOD_CONTROL)
        {
            this.WriteInteger(FloodTime);
        }
    }
}
