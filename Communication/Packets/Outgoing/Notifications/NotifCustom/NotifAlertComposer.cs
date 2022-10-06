namespace WibboEmulator.Communication.Packets.Outgoing.Notifications.NotifCustom;

internal class NotifAlertComposer : ServerPacket
{
    public NotifAlertComposer(string image, string title, string message, string textButton, int roomId, string url)
     : base(ServerPacketHeader.NOTIF_ALERT)
    {
        this.WriteString(image);
        this.WriteString(title);
        this.WriteString(message);
        this.WriteString(textButton);
        this.WriteInteger(roomId);
        this.WriteString(url);
    }
}
