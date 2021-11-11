namespace Butterfly.Communication.Packets.Outgoing.Moderation
{
    internal class ModeratorSupportTicketResponseComposer : ServerPacket
    {
        public ModeratorSupportTicketResponseComposer(int result)
            : base(ServerPacketHeader.ModeratorSupportTicketResponseComposer)
        {
            WriteInteger(result);
            WriteString("");
        }
    }
}
