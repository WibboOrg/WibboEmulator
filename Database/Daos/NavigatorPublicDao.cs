using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class NavigatorPublicDao
    {
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT room_id,image_url,enabled, langue, game FROM navigator_publics ORDER BY order_num ASC");
            DataTable GetPublics = dbClient.GetTable();
        }
    }
}