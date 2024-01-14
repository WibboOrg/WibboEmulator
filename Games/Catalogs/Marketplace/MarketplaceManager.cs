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
        var num = 0;
        var num2 = 0;
        if (this.MarketAverages.ContainsKey(spriteID) && this.MarketCounts.ContainsKey(spriteID))
        {
            if (this.MarketCounts[spriteID] > 0)
            {
                return this.MarketAverages[spriteID] / this.MarketCounts[spriteID];
            }
            return 0;
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().Connection())
        {
            num = CatalogMarketplaceDataDao.GetPriceBySprite(dbClient, spriteID);

            num2 = CatalogMarketplaceDataDao.GetSoldBySprite(dbClient, spriteID);
        }

        this.MarketAverages.Add(spriteID, num);
        this.MarketCounts.Add(spriteID, num2);

        if (num2 > 0)
        {
            return Convert.ToInt32(Math.Ceiling((double)(num / num2)));
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
