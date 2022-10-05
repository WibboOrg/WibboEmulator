namespace WibboEmulator.Communication.Packets.Outgoing.Notifications.NotifCustom;

internal class NotifAlertComposer : ServerPacket
{
    public NotifAlertComposer(string Image, string Title, string Message, string TextButton, int RoomId, string Url)
     : base(ServerPacketHeader.NOTIF_ALERT)
    {
        this.WriteString(Image);
        this.WriteString(Title);
        this.WriteString(Message);
        this.WriteString(TextButton);
        this.WriteInteger(RoomId);
        this.WriteString(Url);
    }
}
