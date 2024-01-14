namespace WibboEmulator.Database.Daos.Catalog;
using System.Data;
using Dapper;

internal sealed class CatalogMarketplaceDataDao
{
    internal static void Replace(IDbConnection dbClient, int spriteId, int totalPrice)
    {
        var id = dbClient.QueryFirstOrDefault<int>(
            "SELECT id FROM catalog_marketplace_data WHERE sprite = @SpriteId LIMIT 1",
            new { SpriteId = spriteId });

        if (id > 0)
        {
            _ = dbClient.Execute(
                "UPDATE catalog_marketplace_data SET sold = sold + 1, avgprice = (avgprice + @TotalPrice) WHERE id = @Id LIMIT 1",
                new { TotalPrice = totalPrice, Id = id });
        }
        else
        {
            _ = dbClient.Execute(
                "INSERT INTO catalog_marketplace_data (sprite, sold, avgprice) VALUES (@SpriteId, 1, @TotalPrice)",
                new { SpriteId = spriteId, TotalPrice = totalPrice });
        }
    }

    internal static int GetPriceBySprite(IDbConnection dbClient, int spriteId) => dbClient.QueryFirstOrDefault<int>(
        "SELECT avgprice FROM catalog_marketplace_data WHERE sprite = @SpriteId LIMIT 1",
        new { SpriteId = spriteId });

    internal static int GetSoldBySprite(IDbConnection dbClient, int spriteId) => dbClient.QueryFirstOrDefault<int>(
        "SELECT sold FROM catalog_marketplace_data WHERE sprite = @SpriteId LIMIT 1",
        new { SpriteId = spriteId });
}
