namespace WibboEmulator.Database.Daos.User;

using System.Data;
using Dapper;

internal sealed class UserPremiumDao
{
    internal static UserPremiumEntity GetOne(IDbConnection dbClient, int userId) => dbClient.QuerySingleOrDefault<UserPremiumEntity>(
        "SELECT `timestamp_activated`, `timestamp_expire_classic`, `timestamp_expire_epic`, `timestamp_expire_legend` FROM `user_premium` WHERE `user_id` = '" + userId + "' LIMIT 1");

    internal static void UpdateExpired(IDbConnection dbClient, int userId, int activated, int expireClassic, int expireEpic, int expireLegend) => dbClient.Execute(
        @"REPLACE INTO user_premium (user_id, timestamp_activated, timestamp_expire_classic, timestamp_expire_epic, timestamp_expire_legend)
        VALUES (@UserId, @Activated, @ExpireClassic, @ExpireEpic, @ExpireLegend)",
        new { UserId = userId, Activated = activated, ExpireClassic = expireClassic, ExpireEpic = expireEpic, ExpireLegend = expireLegend });

    internal static void Insert(IDbConnection dbClient, int userId) => dbClient.Execute(
        "INSERT INTO user_premium (user_id) VALUES (@UserId)",
        new { UserId = userId });
}

public class UserPremiumEntity
{
    public int UserId { get; set; }
    public int TimestampActivated { get; set; }
    public int TimestampExpireClassic { get; set; }
    public int TimestampExpireEpic { get; set; }
    public int TimestampExpireLegend { get; set; }
}