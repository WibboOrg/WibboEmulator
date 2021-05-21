namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class BuddyListMessageComposer : ServerPacket
    {
        public BuddyListMessageComposer()
            : base(ServerPacketHeader.MESSENGER_FRIENDS)
        {

        }
    }
}
