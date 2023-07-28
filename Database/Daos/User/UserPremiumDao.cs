namespace WibboEmulator.Database.Daos.User;

using System.Data;
using WibboEmulator.Database.Interfaces;

internal sealed class UserPremiumDao
{
    internal static DataRow GetOne(IQueryAdapter dbClient, int userId)
    {
        dbClient.SetQuery("SELECT `timestamp_activated`, `timestamp_expire_classic`, `timestamp_expire_epic`, `timestamp_expire_legend` FROM `user_premium` WHERE `user_id` = '" + userId + "' LIMIT 1");
        return dbClient.GetRow();
    }

    internal static void UpdateExpired(IQueryAdapter dbClient, int userId, int activated, int expireClassic, int expireEpic, int expireLegend)
    {
        dbClient.SetQuery("REPLACE INTO `user_premium` (`user_id`, `timestamp_activated`, `timestamp_expire_classic`, `timestamp_expire_epic`, `timestamp_expire_legend`) VALUES ('" + userId + "', @activated, @expireClassic, @expireEpic, @expireLegend)");
        dbClient.AddParameter("activated", activated);
        dbClient.AddParameter("expireClassic", expireClassic);
        dbClient.AddParameter("expireEpic", expireEpic);
        dbClient.AddParameter("expireLegend", expireLegend);
        dbClient.RunQuery();
    }

    internal static void Insert(IQueryAdapter dbClient, int userId)
    {
        dbClient.SetQuery("INSERT INTO `user_premium` (`user_id`) VALUES ('" + userId + "')");
        dbClient.RunQuery();
    }
}
