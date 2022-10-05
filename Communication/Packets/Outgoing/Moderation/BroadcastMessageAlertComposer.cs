namespace WibboEmulator.Communication.Packets.Outgoing.Moderation;

internal class BroadcastMessageAlertComposer : ServerPacket
{
    public BroadcastMessageAlertComposer(string Message, string URL = "")
        : base(ServerPacketHeader.GENERIC_ALERT)
    {
        this.WriteString(Message);
        this.WriteString(URL);
    }
}
