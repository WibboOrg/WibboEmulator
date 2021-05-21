namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class FriendListUpdateMessageComposer : ServerPacket
    {
        public FriendListUpdateMessageComposer()
            : base(ServerPacketHeader.MESSENGER_UPDATE)
        {

        }
    }
}
