namespace WibboEmulator.Communication.Packets.Outgoing.Catalog
{
    internal class CatalogItemDiscountComposer : ServerPacket
    {
        public CatalogItemDiscountComposer()
            : base(ServerPacketHeader.BUNDLE_DISCOUNT_RULESET)
        {
            this.WriteInteger(100);//Most you can get.
            this.WriteInteger(6);
            this.WriteInteger(1);
            this.WriteInteger(1);
            this.WriteInteger(2);//Count
            {
                this.WriteInteger(40);
                this.WriteInteger(99);
            }
        }
    }
}