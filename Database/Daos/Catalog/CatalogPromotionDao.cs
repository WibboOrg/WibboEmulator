using Wibbo.Database.Interfaces;
using System.Data;

namespace Wibbo.Database.Daos
{
    class CatalogPromotionDao
    {
        internal static DataTable GetAll(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT `id`, `title`, `title_en`, `title_br`, `image`, `unknown`, `page_link`, `parent_id` FROM `catalog_promotion`");
            return dbClient.GetTable();
        }
    }
}
