using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class ItemDao
    {
        internal static int insertItem(IQueryAdapter dbClient, int itemId, int userId, string extraData)
        {
            dbClient.SetQuery("INSERT INTO `items` (`base_item`,`user_id`,`extra_data`) VALUES (@baseId, @habboId, @extra_data)");
            dbClient.AddParameter("baseId", itemId);
            dbClient.AddParameter("habboId", userId);
            dbClient.AddParameter("extra_data", extraData);

            return Convert.ToInt32(dbClient.InsertQuery());
        }

        internal static int deleteItem(IQueryAdapter dbClient, int itemId)
        {
            dbClient.SetQuery("DELETE FROM `items` WHERE `id` = @deleteId LIMIT 1");
            dbClient.AddParameter("deleteId", itemId);
            dbClient.RunQuery();
        }
    }
}
