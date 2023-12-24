namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Chat;

internal sealed class ChatAudioComposer : ServerPacket
{
    public ChatAudioComposer(int virtualId, string audioUrl, int color)
        : base(ServerPacketHeader.UNIT_CHAT_AUDIO)
    {
        this.WriteInteger(virtualId);
        this.WriteString(audioUrl);
        this.WriteInteger(0);
        this.WriteInteger(color);
        this.WriteInteger(0);
        this.WriteInteger(-1);
    }
}
