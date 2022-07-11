using WibboEmulator.Database.Interfaces;
using System.Data;

namespace WibboEmulator.Database.Daos
{
    class NavigatorCategoryDao
    {
        internal static DataTable GetAll(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT `id`, `category`, `category_identifier`, `public_name`, `view_mode`, `required_rank`, `category_type`, `search_allowance`, `minimized`, `enabled`, `order_id` FROM `navigator_category` ORDER BY `id` ASC");
            return dbClient.GetTable();
        }
    }
}