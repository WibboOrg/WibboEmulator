namespace Butterfly.Communication.Packets.Outgoing.Moderation
{
    internal class ModeratorSupportTicketMessageComposer : ServerPacket
    {
        public ModeratorSupportTicketMessageComposer()
            : base(ServerPacketHeader.ModeratorSupportTicketMessageComposer)
        {

        }
    }
}
