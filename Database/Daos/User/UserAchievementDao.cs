namespace WibboEmulator.Database.Daos;
using System.Data;
using WibboEmulator.Database.Interfaces;

internal class UserAchievementDao
{
    internal static void Replace(IQueryAdapter dbClient, int userId, int newLevel, int newProgress, string achievementGroup)
    {
        dbClient.SetQuery("REPLACE INTO `user_achievement` VALUES ('" + userId + "', @group, '" + newLevel + "', '" + newProgress + "')");
        dbClient.AddParameter("group", achievementGroup);
        dbClient.RunQuery();
    }

    internal static DataTable GetAll(IQueryAdapter dbClient, int userId)
    {
        dbClient.SetQuery("SELECT `group`, `level`, `progress` FROM `user_achievement` WHERE `user_id` = '" + userId + "'");
        return dbClient.GetTable();
    }
}