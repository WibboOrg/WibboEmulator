namespace WibboEmulator.Database.Daos.Guild;
using System.Data;
using Dapper;

internal sealed class GuildRequestDao
{
    internal static void Delete(IDbConnection dbClient, int groupId) => dbClient.Execute(
        "DELETE FROM `guild_request` WHERE group_id = @GroupId",
        new { GroupId = groupId });

    internal static List<int> GetAll(IDbConnection dbConnection, int groupId) => dbConnection.Query<int>(
        "SELECT user_id FROM guild_request WHERE group_id = @GroupId",
        new { GroupId = groupId }
    ).ToList();

    internal static List<int> GetAllBySearch(IDbConnection dbConnection, int groupId, string searchVal) => dbConnection.Query<int>(
        @"SELECT user.id 
        FROM guild_request 
        INNER JOIN user ON guild_request.user_id = user.id 
        WHERE guild_request.group_id = @GroupId 
        AND user.username LIKE @Username 
        LIMIT 14",
        new { GroupId = groupId, Username = searchVal.Replace("%", "\\%").Replace("_", "\\_") + "%" }
    ).ToList();

    internal static void Insert(IDbConnection dbConnection, int groupId, int userId) => dbConnection.Execute(
        "INSERT INTO guild_request (user_id, group_id) VALUES (@UserId, @GroupId)",
        new { UserId = userId, GroupId = groupId });

    internal static void Delete(IDbConnection dbConnection, int groupId, int userId) => dbConnection.Execute(
        "DELETE FROM guild_request WHERE user_id = @UserId AND group_id = @GroupId LIMIT 1",
        new { UserId = userId, GroupId = groupId });
}