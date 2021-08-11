namespace Butterfly.Communication.Packets.Outgoing.Messenger
{
    internal class NewBuddyRequestMessageComposer : ServerPacket
    {
        public NewBuddyRequestMessageComposer()
            : base(ServerPacketHeader.MESSENGER_REQUEST)
        {

        }
    }
}
