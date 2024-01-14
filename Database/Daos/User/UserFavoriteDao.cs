namespace WibboEmulator.Database.Daos.User;
using System.Data;
using Dapper;

internal sealed class UserFavoriteDao
{
    internal static void Insert(IDbConnection dbClient, int userId, int roomId) => dbClient.Execute(
        "INSERT INTO `user_favorite` (`user_id`, `room_id`) VALUES ('" + userId + "','" + roomId + "')");

    internal static void Delete(IDbConnection dbClient, int userId, int roomId) => dbClient.Execute(
        "DELETE FROM `user_favorite` WHERE `user_id` = '" + userId + "' AND `room_id` = '" + roomId + "'");

    internal static void Delete(IDbConnection dbClient, int roomId) => dbClient.Execute(
        "DELETE FROM `user_favorite` WHERE `room_id` = '" + roomId + "'");

    internal static List<int> GetAll(IDbConnection dbClient, int userId) => dbClient.Query<int>(
        "SELECT `room_id` FROM `user_favorite` WHERE `user_id` = @UserId",
        new { UserId = userId }
    ).ToList();
}