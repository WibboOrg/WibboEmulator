namespace WibboEmulator.Database.Daos.Catalog;
using System.Data;
using Dapper;

internal sealed class CatalogItemLimitedDao
{
    internal static void Update(IDbConnection dbClient, int itemId, int limitSells) => dbClient.Execute(
        "UPDATE `catalog_item_limited` SET `limited_sells` = @LimitSells WHERE `catalog_item_id` = @ItemId LIMIT 1",
        new { LimitSells = limitSells, ItemId = itemId });
}
