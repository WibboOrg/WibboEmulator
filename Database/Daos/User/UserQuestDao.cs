namespace WibboEmulator.Database.Daos.User;

using System.Data;
using Dapper;

internal sealed class UserQuestDao
{
    internal static void Update(IDbConnection dbClient, int userId, int questId, int progress) => dbClient.Execute(
        "UPDATE `user_quest` SET `progress` = '" + progress + "' WHERE `user_id` = '" + userId + "' AND `quest_id` = '" + questId + "'");

    internal static void Replace(IDbConnection dbClient, int userId, int questId) => dbClient.Execute(
        "REPLACE INTO `user_quest` VALUES (" + userId + ", " + questId + ", 0)");

    internal static void Delete(IDbConnection dbClient, int userId, int questId) => dbClient.Execute(
        "DELETE FROM `user_quest` WHERE `user_id` = '" + userId + "' AND `quest_id` = '" + questId + "'");

    internal static List<UserQuestEntity> GetAll(IDbConnection dbClient, int userId) => dbClient.Query<UserQuestEntity>(
        "SELECT `user_id`, `quest_id`, `progress` FROM `user_quest` WHERE `user_id` = '" + userId + "'").ToList();
}

public class UserQuestEntity
{
    public int UserId { get; set; }
    public int QuestId { get; set; }
    public int Progress { get; set; }
}
