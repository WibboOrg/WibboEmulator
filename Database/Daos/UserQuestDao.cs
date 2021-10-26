using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class UserQuestDao
    {
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE user_quests SET progress = '" + num + "' WHERE user_id = '" + Session.GetHabbo().Id + "' AND quest_id = '" + quest.Id + "'");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("REPLACE INTO user_quests VALUES (" + Session.GetHabbo().Id + ", " + quest.Id + ", 0)");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("REPLACE INTO user_quests VALUES (" + Session.GetHabbo().Id + ", " + nextQuestInSeries.Id + ", 0)");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE FROM user_quests WHERE user_id = '" + Session.GetHabbo().Id + "' AND quest_id = '" + quest.Id + "'");
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT * FROM user_quests WHERE user_id = '" + userId + "';");
            Quests = dbClient.GetTable();
        }
    }
}