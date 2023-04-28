namespace WibboEmulator.Communication.Packets.Outgoing.Catalog;

using WibboEmulator.Games.Catalogs;

internal sealed class ClubGiftInfoComposer : ServerPacket
{
    public ClubGiftInfoComposer(List<CatalogItem> items)
        : base(ServerPacketHeader.CLUB_GIFT_INFO)
    {
        this.WriteInteger(0); //daysUntilNextGift
        this.WriteInteger(0); //giftsAvailable

        this.WriteInteger(items.Count); //offerCount

        foreach (var item in items)
        {
            CatalogItemUtility.GenerateOfferData(item, true, this);
        }

        this.WriteInteger(items.Count); //giftDataCount

        foreach (var item in items)
        {
            this.WriteInteger(item.Id); //offerId
            this.WriteBoolean(true); //isVip
            this.WriteInteger(0); //daysRequired
            this.WriteBoolean(true); //isSelectable
        }
    }
}
