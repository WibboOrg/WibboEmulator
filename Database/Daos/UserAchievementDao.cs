using Butterfly.Database.Interfaces;
using System.Data;

namespace Butterfly.Database.Daos
{
    class UserAchievementDao
    {
        internal static void Replace(IQueryAdapter dbClient, int userId, int newLevel, int newProgress, string achievementGroup)
        {
            dbClient.SetQuery("REPLACE INTO user_achievement VALUES ('" + userId + "', @group, '" + newLevel + "', '" + newProgress + "')");
            dbClient.AddParameter("group", achievementGroup);
            dbClient.RunQuery();
        }

        internal static DataTable GetAll(IQueryAdapter dbClient, int userId)
        {
            dbClient.SetQuery("SELECT group, level, progress FROM user_achievement WHERE user_id = '" + userId + "';");
            return dbClient.GetTable();
        }
    }
}