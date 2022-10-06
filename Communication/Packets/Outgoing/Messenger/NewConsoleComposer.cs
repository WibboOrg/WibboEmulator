namespace WibboEmulator.Communication.Packets.Outgoing.Messenger;

internal class NewConsoleComposer : ServerPacket
{
    public NewConsoleComposer(int sender, string message, int time = 0)
        : base(ServerPacketHeader.MESSENGER_CHAT)
    {
        this.WriteInteger(sender);
        this.WriteString(message);
        this.WriteInteger(time);
    }
}
