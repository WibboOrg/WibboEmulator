using Butterfly.Database.Interfaces;
using System.Data;

namespace Butterfly.Database.Daos
{
    class CatalogPageDao
    {
        internal static DataTable GetAll(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT catalog_pages.id, catalog_pages.parent_id, catalog_pages.caption, catalog_pages.page_link, catalog_pages.enabled, catalog_pages.min_rank, catalog_pages.icon_image," +
                                    " catalog_pages.page_layout, catalog_pages.page_strings_1, catalog_pages.page_strings_2, catalog_pages_langue.caption_en, catalog_pages_langue.caption_br," +
                                    " catalog_pages_langue.page_strings_2_en, catalog_pages_langue.page_strings_2_br" +
                                    " FROM catalog_pages" +
                                    " LEFT JOIN catalog_pages_langue ON catalog_pages.id = catalog_pages_langue.page_id" +
                                    " ORDER BY order_num, caption");

            return dbClient.GetTable();
        }
    }
}
