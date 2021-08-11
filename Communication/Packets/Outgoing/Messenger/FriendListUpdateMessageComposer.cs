namespace Butterfly.Communication.Packets.Outgoing.Messenger
{
    internal class FriendListUpdateMessageComposer : ServerPacket
    {
        public FriendListUpdateMessageComposer()
            : base(ServerPacketHeader.MESSENGER_UPDATE)
        {

        }
    }
}
