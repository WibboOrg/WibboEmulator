namespace WibboEmulator.Database.Daos;
using System.Data;
using WibboEmulator.Database.Interfaces;

internal class NavigatorCategoryDao
{
    internal static DataTable GetAll(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT `id`, `category`, `category_identifier`, `public_name`, `view_mode`, `required_rank`, `category_type`, `search_allowance`, `minimized`, `enabled`, `order_id` FROM `navigator_category` ORDER BY `id` ASC");
        return dbClient.GetTable();
    }
}