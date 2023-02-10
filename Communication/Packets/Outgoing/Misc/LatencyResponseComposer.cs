namespace WibboEmulator.Communication.Packets.Outgoing.Misc;

internal sealed class LatencyResponseComposer : ServerPacket
{
    public LatencyResponseComposer(int testResponse)
        : base(ServerPacketHeader.CLIENT_LATENCY) => this.WriteInteger(testResponse);
}
