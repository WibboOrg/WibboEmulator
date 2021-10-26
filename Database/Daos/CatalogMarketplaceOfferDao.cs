using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class CatalogMarketplaceOfferDao
    {
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT state,timestamp,total_price,extra_data,item_id,furni_id,user_id,limited_number,limited_stack FROM catalog_marketplace_offers WHERE offer_id = @OfferId LIMIT 1");
            dbClient.AddParameter("OfferId", OfferId);
            Row = dbClient.GetRow();
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE catalog_marketplace_offers SET state = '2' WHERE offer_id = '" + OfferId + "' LIMIT 1");
        }

        internal static void Query8(IQueryAdapter dbClient)
        {

            dbClient.SetQuery("SELECT offer_id,item_type,sprite_id,total_price,limited_number,limited_stack FROM catalog_marketplace_offers " + builder.ToString() + " " + str + " LIMIT 500");
            dbClient.AddParameter("search_query", SearchQuery.Replace("%", "\\%").Replace("_", "\\_") + "%");
            if (SearchQuery.Length >= 1)
            {
                builder.Append(" AND public_name LIKE @search_query");
            }
            table = dbClient.GetTable();
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("DELETE FROM catalog_marketplace_offers WHERE offer_id = @OfferId AND user_id = @UserId LIMIT 1");
            dbClient.AddParameter("OfferId", OfferId);
            dbClient.AddParameter("UserId", Session.GetHabbo().Id);
            dbClient.RunQuery();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT furni_id, item_id, user_id, extra_data, offer_id, state, timestamp, limited_number, limited_stack FROM catalog_marketplace_offers WHERE offer_id = @OfferId LIMIT 1");
            dbClient.AddParameter("OfferId", OfferId);
            Row = dbClient.GetRow();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT offer_id, item_type, sprite_id, total_price, limited_number,limited_stack FROM catalog_marketplace_offers " + builder.ToString() + " " + str + " LIMIT 500");
            dbClient.AddParameter("search_query", SearchQuery.Replace("%", "\\%").Replace("_", "\\_") + "%");
            table = dbClient.GetTable();
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("INSERT INTO catalog_marketplace_offers (furni_id,item_id,user_id,asking_price,total_price,public_name,sprite_id,item_type,timestamp,extra_data,limited_number,limited_stack) VALUES ('" + ItemId + "','" + Item.BaseItem + "','" + Session.GetHabbo().Id + "','" + SellingPrice + "','" + TotalPrice + "',@public_name,'" + Item.GetBaseItem().SpriteId + "','" + ItemType + "','" + ButterflyEnvironment.GetUnixTimestamp() + "',@extra_data, '" + Item.Limited + "', '" + Item.LimitedStack + "')");
            dbClient.AddParameter("public_name", Item.GetBaseItem().ItemName);
            dbClient.AddParameter("extra_data", Item.ExtraData);
            dbClient.RunQuery();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT asking_price FROM catalog_marketplace_offers WHERE user_id = '" + Session.GetHabbo().Id + "' AND state = '2'");
            Table = dbClient.GetTable();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE FROM catalog_marketplace_offers WHERE user_id = '" + Session.GetHabbo().Id + "' AND state = '2'");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT timestamp, state, offer_id, item_type, sprite_id, total_price, limited_number, limited_stack FROM catalog_marketplace_offers WHERE user_id = '" + UserId + "'");
            table = dbClient.GetTable();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT SUM(asking_price) FROM catalog_marketplace_offers WHERE state = '2' AND user_id = '" + UserId + "'");
            i = dbClient.GetInteger();
        }
    }
}