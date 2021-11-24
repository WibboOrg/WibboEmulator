namespace Butterfly.Communication.Packets.Outgoing.Messenger
{
    internal class BuddyRequestsComposer : ServerPacket
    {
        public BuddyRequestsComposer()
            : base(ServerPacketHeader.MESSENGER_REQUESTS)
        {

        }
    }
}
