namespace WibboEmulator.Database.Daos.Catalog;
using System.Data;
using WibboEmulator.Database.Interfaces;

internal sealed class CatalogItemDao
{
    internal static DataTable GetAll(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT id, item_id, catalog_name, cost_credits, cost_pixels, cost_diamonds, cost_limitcoins, amount, page_id, catalog_item_limited.limited_sells, catalog_item_limited.limited_stack, offer_active, badge FROM `catalog_item` LEFT JOIN `catalog_item_limited` ON (`catalog_item_limited`.catalog_item_id = id) ORDER by ID DESC");

        return dbClient.GetTable();
    }

    internal static DataTable GetItemIdByRank(IQueryAdapter dbClient, string permission)
    {
        dbClient.SetQuery("SELECT `item_id` FROM `catalog_item` WHERE `page_id` IN (SELECT `id` FROM `catalog_page` WHERE `required_right` <= '" + permission + "') AND `cost_pixels` = '0' AND `cost_diamonds` = '0' AND `offer_active` = '1' GROUP BY `item_id`");

        return dbClient.GetTable();
    }
}
