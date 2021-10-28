using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class UserPresentDao
    {
        internal static void InsertPresent(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("INSERT INTO user_presents (item_id,base_id,extra_data) VALUES (@itemId, @baseId, @extra_data)");
            dbClient.AddParameter("itemId", NewItemId);
            dbClient.AddParameter("baseId", Item.Data.Id);
            dbClient.AddParameter("extra_data", (string.IsNullOrEmpty(ItemExtraData) ? "" : ItemExtraData));
            dbClient.RunQuery();
        }
    }
}
