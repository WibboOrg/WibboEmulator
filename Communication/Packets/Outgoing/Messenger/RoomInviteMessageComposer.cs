namespace Butterfly.Communication.Packets.Outgoing.Messenger
{
    internal class RoomInviteMessageComposer : ServerPacket
    {
        public RoomInviteMessageComposer()
            : base(ServerPacketHeader.MESSENGER_ROOM_INVITE)
        {

        }
    }
}
