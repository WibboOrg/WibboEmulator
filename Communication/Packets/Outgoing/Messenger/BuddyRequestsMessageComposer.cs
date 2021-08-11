namespace Butterfly.Communication.Packets.Outgoing.Messenger
{
    internal class BuddyRequestsMessageComposer : ServerPacket
    {
        public BuddyRequestsMessageComposer()
            : base(ServerPacketHeader.MESSENGER_REQUESTS)
        {

        }
    }
}
