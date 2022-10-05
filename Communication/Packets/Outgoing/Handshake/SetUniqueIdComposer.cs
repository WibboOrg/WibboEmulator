namespace WibboEmulator.Communication.Packets.Outgoing.Handshake;

internal class SetUniqueIdComposer : ServerPacket
{
    public SetUniqueIdComposer(string Id)
        : base(ServerPacketHeader.SECURITY_MACHINE) => this.WriteString(Id);
}
