using System.Data;
using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class UserPresentsDao
    {
        internal static DataRow GetOne(IQueryAdapter dbClient, int itemId)
        {
            dbClient.SetQuery("SELECT base_id,extra_data FROM user_presents WHERE item_id = @presentId LIMIT 1");
            dbClient.AddParameter("presentId", itemId);
            return dbClient.GetRow();
        }

        internal static void Delete(IQueryAdapter dbClient, int itemId)
        {
            dbClient.RunQuery("DELETE FROM user_presents WHERE item_id = '" + itemId + "' LIMIT 1");
        }
    }
}