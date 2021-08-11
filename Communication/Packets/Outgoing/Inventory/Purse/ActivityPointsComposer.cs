namespace Butterfly.Communication.Packets.Outgoing.Inventory.Purse
{
    internal class ActivityPointsComposer : ServerPacket
    {
        public ActivityPointsComposer(int WibboPoints)
            : base(ServerPacketHeader.USER_CURRENCY)
        {
            this.WriteInteger(1);//Count
            {
                this.WriteInteger(105);//Icon
                this.WriteInteger(WibboPoints);
            }
        }
    }
}
