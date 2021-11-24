namespace Butterfly.Communication.Packets.Outgoing.Messenger
{
    internal class NewBuddyRequestComposer : ServerPacket
    {
        public NewBuddyRequestComposer()
            : base(ServerPacketHeader.MESSENGER_REQUEST)
        {

        }
    }
}
