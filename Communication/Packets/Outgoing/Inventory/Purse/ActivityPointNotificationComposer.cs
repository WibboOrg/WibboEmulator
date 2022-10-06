namespace WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;

internal class ActivityPointNotificationComposer : ServerPacket
{
    public ActivityPointNotificationComposer(int balance, int notif, int type = 0)
        : base(ServerPacketHeader.ACTIVITY_POINT_NOTIFICATION)
    {
        this.WriteInteger(balance);
        this.WriteInteger(notif);
        this.WriteInteger(type);//Type
    }
}
