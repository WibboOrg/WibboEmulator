namespace WibboEmulator.Communication.Packets.Outgoing.WibboTool;

internal class AddChatlogsComposer : ServerPacket
{
    public AddChatlogsComposer(int userId, string userName, string message)
      : base(ServerPacketHeader.ADD_CHATLOGS)
    {
        this.WriteInteger(userId);
        this.WriteString(userName);
        this.WriteString(message);
    }
}
