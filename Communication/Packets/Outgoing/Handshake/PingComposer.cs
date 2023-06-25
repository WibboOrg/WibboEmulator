namespace WibboEmulator.Communication.Packets.Outgoing.Handshake;

internal sealed class PingComposer : ServerPacket
{
    public PingComposer()
        : base(ServerPacketHeader.CLIENT_PING)
    {

    }
}
