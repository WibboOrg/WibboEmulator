namespace WibboEmulator.Database.Daos.User;
using System.Data;
using WibboEmulator.Database.Interfaces;

internal sealed class UserBannerDao
{
    internal static void Insert(IQueryAdapter dbClient, int userId, int bannerId)
    {
        dbClient.SetQuery("INSERT INTO `user_banner` (`user_id`, `banner_id`) VALUES (" + userId + ", @bannerId)");
        dbClient.AddParameter("bannerId", bannerId);
        dbClient.RunQuery();
    }

    internal static void Delete(IQueryAdapter dbClient, int userId, int bannerId)
    {
        dbClient.SetQuery("DELETE FROM `user_banner` WHERE banner_id = @bannerId AND user_id = '" + userId + "' LIMIT 1");
        dbClient.AddParameter("bannerid", bannerId);
        dbClient.RunQuery();
    }

    internal static DataTable GetAll(IQueryAdapter dbClient, int userId)
    {
        dbClient.SetQuery("SELECT `banner_id` FROM `user_banner` WHERE user_id = '" + userId + "'");
        return dbClient.GetTable();
    }
}