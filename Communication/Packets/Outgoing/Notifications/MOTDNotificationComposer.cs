namespace WibboEmulator.Communication.Packets.Outgoing.Notifications;

internal class MOTDNotificationComposer : ServerPacket
{
    public MOTDNotificationComposer(string message)
        : base(ServerPacketHeader.MOTD_MESSAGES)
    {
        this.WriteInteger(1);
        this.WriteString(message);

    }
}
