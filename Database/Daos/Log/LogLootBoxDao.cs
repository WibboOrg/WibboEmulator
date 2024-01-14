namespace WibboEmulator.Database.Daos.Log;
using System.Data;
using Dapper;

internal sealed class LogLootBoxDao
{
    internal static void Insert(IDbConnection dbClient, string interactionType, int userId, int itemId, int baseId) => dbClient.Execute(
        "INSERT INTO log_lootbox (interaction_type, user_id, item_id, base_id, timestamp) VALUES (@InteractionType, @UserId, @ItemId, @BaseId, UNIX_TIMESTAMP())",
        new { InteractionType = interactionType, UserId = userId, ItemId = itemId, BaseId = baseId });

    internal static int GetCount(IDbConnection dbClient, string interactionType, int timestamp, int rarityLevel) => dbClient.ExecuteScalar<int>(
        "SELECT COUNT(0) FROM log_lootbox LEFT JOIN item_base ON (log_lootbox.base_id = item_base.id) WHERE log_lootbox.interaction_type = @InteractionType AND log_lootbox.timestamp > @Timestamp AND item_base.rarity_level = @RarityLevel",
        new { InteractionType = interactionType, Timestamp = timestamp, RarityLevel = rarityLevel });
}