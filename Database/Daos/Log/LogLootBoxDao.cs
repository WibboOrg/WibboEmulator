using WibboEmulator.Database.Interfaces;

namespace WibboEmulator.Database.Daos
{
    class LogLootBoxDao
    {
        internal static void Insert(IQueryAdapter dbClient, string interactionType, int userId, int itemId, int baseId)
        {
            dbClient.SetQuery("INSERT INTO `log_lootbox` (interaction_type, user_id, item_id, base_id, timestamp) VALUES ('" + interactionType + "', '" + userId + "', '" + itemId + "', '" + baseId + "', UNIX_TIMESTAMP())");
            dbClient.RunQuery();
        }
    }
}