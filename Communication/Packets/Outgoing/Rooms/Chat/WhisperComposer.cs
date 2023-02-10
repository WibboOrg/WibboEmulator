namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Chat;

internal sealed class WhisperComposer : ServerPacket
{
    public WhisperComposer(int virtualId, string text, int colour)
        : base(ServerPacketHeader.UNIT_CHAT_WHISPER)
    {
        this.WriteInteger(virtualId);
        this.WriteString(text);
        this.WriteInteger(WibboEnvironment.GetGame().GetChatManager().GetEmotions().GetEmotionsForText(text));
        this.WriteInteger(colour);

        this.WriteInteger(0);
        this.WriteInteger(-1);
    }
}
