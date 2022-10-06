namespace WibboEmulator.Games.Catalog;
using WibboEmulator.Games.Items;

public class CatalogItem
{
    public int Id { get; private set; }
    public int ItemId { get; private set; }
    public ItemData Data { get; private set; }
    public int Amount { get; private set; }
    public int CostCredits { get; private set; }
    public bool HaveOffer { get; private set; }
    public bool IsLimited { get; private set; }
    public string Name { get; private set; }
    public int PageID { get; private set; }
    public int CostDuckets { get; private set; }
    public int LimitedEditionStack { get; private set; }
    public int LimitedEditionSells { get; set; }
    public int CostWibboPoints { get; private set; }
    public int CostLimitCoins { get; private set; }
    public string Badge { get; private set; }

    public CatalogItem(int id, int itemId, ItemData data, string catalogName, int pageId, int costCredits, int costPixels,
        int costWibboPoints, int costLimitCoins, int amount, int limitedEditionSells, int limitedEditionStack, bool haveOffer, string badge)
    {
        this.Id = id;
        this.Name = catalogName;
        this.ItemId = itemId;
        this.Data = data;
        this.PageID = pageId;
        this.CostCredits = costCredits;
        this.CostDuckets = costPixels;
        this.CostWibboPoints = costWibboPoints;
        this.CostLimitCoins = costLimitCoins;
        this.Amount = amount;
        this.LimitedEditionSells = limitedEditionSells;
        this.LimitedEditionStack = limitedEditionStack;
        this.IsLimited = limitedEditionStack > 0;
        this.HaveOffer = haveOffer;
        this.Badge = badge;
    }
}
