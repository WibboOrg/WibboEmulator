namespace WibboEmulator.Database.Daos.User;
using System.Data;
using Dapper;

internal sealed class UserBannerDao
{
    internal static void Insert(IDbConnection dbClient, int userId, int bannerId) => dbClient.Execute(
        "INSERT INTO user_banner (user_id, banner_id) VALUES (@UserId, @BannerId)",
        new { UserId = userId, BannerId = bannerId });

    internal static void Delete(IDbConnection dbClient, int userId, int bannerId) => dbClient.Execute(
        "DELETE FROM user_banner WHERE banner_id = @BannerId AND user_id = @UserId LIMIT 1",
        new { BannerId = bannerId, UserId = userId });

    internal static List<int> GetAll(IDbConnection dbClient, int userId) => dbClient.Query<int>(
        "SELECT `banner_id` FROM `user_banner` WHERE user_id = @UserId",
        new { UserId = userId }
    ).ToList();
}