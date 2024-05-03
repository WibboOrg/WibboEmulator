namespace WibboEmulator.Games.Catalogs;
using WibboEmulator.Games.Items;

public class CatalogItem(int id, int itemId, ItemData data, string catalogName, int pageId, int costCredits, int costPixels,
    int costWibboPoints, int costLimitCoins, int amount, int limitedEditionSells, int limitedEditionStack, bool haveOffer, string badge)
{
    public int Id { get; private set; } = id;
    public int ItemId { get; private set; } = itemId;
    public ItemData Data { get; private set; } = data;
    public int Amount { get; private set; } = amount;
    public int CostCredits { get; private set; } = costCredits;
    public bool HaveOffer { get; private set; } = haveOffer;
    public bool IsLimited { get; private set; } = limitedEditionStack > 0;
    public string Name { get; private set; } = catalogName;
    public int PageID { get; private set; } = pageId;
    public int CostDuckets { get; private set; } = costPixels;
    public int LimitedEditionStack { get; private set; } = limitedEditionStack;
    public int LimitedEditionSells { get; set; } = limitedEditionSells;
    public int CostWibboPoints { get; private set; } = costWibboPoints;
    public int CostLimitCoins { get; private set; } = costLimitCoins;
    public string Badge { get; private set; } = badge;
}
