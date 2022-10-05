namespace WibboEmulator.Communication.Packets.Outgoing.Moderation;

internal class ModeratorSupportTicketResponseComposer : ServerPacket
{
    public ModeratorSupportTicketResponseComposer(string messageAlert)
        : base(ServerPacketHeader.CFH_REPLY) => this.WriteString(messageAlert);
}
