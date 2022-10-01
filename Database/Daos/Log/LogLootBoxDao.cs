using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.GameClients;

namespace WibboEmulator.Database.Daos
{
    class LogLootBoxDao
    {
        internal static void Insert(IQueryAdapter dbClient, string interactionType, int userId, int itemId, int baseId)
        {
            dbClient.SetQuery("INSERT INTO `log_lootbox` (interaction_type, user_id, item_id, base_id, timestamp) VALUES ('" + interactionType + "', '" + userId + "', '" + itemId + "', '" + baseId + "', UNIX_TIMESTAMP())");
            dbClient.RunQuery();
        }

        internal static int GetCount(IQueryAdapter dbClient, string interactionType, int timestamp, int rarityLevel)
        {
            dbClient.SetQuery("SELECT COUNT(0) FROM `log_lootbox` LEFT JOIN item_base ON (log_lootbox.base_id = item_base.id) WHERE log_lootbox.interaction_type = @interactionType AND log_lootbox.timestamp > @timestamp AND item_base.rarity_level = @rarityLevel");
            dbClient.AddParameter("interactionType", interactionType);
            dbClient.AddParameter("timestamp", timestamp);
            dbClient.AddParameter("rarityLevel", rarityLevel);

            return dbClient.GetInteger();
        }
    }
}