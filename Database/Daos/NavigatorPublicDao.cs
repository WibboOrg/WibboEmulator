using Butterfly.Database.Interfaces;
using System.Data;

namespace Butterfly.Database.Daos
{
    class NavigatorPublicDao
    {
        internal static DataTable GetAll(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT room_id,image_url,enabled, langue, game FROM navigator_publics ORDER BY order_num ASC");
            return dbClient.GetTable();
        }
    }
}