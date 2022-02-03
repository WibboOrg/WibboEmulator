namespace Butterfly.Communication.Packets.Outgoing.Moderation
{
    internal class ModeratorSupportTicketResponseComposer : ServerPacket
    {
        public ModeratorSupportTicketResponseComposer(string messageAlert)
            : base(ServerPacketHeader.CFH_REPLY)
        {
            WriteString(messageAlert);
        }
    }
}
