using Butterfly.Database.Interfaces;
using System.Data;

namespace Butterfly.Database.Daos
{
    class CatalogItemDao
    {
        internal static void UpdateLimited(IQueryAdapter dbClient, int itemId, int limitedEditionSells)
        {
            dbClient.SetQuery("UPDATE `catalog_item` SET `limited_sells` = @limitSells WHERE `id` = @itemId LIMIT 1");
            dbClient.AddParameter("limitSells", limitedEditionSells);
            dbClient.AddParameter("itemId", itemId);
            dbClient.RunQuery();
        }

        internal static DataTable GetAll(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT id, item_id, catalog_name, cost_credits, cost_pixels, cost_diamonds, cost_limitcoins, amount, page_id, limited_sells, limited_stack, offer_active, badge FROM `catalog_item` ORDER by ID DESC");

            return dbClient.GetTable();
        }

        internal static DataTable GetItemIdByRank(IQueryAdapter dbClient, int rank)
        {
            dbClient.SetQuery("SELECT `item_id` FROM `catalog_item` WHERE `page_id` IN (SELECT `id` FROM `catalog_page` WHERE `min_rank` <= '" + rank + "') AND `cost_pixels` = '0' AND `cost_diamonds` = '0' AND `limited_sells` = '0' AND `limited_stack` = '0' AND `offer_active` = '1' GROUP BY `item_id`");

            return dbClient.GetTable();
        }
    }
}
