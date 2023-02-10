namespace WibboEmulator.Communication.Packets.Outgoing.Notifications;

internal sealed class InClientLinkComposer : ServerPacket
{
    public InClientLinkComposer(string message)
        : base(ServerPacketHeader.IN_CLIENT_LINK) => this.WriteString(message);

}
