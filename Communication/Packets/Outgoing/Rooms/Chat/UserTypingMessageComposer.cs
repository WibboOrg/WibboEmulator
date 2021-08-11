namespace Butterfly.Communication.Packets.Outgoing.Rooms.Chat
{
    internal class UserTypingMessageComposer : ServerPacket
    {
        public UserTypingMessageComposer()
            : base(ServerPacketHeader.UNIT_TYPING)
        {

        }
    }
}
