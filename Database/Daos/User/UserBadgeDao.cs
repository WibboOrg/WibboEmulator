namespace WibboEmulator.Database.Daos.User;
using System.Data;
using Dapper;

internal sealed class UserBadgeDao
{
    internal static void UpdateResetSlot(IDbConnection dbClient, int userId) => dbClient.Execute(
        "UPDATE `user_badge` SET badge_slot = '0' WHERE user_id = '" + userId + "' AND badge_slot != '0'");

    internal static void UpdateSlot(IDbConnection dbClient, int userId, int slot, string badge) => dbClient.Execute(
        "UPDATE user_badge SET badge_slot = @Slot WHERE badge_id = @Badge AND user_id = @UserId",
        new { Slot = slot, Badge = badge, UserId = userId });

    internal static void InsertAll(IDbConnection dbClient, List<int> userIds, string badge)
    {
        if (userIds.Count == 0)
        {
            return;
        }

        var badgeList = new List<UserBadgeEntity>();

        foreach (var userId in userIds)
        {
            badgeList.Add(new UserBadgeEntity() { BadgeId = badge, UserId = userId });
        }

        _ = dbClient.Execute(
            "INSERT INTO `user_badge` (user_id, badge_id, badge_slot) VALUES (@UserId, @BadgeId, 0)",
            badgeList);
    }

    internal static void Insert(IDbConnection dbClient, int userId, int slot, string badge) => dbClient.Execute(
        "INSERT INTO user_badge (user_id, badge_id, badge_slot) VALUES (@UserId, @Badge, @Slot)",
        new { UserId = userId, Badge = badge, Slot = slot });

    internal static void Delete(IDbConnection dbClient, int userId, string badge) => dbClient.Execute(
        "DELETE FROM user_badge WHERE badge_id = @Badge AND user_id = @UserId LIMIT 1",
        new { Badge = badge, UserId = userId });

    internal static List<UserBadgeEntity> GetAllProfil(IDbConnection dbClient, int userId) => dbClient.Query<UserBadgeEntity>(
        "SELECT `user_id`, `badge_id`, `badge_slot` FROM `user_badge` WHERE user_id = '" + userId + "' AND badge_slot != '0'"
    ).ToList();

    internal static List<UserBadgeEntity> GetAll(IDbConnection dbClient, int userId) => dbClient.Query<UserBadgeEntity>(
        "SELECT `user_id`, `badge_id`, `badge_slot` FROM `user_badge` WHERE user_id = '" + userId + "'"
    ).ToList();
}

public class UserBadgeEntity
{
    public int UserId { get; set; }

    public string BadgeId { get; set; }

    public int BadgeSlot { get; set; }
}
