namespace Butterfly.Communication.Packets.Outgoing.Messenger
{
    internal class BuddyListComposer : ServerPacket
    {
        public BuddyListComposer()
            : base(ServerPacketHeader.MESSENGER_FRIENDS)
        {

        }
    }
}
