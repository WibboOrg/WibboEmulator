namespace WibboEmulator.Games.Catalog.Marketplace;

public class MarketOffer
{
    public int OfferID { get; set; }
    public int ItemType { get; set; }
    public int SpriteId { get; set; }
    public int TotalPrice { get; set; }
    public int LimitedNumber { get; set; }
    public int LimitedStack { get; set; }

    public MarketOffer(int offerID, int spriteId, int totalPrice, int itemType, int limitedNumber, int limitedStack)
    {
        this.OfferID = offerID;
        this.SpriteId = spriteId;
        this.ItemType = itemType;
        this.TotalPrice = totalPrice;
        this.LimitedNumber = limitedNumber;
        this.LimitedStack = limitedStack;
    }
}
