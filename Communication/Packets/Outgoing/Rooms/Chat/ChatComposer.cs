namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Chat;

internal sealed class ChatComposer : ServerPacket
{
    public ChatComposer(int virtualId, string message, int color)
        : base(ServerPacketHeader.UNIT_CHAT)
    {
        this.WriteInteger(virtualId);
        this.WriteString(message);
        this.WriteInteger(WibboEnvironment.GetGame().GetChatManager().GetEmotions().GetEmotionsForText(message));
        this.WriteInteger(color);
        this.WriteInteger(0);
        this.WriteInteger(-1);
    }
}
