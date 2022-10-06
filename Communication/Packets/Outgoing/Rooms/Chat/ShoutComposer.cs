namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Chat;

internal class ShoutComposer : ServerPacket
{
    public ShoutComposer(int virtualId, string message, int color)
        : base(ServerPacketHeader.UNIT_CHAT_SHOUT)
    {
        this.WriteInteger(virtualId);
        this.WriteString(message);
        this.WriteInteger(WibboEnvironment.GetGame().GetChatManager().GetEmotions().GetEmotionsForText(message));
        this.WriteInteger(color);
        this.WriteInteger(0);
        this.WriteInteger(-1);
    }
}
