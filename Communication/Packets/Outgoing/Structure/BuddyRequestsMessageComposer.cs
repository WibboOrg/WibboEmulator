namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class BuddyRequestsMessageComposer : ServerPacket
    {
        public BuddyRequestsMessageComposer()
            : base(ServerPacketHeader.MESSENGER_REQUESTS)
        {

        }
    }
}
