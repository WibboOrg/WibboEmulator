using Butterfly.Database.Interfaces;
using System.Data;
using System.Text;

namespace Butterfly.Database.Daos
{
    class CatalogMarketplaceOfferDao
    {
        internal static DataRow GetOneByOfferId(IQueryAdapter dbClient, int offerId)
        {
            dbClient.SetQuery("SELECT state, timestamp, total_price, extra_data, item_id, furni_id, user_id, limited_number, limited_stack FROM `catalog_marketplace_offer` WHERE offer_id = @OfferId LIMIT 1");
            dbClient.AddParameter("OfferId", offerId);
            return dbClient.GetRow();
        }

        internal static void UpdateState(IQueryAdapter dbClient, int offerId)
        {
            dbClient.RunQuery("UPDATE `catalog_marketplace_offer` SET state = '2' WHERE offer_id = '" + offerId + "' LIMIT 1");
        }

        internal static DataTable GetAll(IQueryAdapter dbClient, string searchQuery, int minCost, int maxCost, int filterMode)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("WHERE state = '1' AND timestamp >= " + ButterflyEnvironment.GetGame().GetCatalog().GetMarketplace().FormatTimestamp());
            if (minCost >= 0)
            {
                builder.Append(" AND total_price > " + minCost);
            }
            if (maxCost >= 0)
            {
                builder.Append(" AND total_price < " + maxCost);
            }
            string str;
            switch (filterMode)
            {
                case 1:
                    str = "ORDER BY asking_price DESC";
                    break;

                default:
                    str = "ORDER BY asking_price ASC";
                    break;
            }

            if (searchQuery.Length >= 1)
            {
                builder.Append(" AND public_name LIKE @search_query");
            }

            dbClient.SetQuery("SELECT offer_id, item_type, sprite_id, total_price, limited_number, limited_stack FROM `catalog_marketplace_offer` " + builder + " " + str + " LIMIT 500");
            dbClient.AddParameter("search_query", searchQuery.Replace("%", "\\%").Replace("_", "\\_") + "%");
            return dbClient.GetTable();
        }

        internal static void DeleteUserOffer(IQueryAdapter dbClient, int offerId, int userId)
        {
            dbClient.SetQuery("DELETE FROM `catalog_marketplace_offer` WHERE offer_id = @OfferId AND user_id = @UserId LIMIT 1");
            dbClient.AddParameter("OfferId", offerId);
            dbClient.AddParameter("UserId", userId);
            dbClient.RunQuery();
        }

        internal static DataRow GetByOfferId(IQueryAdapter dbClient, int offerId)
        {
            dbClient.SetQuery("SELECT furni_id, item_id, user_id, extra_data, offer_id, state, timestamp, limited_number, limited_stack FROM `catalog_marketplace_offer` WHERE offer_id = @OfferId LIMIT 1");
            dbClient.AddParameter("OfferId", offerId);

            return dbClient.GetRow();
        }

        internal static void Insert(IQueryAdapter dbClient, string itemName, string extraData, int itemId, int baseItem, int userId, int sellingPrice, int totalPrice, int spriteId, int itemType, int limited, int limitedStack)
        {
            dbClient.SetQuery("INSERT INTO `catalog_marketplace_offer` (furni_id,item_id,user_id,asking_price,total_price,public_name,sprite_id,item_type,timestamp,extra_data,limited_number,limited_stack) VALUES ('" + itemId + "','" + baseItem + "','" + userId + "','" + sellingPrice + "','" + totalPrice + "',@public_name,'" + spriteId + "','" + itemType + "','" + ButterflyEnvironment.GetUnixTimestamp() + "',@extra_data, '" + limited + "', '" + limitedStack + "')");
            dbClient.AddParameter("public_name", itemName);
            dbClient.AddParameter("extra_data", extraData);
            dbClient.RunQuery();
        }

        internal static DataTable GetPriceByUserId(IQueryAdapter dbClient, int userId)
        {
            dbClient.SetQuery("SELECT asking_price FROM `catalog_marketplace_offer` WHERE user_id = '" + userId + "' AND state = '2'");
            return dbClient.GetTable();
        }

        internal static void Delete(IQueryAdapter dbClient, int userId)
        {
            dbClient.RunQuery("DELETE FROM `catalog_marketplace_offer` WHERE user_id = '" + userId + "' AND state = '2'");
        }

        internal static DataTable GetOneByUserId(IQueryAdapter dbClient, int userId)
        {
            dbClient.SetQuery("SELECT timestamp, state, offer_id, item_type, sprite_id, total_price, limited_number, limited_stack FROM `catalog_marketplace_offer` WHERE user_id = '" + userId + "'");
            return dbClient.GetTable();
        }

        internal static int GetSunPrice(IQueryAdapter dbClient, int userId)
        {
            dbClient.SetQuery("SELECT SUM(asking_price) FROM `catalog_marketplace_offer` WHERE state = '2' AND user_id = '" + userId + "'");
            return dbClient.GetInteger();
        }
    }
}