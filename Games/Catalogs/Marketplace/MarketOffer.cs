namespace WibboEmulator.Games.Catalogs.Marketplace;

public class MarketOffer(int offerID, int spriteId, int totalPrice, int itemType, int limitedNumber, int limitedStack)
{
    public int OfferID { get; set; } = offerID;
    public int ItemType { get; set; } = itemType;
    public int SpriteId { get; set; } = spriteId;
    public int TotalPrice { get; set; } = totalPrice;
    public int LimitedNumber { get; set; } = limitedNumber;
    public int LimitedStack { get; set; } = limitedStack;
}
