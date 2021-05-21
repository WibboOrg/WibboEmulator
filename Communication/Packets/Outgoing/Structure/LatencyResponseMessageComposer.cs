namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class LatencyResponseMessageComposer : ServerPacket
    {
        public LatencyResponseMessageComposer()
            : base(ServerPacketHeader.CLIENT_LATENCY)
        {

        }
    }
}
