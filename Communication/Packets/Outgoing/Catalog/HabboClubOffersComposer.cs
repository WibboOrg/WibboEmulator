namespace WibboEmulator.Communication.Packets.Outgoing.Catalog;

using WibboEmulator.Games.Catalogs;

internal sealed class HabboClubOffersComposer : ServerPacket
{
    public HabboClubOffersComposer(Dictionary<int, CatalogItem> offers)
        : base(ServerPacketHeader.CLUB_OFFERS)
    {
        this.WriteInteger(offers.Count); //totalOffers

        foreach (var item in offers.Values)
        {
            this.WriteInteger(item.Id); //offerId
            this.WriteString(item.Name); //productCode
            this.WriteBoolean(false);//useless
            this.WriteInteger(item.CostCredits); //priceCredits

            if (item.CostWibboPoints > 0)
            {
                this.WriteInteger(item.CostWibboPoints);
                this.WriteInteger(105);
            }
            else if (item.CostLimitCoins > 0)
            {
                this.WriteInteger(item.CostLimitCoins);
                this.WriteInteger(55);
            }
            else
            {
                this.WriteInteger(item.CostDuckets);
                this.WriteInteger(0);
            }

            this.WriteBoolean(true); //vip
            this.WriteInteger(1); //months
            this.WriteInteger(0); //extraDays
            this.WriteBoolean(false); //giftable

            var date = DateTime.Now.AddDays(31);

            this.WriteInteger(31); //daysLeftAfterPurchase
            this.WriteInteger(date.Year); //year
            this.WriteInteger(date.Month); //month
            this.WriteInteger(date.Day); //day
        }
    }
}
