namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Chat;

internal class ShoutComposer : ServerPacket
{
    public ShoutComposer(int VirtualId, string Message, int Color)
        : base(ServerPacketHeader.UNIT_CHAT_SHOUT)
    {
        this.WriteInteger(VirtualId);
        this.WriteString(Message);
        this.WriteInteger(WibboEnvironment.GetGame().GetChatManager().GetEmotions().GetEmotionsForText(Message));
        this.WriteInteger(Color);
        this.WriteInteger(0);
        this.WriteInteger(-1);
    }
}
