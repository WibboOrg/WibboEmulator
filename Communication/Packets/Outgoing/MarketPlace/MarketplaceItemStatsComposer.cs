namespace WibboEmulator.Communication.Packets.Outgoing.MarketPlace;

internal sealed class MarketplaceItemStatsComposer : ServerPacket
{
    public MarketplaceItemStatsComposer(int itemId, int spriteId, int averagePrice)
        : base(ServerPacketHeader.MARKETPLACE_ITEM_STATS)
    {
        this.WriteInteger(averagePrice);//Avg price in last 7 days.
        this.WriteInteger(WibboEnvironment.GetGame().GetCatalog().GetMarketplace().OfferCountForSprite(spriteId));
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

        this.WriteInteger(itemId);
        this.WriteInteger(spriteId);
    }
}
