using System;
using System.Data;
using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class CatalogMarketplaceDataDao
    {
        internal static void Replace(IQueryAdapter dbClient, int spriteId, int totalPrice)
        {
            int Id = 0;
            dbClient.SetQuery("SELECT id FROM catalog_marketplace_data WHERE sprite = " + spriteId + " LIMIT 1;");
            Id = dbClient.GetInteger();

            if (Id > 0)
            {
                dbClient.RunQuery("UPDATE catalog_marketplace_data SET sold = sold + 1, avgprice = (avgprice + " + totalPrice + ") WHERE id = " + Id + " LIMIT 1;");
            }
            else
            {
                dbClient.RunQuery("INSERT INTO catalog_marketplace_data (sprite, sold, avgprice) VALUES ('" + spriteId + "', '1', '" + totalPrice + "')");
            }
        }

        internal static int GetPriceBySprite(IQueryAdapter dbClient, int spriteId)
        {
            dbClient.SetQuery("SELECT avgprice FROM catalog_marketplace_data WHERE sprite = @SpriteId LIMIT 1");
            dbClient.AddParameter("SpriteId", spriteId);
            return dbClient.GetInteger();
        }

        internal static int GetSoldBySprite(IQueryAdapter dbClient, int spriteID)
        {
            dbClient.SetQuery("SELECT sold FROM catalog_marketplace_data WHERE sprite = '" + spriteID + "' LIMIT 1");
            return dbClient.GetInteger();
        }
    }
}