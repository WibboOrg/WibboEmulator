namespace WibboEmulator.Database.Daos.Catalog;
using WibboEmulator.Database.Interfaces;

internal sealed class CatalogItemLimitedDao
{
    internal static void Update(IQueryAdapter dbClient, int itemId, int limitedEditionSells)
    {
        dbClient.SetQuery("UPDATE `catalog_item_limited` SET `limited_sells` = @limitSells WHERE `catalog_item_id` = @itemId LIMIT 1");
        dbClient.AddParameter("limitSells", limitedEditionSells);
        dbClient.AddParameter("itemId", itemId);
        dbClient.RunQuery();
    }
}
