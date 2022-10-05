namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Chat;

internal class WhisperComposer : ServerPacket
{
    public WhisperComposer(int VirtualId, string Text, int Colour)
        : base(ServerPacketHeader.UNIT_CHAT_WHISPER)
    {
        this.WriteInteger(VirtualId);
        this.WriteString(Text);
        this.WriteInteger(WibboEnvironment.GetGame().GetChatManager().GetEmotions().GetEmotionsForText(Text));
        this.WriteInteger(Colour);

        this.WriteInteger(0);
        this.WriteInteger(-1);
    }
}
