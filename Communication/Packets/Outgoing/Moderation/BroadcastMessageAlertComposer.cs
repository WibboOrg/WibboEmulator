namespace WibboEmulator.Communication.Packets.Outgoing.Moderation;

internal class BroadcastMessageAlertComposer : ServerPacket
{
    public BroadcastMessageAlertComposer(string message, string url = "")
        : base(ServerPacketHeader.GENERIC_ALERT)
    {
        this.WriteString(message);
        this.WriteString(url);
    }
}
