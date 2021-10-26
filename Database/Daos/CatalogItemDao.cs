using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class CatalogItemDao
    {
        internal static void UpdateLimited(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("UPDATE `catalog_items` SET `limited_sells` = @limitSells WHERE `id` = @itemId LIMIT 1");
            dbClient.AddParameter("limitSells", Item.LimitedEditionSells);
            dbClient.AddParameter("itemId", Item.Id);
            dbClient.RunQuery();
        }

         internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT id,item_id,catalog_name,cost_credits,cost_pixels,cost_diamonds,amount,page_id,limited_sells,limited_stack,offer_active,badge FROM catalog_items ORDER by ID DESC");
            DataTable CatalogueItems = dbClient.GetTable();
        }

          internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("SELECT item_id FROM catalog_items WHERE page_id IN (SELECT id FROM catalog_pages WHERE min_rank <= '" + Session.GetHabbo().Rank + "') AND cost_pixels = '0' AND cost_diamonds = '0' AND limited_sells = '0' AND limited_stack = '0' AND offer_active = '1' GROUP BY item_id");
    }
    }
}
