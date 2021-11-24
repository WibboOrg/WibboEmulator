namespace Butterfly.Communication.Packets.Outgoing.Messenger
{
    internal class RoomInviteComposer : ServerPacket
    {
        public RoomInviteComposer()
            : base(ServerPacketHeader.MESSENGER_ROOM_INVITE)
        {

        }
    }
}
