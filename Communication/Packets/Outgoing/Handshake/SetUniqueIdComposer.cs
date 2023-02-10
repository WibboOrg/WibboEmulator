namespace WibboEmulator.Communication.Packets.Outgoing.Handshake;

internal sealed class SetUniqueIdComposer : ServerPacket
{
    public SetUniqueIdComposer(string id)
        : base(ServerPacketHeader.SECURITY_MACHINE) => this.WriteString(id);
}
