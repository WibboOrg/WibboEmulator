using Butterfly.Database.Interfaces;
using System.Data;

namespace Butterfly.Database.Daos
{
    class CatalogPromotionDao
    {
        internal static DataTable GetAll(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT * FROM catalog_promotions");
            return dbClient.GetTable();
        }
    }
}
