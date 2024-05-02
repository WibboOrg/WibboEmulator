namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Chat;

using WibboEmulator.Games.Chats.Emotions;

internal sealed class ShoutComposer : ServerPacket
{
    public ShoutComposer(int virtualId, string message, int color)
        : base(ServerPacketHeader.UNIT_CHAT_SHOUT)
    {
        this.WriteInteger(virtualId);
        this.WriteString(message);
        this.WriteInteger(ChatEmotionsManager.GetEmotionsForText(message));
        this.WriteInteger(color);
        this.WriteInteger(0);
        this.WriteInteger(-1);
    }
}
