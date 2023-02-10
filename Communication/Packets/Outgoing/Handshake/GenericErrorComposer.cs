namespace WibboEmulator.Communication.Packets.Outgoing.Handshake;

internal sealed class GenericErrorComposer : ServerPacket
{
    public GenericErrorComposer(int errorId)
        : base(ServerPacketHeader.GENERIC_ERROR) => this.WriteInteger(errorId);
}
