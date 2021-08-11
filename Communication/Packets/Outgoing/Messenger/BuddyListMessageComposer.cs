namespace Butterfly.Communication.Packets.Outgoing.Messenger
{
    internal class BuddyListMessageComposer : ServerPacket
    {
        public BuddyListMessageComposer()
            : base(ServerPacketHeader.MESSENGER_FRIENDS)
        {

        }
    }
}
