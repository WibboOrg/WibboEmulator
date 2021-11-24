namespace Butterfly.Communication.Packets.Outgoing.Misc
{
    internal class LatencyResponseComposer : ServerPacket
    {
        public LatencyResponseComposer()
            : base(ServerPacketHeader.CLIENT_LATENCY)
        {

        }
    }
}
