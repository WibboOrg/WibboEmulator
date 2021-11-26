namespace Butterfly.Communication.Packets.Outgoing.Rooms.Chat
{
    internal class UserTypingComposer : ServerPacket
    {
        public UserTypingComposer(int virtualId, int type)
            : base(ServerPacketHeader.UNIT_TYPING)
        {
            WriteInteger(virtualId);
            WriteInteger(type);
        }
    }
}
