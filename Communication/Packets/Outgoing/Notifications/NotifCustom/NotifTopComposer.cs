namespace WibboEmulator.Communication.Packets.Outgoing.Notifications.NotifCustom;

internal class NotifTopComposer : ServerPacket
{
    public NotifTopComposer(string message)
     : base(ServerPacketHeader.NOTIF_TOP) => this.WriteString(message);
}
