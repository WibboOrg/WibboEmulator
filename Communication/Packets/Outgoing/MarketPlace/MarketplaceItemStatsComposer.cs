namespace WibboEmulator.Communication.Packets.Outgoing.MarketPlace;

internal class MarketplaceItemStatsComposer : ServerPacket
{
    public MarketplaceItemStatsComposer(int ItemId, int SpriteId, int AveragePrice)
        : base(ServerPacketHeader.MARKETPLACE_ITEM_STATS)
    {
        this.WriteInteger(AveragePrice);//Avg price in last 7 days.
        this.WriteInteger(WibboEnvironment.GetGame().GetCatalog().GetMarketplace().OfferCountForSprite(SpriteId));
        this.WriteInteger(7);//Day

        this.WriteInteger(4);//Count
        {
            this.WriteInteger(1); // Jour ?
            this.WriteInteger(2); // Prix moyen
            this.WriteInteger(1); // Volume échange

            this.WriteInteger(1); //x
            this.WriteInteger(2); //?
            this.WriteInteger(2); //y

            this.WriteInteger(3); //x
            this.WriteInteger(5);
            this.WriteInteger(3); //y

            this.WriteInteger(1); //x
            this.WriteInteger(7); //?
            this.WriteInteger(4); //y
        }

        this.WriteInteger(ItemId);
        this.WriteInteger(SpriteId);
    }
}
