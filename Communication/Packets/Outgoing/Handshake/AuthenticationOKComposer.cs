namespace WibboEmulator.Communication.Packets.Outgoing.Handshake;

internal sealed class AuthenticationOKComposer : ServerPacket
{
    public AuthenticationOKComposer()
        : base(ServerPacketHeader.AUTHENTICATED)
    {

    }
}
