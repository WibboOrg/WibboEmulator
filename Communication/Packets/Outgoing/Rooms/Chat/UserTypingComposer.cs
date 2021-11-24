namespace Butterfly.Communication.Packets.Outgoing.Rooms.Chat
{
    internal class UserTypingComposer : ServerPacket
    {
        public UserTypingComposer()
            : base(ServerPacketHeader.UNIT_TYPING)
        {

        }
    }
}
