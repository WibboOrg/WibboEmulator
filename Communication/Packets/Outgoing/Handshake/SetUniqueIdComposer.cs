namespace WibboEmulator.Communication.Packets.Outgoing.Handshake;

internal class SetUniqueIdComposer : ServerPacket
{
    public SetUniqueIdComposer(string id)
        : base(ServerPacketHeader.SECURITY_MACHINE) => this.WriteString(id);
}
