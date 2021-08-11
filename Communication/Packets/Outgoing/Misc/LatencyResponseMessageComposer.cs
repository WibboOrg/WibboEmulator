namespace Butterfly.Communication.Packets.Outgoing.Misc
{
    internal class LatencyResponseMessageComposer : ServerPacket
    {
        public LatencyResponseMessageComposer()
            : base(ServerPacketHeader.CLIENT_LATENCY)
        {

        }
    }
}
