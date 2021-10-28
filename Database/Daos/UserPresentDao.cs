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
    }
}
