using System.Data;
using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class UserPresentDao
    {
        internal static void Insert(IQueryAdapter dbClient, int itemId, int baseId, string extraData)
        {
            dbClient.SetQuery("INSERT INTO user_presents (item_id,base_id,extra_data) VALUES (@itemId, @baseId, @extra_data)");
            dbClient.AddParameter("itemId", itemId);
            dbClient.AddParameter("baseId", baseId);
            dbClient.AddParameter("extra_data", extraData);
            dbClient.RunQuery();
        }

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
