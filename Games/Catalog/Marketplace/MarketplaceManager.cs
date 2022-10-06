namespace WibboEmulator.Games.Catalog.Marketplace;
using WibboEmulator.Database.Daos.Catalog;

public class MarketplaceManager
{
    public List<int> MarketItemKeys = new();
    public List<MarketOffer> MarketItems = new();
    public Dictionary<int, int> MarketCounts = new();
    public Dictionary<int, int> MarketAverages = new();

    public MarketplaceManager()
    {
        this.MarketItemKeys = new List<int>();
        this.MarketItems = new List<MarketOffer>();
        this.MarketCounts = new Dictionary<int, int>();
        this.MarketAverages = new Dictionary<int, int>();
    }

    public int AvgPriceForSprite(int SpriteID)
    {
        var num = 0;
        var num2 = 0;
        if (this.MarketAverages.ContainsKey(SpriteID) && this.MarketCounts.ContainsKey(SpriteID))
        {
            if (this.MarketCounts[SpriteID] > 0)
            {
                return this.MarketAverages[SpriteID] / this.MarketCounts[SpriteID];
            }
            return 0;
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            num = CatalogMarketplaceDataDao.GetPriceBySprite(dbClient, SpriteID);

            num2 = CatalogMarketplaceDataDao.GetSoldBySprite(dbClient, SpriteID);
        }

        this.MarketAverages.Add(SpriteID, num);
        this.MarketCounts.Add(SpriteID, num2);

        if (num2 > 0)
        {
            return Convert.ToInt32(Math.Ceiling((double)(num / num2)));
        }

        return 0;
    }

    public double FormatTimestamp() => WibboEnvironment.GetUnixTimestamp() - 172800;

    public int OfferCountForSprite(int spriteID)
    {
        var dictionary = new Dictionary<int, MarketOffer>();
        var dictionary2 = new Dictionary<int, int>();
        foreach (var item in this.MarketItems)
        {
            if (dictionary.ContainsKey(item.SpriteId))
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
        if (dictionary2.ContainsKey(spriteID))
        {
            return dictionary2[spriteID];
        }
        return 0;
    }

    public int CalculateComissionPrice(float sellingPrice) => Convert.ToInt32(Math.Ceiling(sellingPrice / 100 * 1));
}
