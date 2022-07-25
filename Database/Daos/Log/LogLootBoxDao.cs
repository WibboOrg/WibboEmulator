using WibboEmulator.Database.Interfaces;

namespace WibboEmulator.Database.Daos
{
    class LogLootBoxDao
    {
        internal static void Insert(IQueryAdapter dbClient, int userId, int itemId, int baseId)
        {
            dbClient.SetQuery("INSERT INTO `log_lootbox` (user_id, item_id, base_id, timestamp) VALUES ('" + userId + "', '" + itemId + "', '" + baseId + "', UNIX_TIMESTAMP())");
            dbClient.RunQuery();
        }
    }
}