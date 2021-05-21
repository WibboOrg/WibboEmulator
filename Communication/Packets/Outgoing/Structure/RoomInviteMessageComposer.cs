namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class RoomInviteMessageComposer : ServerPacket
    {
        public RoomInviteMessageComposer()
            : base(ServerPacketHeader.MESSENGER_ROOM_INVITE)
        {

        }
    }
}
