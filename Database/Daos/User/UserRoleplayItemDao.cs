namespace WibboEmulator.Database.Daos.User;

using System.Data;
using Dapper;

internal sealed class UserRoleplayItemDao
{
    internal static void Delete(IDbConnection dbClient, int userId, int roleplayId) => dbClient.Execute(
        "DELETE FROM `user_roleplay_item` WHERE `user_id` = '" + userId + "' AND `rp_id` = '" + roleplayId + "'");

    internal static List<UserRoleplayItemEntity> GetAll(IDbConnection dbClient, int userId, int roleplayId) => dbClient.Query<UserRoleplayItemEntity>(
        "SELECT `id`, `user_id`, `rp_id`, `item_id`, `count` FROM `user_roleplay_item` WHERE `user_id` = '" + userId + "' AND `rp_id` = '" + roleplayId + "'"
    ).ToList();

    internal static int Insert(IDbConnection dbClient, int userId, int roleplayId, int itemId, int count) => dbClient.ExecuteScalar<int>(
        "INSERT INTO user_roleplay_item (user_id, rp_id, item_id, count) VALUES (@UserId, @RoleplayId, @ItemId, @Count); SELECT LAST_INSERT_ID();",
        new { UserId = userId, RoleplayId = roleplayId, ItemId = itemId, Count = count });

    internal static void UpdateAddCount(IDbConnection dbClient, int itemId, int count) => dbClient.Execute(
        "UPDATE `user_roleplay_item` SET `count` = `count` + '" + count + "' WHERE `id` = '" + itemId + "' LIMIT 1");

    internal static void UpdateRemoveCount(IDbConnection dbClient, int itemId, int count) => dbClient.Execute(
        "UPDATE `user_roleplay_item` SET `count` = `count` - '" + count + "' WHERE `id` = '" + itemId + "' LIMIT 1");

    internal static void Delete(IDbConnection dbClient, int itemId) => dbClient.Execute(
        "DELETE FROM `user_roleplay_item` WHERE `id` = '" + itemId + "' LIMIT 1");
}

public class UserRoleplayItemEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int RpId { get; set; }
    public int ItemId { get; set; }
    public int Count { get; set; }
}