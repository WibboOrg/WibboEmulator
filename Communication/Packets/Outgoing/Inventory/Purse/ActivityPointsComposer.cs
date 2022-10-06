namespace WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;

internal class ActivityPointsComposer : ServerPacket
{
    public ActivityPointsComposer(int wibboPoints, int limitCoins = 0)
        : base(ServerPacketHeader.USER_CURRENCY)
    {
        this.WriteInteger(2);//Count
        {
            this.WriteInteger(105);//Icon
            this.WriteInteger(wibboPoints);

            this.WriteInteger(55);//Icon
            this.WriteInteger(limitCoins);
        }
    }
}
