namespace Butterfly.Communication.Packets.Outgoing.Rooms.Chat
{
    internal class WhisperMessageComposer : ServerPacket
    {
        public WhisperMessageComposer(int VirtualId, string Text, int Emotion, int Colour)
            : base(ServerPacketHeader.UNIT_CHAT_WHISPER)
        {
            WriteInteger(VirtualId);
            WriteString(Text);
            WriteInteger(Emotion);
            WriteInteger(Colour);

            WriteInteger(0);
            WriteInteger(-1);
        }
    }
}
