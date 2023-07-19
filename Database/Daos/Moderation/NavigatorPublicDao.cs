namespace WibboEmulator.Database.Daos.Moderation;
using System.Data;
using WibboEmulator.Database.Interfaces;

internal sealed class NavigatorPublicDao
{
    internal static DataTable GetAll(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT room_id, image_url, langue, category_type FROM `navigator_public` WHERE enabled = '1' ORDER BY order_num ASC");
        return dbClient.GetTable();
    }
}