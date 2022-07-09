namespace Wibbo.Communication.Packets.Outgoing.Inventory.Purse
{
    internal class ActivityPointsComposer : ServerPacket
    {
        public ActivityPointsComposer(int WibboPoints, int LimitCoins = 0)
            : base(ServerPacketHeader.USER_CURRENCY)
        {
            this.WriteInteger(2);//Count
            {
                this.WriteInteger(105);//Icon
                this.WriteInteger(WibboPoints);

                this.WriteInteger(55);//Icon
                this.WriteInteger(LimitCoins);
            }
        }
    }
}
