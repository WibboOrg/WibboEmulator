namespace WibboEmulator.Database.Daos.Catalog;
using System.Data;
using Dapper;

internal sealed class CatalogItemLimitedDao
{
    internal static List<CatalogItemLimitedEntity> GetAll(IDbConnection dbClient) => dbClient.Query<CatalogItemLimitedEntity>(
        @"SELECT catalog_item.item_id, catalog_item_limited.limited_sells, catalog_item_limited.limited_stack
        FROM `catalog_item_limited` 
        LEFT JOIN `catalog_item` 
        ON (`catalog_item_limited`.catalog_item_id = id)"
    ).ToList();

    internal static void Update(IDbConnection dbClient, int itemId, int limitSells) => dbClient.Execute(
        "UPDATE `catalog_item_limited` SET `limited_sells` = @LimitSells WHERE `catalog_item_id` = @ItemId LIMIT 1",
        new { LimitSells = limitSells, ItemId = itemId });
}

public class CatalogItemLimitedEntity
{
    public int ItemId { get; set; }
    public int LimitedSells { get; set; }
    public int LimitedStack { get; set; }
}
