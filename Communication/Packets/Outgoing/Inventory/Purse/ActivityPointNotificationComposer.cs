namespace Wibbo.Communication.Packets.Outgoing.Inventory.Purse
{
    internal class ActivityPointNotificationComposer : ServerPacket
    {
        public ActivityPointNotificationComposer(int Balance, int Notif, int Type = 0)
            : base(ServerPacketHeader.ACTIVITY_POINT_NOTIFICATION)
        {
            this.WriteInteger(Balance);
            this.WriteInteger(Notif);
            this.WriteInteger(Type);//Type
        }
    }
}
