namespace WibboEmulator.Games.Catalogs.Marketplace;

using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Catalog;

public static class MarketplaceManager
{
    public static List<int> MarketItemKeys { get; set; } = [];
    public static List<MarketOffer> MarketItems { get; set; } = [];
    public static Dictionary<int, int> MarketCounts { get; set; } = [];
    public static Dictionary<int, int> MarketAverages { get; set; } = [];

    public static int AvgPriceForSprite(int spriteID)
    {
        if (MarketAverages.TryGetValue(spriteID, out var spriteCount) && MarketCounts.TryGetValue(spriteID, out var averageSprite))
        {
            if (spriteCount > 0)
            {
                return averageSprite / spriteCount;
            }

            return 0;
        }

        using var dbClient = DatabaseManager.Connection;

        var priceSprite = CatalogMarketplaceDataDao.GetPriceBySprite(dbClient, spriteID);
        var soldSprite = CatalogMarketplaceDataDao.GetSoldBySprite(dbClient, spriteID);

        MarketAverages.Add(spriteID, priceSprite);
        MarketCounts.Add(spriteID, soldSprite);

        if (soldSprite > 0)
        {
            return Convert.ToInt32(Math.Ceiling((double)(priceSprite / soldSprite)));
        }

        return 0;
    }

    public static double FormatTimestamp() => WibboEnvironment.GetUnixTimestamp() - 172800;

    public static int OfferCountForSprite(int spriteID)
    {
        var dictionary = new Dictionary<int, MarketOffer>();
        var dictionary2 = new Dictionary<int, int>();
        foreach (var item in MarketItems)
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
