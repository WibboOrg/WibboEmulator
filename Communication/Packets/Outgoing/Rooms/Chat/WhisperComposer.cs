namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Chat
{
    internal class WhisperComposer : ServerPacket
    {
        public WhisperComposer(int VirtualId, string Text, int Colour)
            : base(ServerPacketHeader.UNIT_CHAT_WHISPER)
        {
            WriteInteger(VirtualId);
            WriteString(Text);
            WriteInteger(WibboEnvironment.GetGame().GetChatManager().GetEmotions().GetEmotionsForText(Text));
            WriteInteger(Colour);

            WriteInteger(0);
            WriteInteger(-1);
        }
    }
}
