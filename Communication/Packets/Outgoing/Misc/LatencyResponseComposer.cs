namespace WibboEmulator.Communication.Packets.Outgoing.Misc
{
    internal class LatencyResponseComposer : ServerPacket
    {
        public LatencyResponseComposer(int testResponse)
            : base(ServerPacketHeader.CLIENT_LATENCY)
        {
            this.WriteInteger(testResponse);
        }
    }
}
