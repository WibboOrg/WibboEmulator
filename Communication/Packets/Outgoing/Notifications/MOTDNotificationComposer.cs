namespace WibboEmulator.Communication.Packets.Outgoing.Notifications;

internal class MOTDNotificationComposer : ServerPacket
{
    public MOTDNotificationComposer(string Message)
        : base(ServerPacketHeader.MOTD_MESSAGES)
    {
        this.WriteInteger(1);
        this.WriteString(Message);

    }
}
