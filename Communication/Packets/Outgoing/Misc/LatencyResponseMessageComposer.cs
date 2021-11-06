namespace Butterfly.Communication.Packets.Outgoing.Misc
{
    internal class LatencyResponseMessageComposer : ServerPacket
    {
        public LatencyResponseMessageComposer(int TestReponse)
            : base(ServerPacketHeader.CLIENT_LATENCY)
        {
            WriteInteger(TestReponse);

        }
    }
}
