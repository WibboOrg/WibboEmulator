namespace Butterfly.Communication.Packets.Outgoing.Rooms.Chat
{
    internal class UserTypingMessageComposer : ServerPacket
    {
        public UserTypingMessageComposer(int VirtualId, bool Typing)
            : base(ServerPacketHeader.UNIT_TYPING)
        {
            WriteInteger(VirtualId);
            WriteInteger(Typing ? 1 : 0);
        }
    }
}
