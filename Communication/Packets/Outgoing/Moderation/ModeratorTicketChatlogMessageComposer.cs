namespace Butterfly.Communication.Packets.Outgoing.Moderation
{
    internal class ModeratorTicketChatlogMessageComposer : ServerPacket
    {
        public ModeratorTicketChatlogMessageComposer()
            : base(ServerPacketHeader.ModeratorTicketChatlogMessageComposer)
        {

        }
    }
}
