using Butterfly.Database.Interfaces;
using System.Data;

namespace Butterfly.Database.Daos
{
    class UserStatsDao
    {
        internal static string BuildUpdateQuery(int userId, int favouriteGroupId, int timeOnlineSec, int currentQuestId, int respect, int dailyRespectPoints, int dailyPetRespectPoints)
        {
            return  "UPDATE `user_stats` SET group_id = " + favouriteGroupId + ", online_time = online_time + " + timeOnlineSec + ", quest_id = '" + currentQuestId + "', Respect = '" + respect + "', daily_respect_points = '" + dailyRespectPoints + "', daily_pet_respect_points = '" + dailyPetRespectPoints + "' WHERE id = '" + userId + "';";
        }

        internal static void UpdateRemoveAllGroupId(IQueryAdapter dbClient, int groupId)
        {
            dbClient.RunQuery("UPDATE `user_stats` SET group_id = '0' WHERE group_id = '" + groupId + "' LIMIT 1");
        }

        internal static void UpdateRemoveGroupId(IQueryAdapter dbClient, int userId)
        {
            dbClient.SetQuery("UPDATE `user_stats` SET group_id = '0' WHERE id = @userId LIMIT 1");
            dbClient.AddParameter("userId", userId);
            dbClient.RunQuery();
        }

        internal static void UpdateGroupId(IQueryAdapter dbClient, int groupId, int userId)
        {
            dbClient.SetQuery("UPDATE `user_stats` SET group_id = @groupId WHERE id = @userId LIMIT 1");
            dbClient.AddParameter("groupId", groupId);
            dbClient.AddParameter("userId", userId);
            dbClient.RunQuery();
        }

        internal static void UpdateAchievementScore(IQueryAdapter dbClient, int userId, int score)
        {
            dbClient.RunQuery("UPDATE `user_stats` SET achievement_score = achievement_score + '" + score + "' WHERE id = '" + userId + "'");
        }

        internal static void UpdateAll(IQueryAdapter dbClient, int userId, int groupId, int onlineTime, int questId, int respect, int dailyRespectPoints, int dailyPetRespectPoints)
        {
            dbClient.RunQuery("UPDATE `user_stats` SET group_id = '" + groupId + "',  online_time = online_time + '" + onlineTime + "', quest_id = '" + questId + "', Respect = '" + respect + "', daily_respect_points = '" + dailyRespectPoints + "', daily_pet_respect_points = '" + dailyPetRespectPoints + "' WHERE id = '" + userId + "'");
        }

        internal static void UpdateRespectPoint(IQueryAdapter dbClient, int userId, int count)
        {
            dbClient.RunQuery("UPDATE `user_stats` SET daily_respect_points = '" + count + "', daily_pet_respect_points = '" + count + "' WHERE id = '" + userId + "' LIMIT 1");
        }

        internal static void Insert(IQueryAdapter dbClient, int userId)
        {
            dbClient.RunQuery("INSERT INTO `user_stats` (id) VALUES ('" + userId + "')");
        }

        internal static DataRow GetOne(IQueryAdapter dbClient, int userId)
        {
            dbClient.SetQuery("SELECT `id`, `online_time`, `respect`, `respect_given`, `gifts_given`, `gifts_received`, `daily_respect_points`, `daily_pet_respect_points`, `achievement_score`, `quest_id`, `quest_progress`, `lev_builder`, `lev_social`, `lev_identity`, `lev_explore`, `group_id` FROM `user_stats` WHERE id = @id");
            dbClient.AddParameter("id", userId);
            return dbClient.GetRow();
        }
    }
}
