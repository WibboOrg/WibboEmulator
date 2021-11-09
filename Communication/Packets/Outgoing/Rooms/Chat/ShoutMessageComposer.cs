namespace Butterfly.Communication.Packets.Outgoing.Rooms.Chat
{
    internal class ShoutMessageComposer : ServerPacket
    {
        public ShoutMessageComposer(int VirtualId, string Message, int Emotion, int Colour)
            : base(ServerPacketHeader.UNIT_CHAT_SHOUT)
        {
            WriteInteger(VirtualId);
            WriteString(Message);
            WriteInteger(Emotion);
            WriteInteger(Colour);
            WriteInteger(0);
            WriteInteger(-1);
        }
    }
}
