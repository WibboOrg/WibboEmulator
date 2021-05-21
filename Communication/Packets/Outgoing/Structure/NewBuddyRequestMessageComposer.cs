namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class NewBuddyRequestMessageComposer : ServerPacket
    {
        public NewBuddyRequestMessageComposer()
            : base(ServerPacketHeader.MESSENGER_REQUEST)
        {

        }
    }
}
