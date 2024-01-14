namespace WibboEmulator.Database.Daos.User;
using System.Data;
using Dapper;

internal sealed class UserAchievementDao
{
    internal static void Replace(IDbConnection dbClient, int userId, int newLevel, int newProgress, string achievementGroup) => dbClient.Execute(
        "REPLACE INTO user_achievement VALUES (@UserId, @Group, @NewLevel, @NewProgress)",
        new { UserId = userId, Group = achievementGroup, NewLevel = newLevel, NewProgress = newProgress });

    internal static List<UserAchievementEntity> GetAll(IDbConnection dbClient, int userId) => dbClient.Query<UserAchievementEntity>(
        "SELECT `group`, `level`, `progress` FROM `user_achievement` WHERE `user_id` = @UserId",
        new { UserId = userId }
    ).ToList();
}

public class UserAchievementEntity
{
    public int UserId { get; set; }
    public string Group { get; set; }
    public int Level { get; set; }
    public int Progress { get; set; }
}