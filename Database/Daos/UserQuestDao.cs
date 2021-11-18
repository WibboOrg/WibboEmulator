using Butterfly.Database.Interfaces;
using System.Data;

namespace Butterfly.Database.Daos
{
    class UserQuestDao
    {
        internal static void Update(IQueryAdapter dbClient, int userId, int questId, int progress)
        {
            dbClient.RunQuery("UPDATE `user_quest` SET `progress` = '" + progress + "' WHERE `user_id` = '" + userId + "' AND `quest_id` = '" + questId + "'");
        }

        internal static void Replace(IQueryAdapter dbClient, int userId, int questId)
        {
            dbClient.RunQuery("REPLACE INTO `user_quest` VALUES (" + userId + ", " + questId + ", 0)");
        }

        internal static void Delete(IQueryAdapter dbClient, int userId, int questId)
        {
            dbClient.RunQuery("DELETE FROM `user_quest` WHERE `user_id` = '" + userId + "' AND `quest_id` = '" + questId + "'");
        }

        internal static DataTable GetAll(IQueryAdapter dbClient, int userId)
        {
            dbClient.SetQuery("SELECT `user_id`, `quest_id`, `progress` FROM `user_quest` WHERE `user_id` = '" + userId + "'");
            return dbClient.GetTable();
        }
    }
}