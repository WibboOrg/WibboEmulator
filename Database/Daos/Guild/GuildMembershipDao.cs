namespace WibboEmulator.Database.Daos.Guild;
using System.Data;
using Dapper;

internal sealed class GuildMembershipDao
{
    internal static void Delete(IDbConnection dbClient, int groupId) => dbClient.Execute(
        "DELETE FROM `guild_membership` WHERE group_id = @GroupId",
        new { GroupId = groupId });

    internal static List<int> GetAllUserIdBySearchAndStaff(IDbConnection dbConnection, int groupId, string searchVal) => dbConnection.Query<int>(
        @"SELECT user.id 
        FROM guild_membership 
        INNER JOIN user ON guild_membership.user_id = user.id 
        WHERE guild_membership.group_id = @GroupId 
        AND guild_membership.rank > '0' 
        AND user.username LIKE @Username 
        LIMIT 14",
        new { GroupId = groupId, Username = searchVal.Replace("%", "\\%").Replace("_", "\\_") + "%" }
    ).ToList();

    internal static List<int> GetAllUserIdBySearch(IDbConnection dbConnection, int groupId, string searchVal) => dbConnection.Query<int>(
        @"SELECT user.id 
        FROM guild_membership 
        INNER JOIN user ON guild_membership.user_id = user.id 
        WHERE guild_membership.group_id = @GroupId 
        AND user.username LIKE @Username 
        LIMIT 14",
        new { GroupId = groupId, Username = searchVal.Replace("%", "\\%").Replace("_", "\\_") + "%" }
    ).ToList();

    internal static List<GuildMembershipEntity> GetAll(IDbConnection dbClient, int id) => dbClient.Query<GuildMembershipEntity>(
        "SELECT user_id, rank FROM `guild_membership` WHERE group_id = @Id",
        new { Id = id }
    ).ToList();

    internal static void UpdateRank(IDbConnection dbConnection, int groupId, int userId, int rank) => dbConnection.Execute(
        "UPDATE guild_membership SET rank = @Rank WHERE user_id = @UserId AND group_id = @GroupId LIMIT 1",
        new { Rank = rank, UserId = userId, GroupId = groupId });

    internal static void Delete(IDbConnection dbConnection, int groupId, int userId) => dbConnection.Execute(
        "DELETE FROM guild_membership WHERE user_id = @UserId AND group_id = @GroupId LIMIT 1",
        new { UserId = userId, GroupId = groupId });

    internal static void Insert(IDbConnection dbConnection, int groupId, int userId) => dbConnection.Execute(
        "INSERT INTO guild_membership (user_id, group_id) VALUES (@UserId, @GroupId)",
        new { GroupId = groupId, UserId = userId });

    internal static List<int> GetAllByUserId(IDbConnection dbClient, int userId) => dbClient.Query<int>(
        "SELECT group_id FROM `guild_membership` WHERE user_id = @UserId",
        new { UserId = userId }
    ).ToList();
}

public class GuildMembershipEntity
{
    public int GroupId { get; set; }
    public int UserId { get; set; }
    public int Rank { get; set; }
}