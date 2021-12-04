namespace Butterfly.Communication.Packets.Outgoing.Inventory.Purse
{
    internal class HabboActivityPointNotificationComposer : ServerPacket
    {
        public HabboActivityPointNotificationComposer(int Balance, int Notif, int Type = 0)
            : base(ServerPacketHeader.USER_CURRENCY_UPDATE)
        {
            this.WriteInteger(Balance);
            this.WriteInteger(Notif);
            this.WriteInteger(Type);//Type
        }
    }
}
