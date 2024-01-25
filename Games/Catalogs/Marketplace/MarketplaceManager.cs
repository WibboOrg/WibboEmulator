namespace WibboEmulator.Games.Catalogs.Marketplace;
using WibboEmulator.Database.Daos.Catalog;

public class MarketplaceManager
{
    public List<int> MarketItemKeys { get; set; } = new();
    public List<MarketOffer> MarketItems { get; set; } = new();
    public Dictionary<int, int> MarketCounts { get; set; } = new();
    public Dictionary<int, int> MarketAverages { get; set; } = new();

    public MarketplaceManager()
    {
        this.MarketItemKeys = new List<int>();
        this.MarketItems = new List<MarketOffer>();
        this.MarketCounts = new Dictionary<int, int>();
        this.MarketAverages = new Dictionary<int, int>();
    }

    public int AvgPriceForSprite(int spriteID)
    {
        if (this.MarketAverages.TryGetValue(spriteID, out var spriteCount) && this.MarketCounts.TryGetValue(spriteID, out var averageSprite))
        {
            if (spriteCount > 0)
            {
                return averageSprite / spriteCount;
            }

            return 0;
        }

        using var dbClient = WibboEnvironment.GetDatabaseManager().Connection();

        var priceSprite = CatalogMarketplaceDataDao.GetPriceBySprite(dbClient, spriteID);
        var soldSprite = CatalogMarketplaceDataDao.GetSoldBySprite(dbClient, spriteID);

        this.MarketAverages.Add(spriteID, priceSprite);
        this.MarketCounts.Add(spriteID, soldSprite);

        if (soldSprite > 0)
        {
            return Convert.ToInt32(Math.Ceiling((double)(priceSprite / soldSprite)));
        }

        return 0;
    }

    public static double FormatTimestamp() => WibboEnvironment.GetUnixTimestamp() - 172800;

    public int OfferCountForSprite(int spriteID)
    {
        var dictionary = new Dictionary<int, MarketOffer>();
        var dictionary2 = new Dictionary<int, int>();
        foreach (var item in this.MarketItems)
        {
            if (dictionary.TryGetValue(item.SpriteId, out var _))
            {
                if (dictionary[item.SpriteId].TotalPrice > item.TotalPrice)
                {
                    _ = dictionary.Remove(item.SpriteId);
                    dictionary.Add(item.SpriteId, item);
                }

                var num = dictionary2[item.SpriteId];
                _ = dictionary2.Remove(item.SpriteId);
                dictionary2.Add(item.SpriteId, num + 1);
            }
            else
            {
                dictionary.Add(item.SpriteId, item);
                dictionary2.Add(item.SpriteId, 1);
            }
        }
        if (dictionary2.TryGetValue(spriteID, out var value))
        {
            return value;
        }
        return 0;
    }
}
