namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Chat
{
    internal class UserTypingComposer : ServerPacket
    {
        public UserTypingComposer(int VirtualId, bool Typing)
            : base(ServerPacketHeader.UNIT_TYPING)
        {
            WriteInteger(VirtualId);
            WriteInteger(Typing ? 1 : 0);
        }
    }
}
