using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class UserPresentsDao
    {
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT base_id,extra_data FROM user_presents WHERE item_id = @presentId LIMIT 1");
            dbClient.AddParameter("presentId", Present.Id);
            Data = dbClient.GetRow();
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE FROM user_presents WHERE item_id = '" + Present.Id + "' LIMIT 1");
        }
    }
}