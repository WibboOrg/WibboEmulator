namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class ModeratorSupportTicketMessageComposer : ServerPacket
    {
        public ModeratorSupportTicketMessageComposer()
            : base(ServerPacketHeader.ModeratorSupportTicketMessageComposer)
        {

        }
    }
}
