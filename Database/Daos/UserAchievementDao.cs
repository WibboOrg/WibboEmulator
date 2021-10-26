using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class UserAchievementDao
    {
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("REPLACE INTO user_achievement VALUES ('" + Session.GetHabbo().Id + "', @group, '" + NewLevel + "', '" + NewProgress + "')");
            dbClient.AddParameter("group", AchievementGroup);
            dbClient.RunQuery();
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("REPLACE INTO user_achievement VALUES ('" + Session.GetHabbo().Id + "', @group, '" + NewLevel + "', '" + NewProgress + "')");
            dbClient.AddParameter("group", AchievementGroup);
            dbClient.RunQuery();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT group, level, progress FROM user_achievement WHERE user_id = '" + userId + "';");
            Achievement = dbClient.GetTable();
        }
    }
}