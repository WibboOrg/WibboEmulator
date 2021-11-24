namespace Butterfly.Communication.Packets.Outgoing.Messenger
{
    internal class FriendListUpdateComposer : ServerPacket
    {
        public FriendListUpdateComposer()
            : base(ServerPacketHeader.MESSENGER_UPDATE)
        {

        }
    }
}
