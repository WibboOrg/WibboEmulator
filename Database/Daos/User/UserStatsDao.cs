namespace WibboEmulator.Database.Daos.User;

using System.Data;
using Dapper;

internal sealed class UserStatsDao
{
    internal static void UpdateRemoveAllGroupId(IDbConnection dbClient, int groupId) => dbClient.Execute(
        "UPDATE `user_stats` SET `group_id` = '0' WHERE `group_id` = '" + groupId + "' LIMIT 1");

    internal static void UpdateRemoveGroupId(IDbConnection dbClient, int userId) => dbClient.Execute(
        "UPDATE user_stats SET group_id = '0' WHERE id = @UserId LIMIT 1", new { UserId = userId });

    internal static void UpdateGroupId(IDbConnection dbClient, int groupId, int userId) => dbClient.Execute(
        "UPDATE user_stats SET group_id = @GroupId WHERE id = @UserId LIMIT 1",
        new { GroupId = groupId, UserId = userId });

    internal static void UpdateAchievementScore(IDbConnection dbClient, int userId, int score) => dbClient.Execute(
        "UPDATE `user_stats` SET `achievement_score` = `achievement_score` + '" + score + "' WHERE `id` = '" + userId + "'");

    internal static void UpdateAll(IDbConnection dbClient, int userId, int groupId, int onlineTime, int questId, int respect, int dailyRespectPoints, int dailyPetRespectPoints) => dbClient.Execute(
        @"UPDATE user_stats
        SET group_id = @GroupId,
        online_time = online_time + @OnlineTime,
        quest_id = @QuestId,
        respect = @Respect,
        daily_respect_points = @DailyRespectPoints, 
        daily_pet_respect_points = @DailyPetRespectPoints 
        WHERE id = @Id",
        new
        {
            Id = userId,
            GroupId = groupId,
            OnlineTime = onlineTime,
            QuestId = questId,
            Respect = respect,
            DailyRespectPoints = dailyRespectPoints,
            DailyPetRespectPoints = dailyPetRespectPoints
        });

    internal static void UpdateRespectPoint(IDbConnection dbClient, int userId, int count) => dbClient.Execute(
        "UPDATE `user_stats` SET `daily_respect_points` = '" + count + "', `daily_pet_respect_points` = '" + count + "' WHERE `id` = '" + userId + "' LIMIT 1");

    internal static void Insert(IDbConnection dbClient, int userId) => dbClient.Execute(
        "INSERT INTO `user_stats` (`id`) VALUES ('" + userId + "')");

    internal static UserStatsEntity GetOne(IDbConnection dbClient, int userId) => dbClient.QuerySingleOrDefault<UserStatsEntity>(
        "SELECT `id`, `online_time`, `respect`, `respect_given`, `gifts_given`, `gifts_received`, `daily_respect_points`, `daily_pet_respect_points`, `achievement_score`, `quest_id`, `quest_progress`, `lev_builder`, `lev_social`, `lev_identity`, `lev_explore`, `group_id` FROM `user_stats` WHERE id = @Id", new { Id = userId });
}

public class UserStatsEntity
{
    public int Id { get; set; }
    public int OnlineTime { get; set; }
    public int Respect { get; set; }
    public int RespectGiven { get; set; }
    public int GiftsGiven { get; set; }
    public int GiftsReceived { get; set; }
    public int DailyRespectPoints { get; set; }
    public int DailyPetRespectPoints { get; set; }
    public int AchievementScore { get; set; }
    public int QuestId { get; set; }
    public int QuestProgress { get; set; }
    public int LevBuilder { get; set; }
    public int LevSocial { get; set; }
    public int LevIdentity { get; set; }
    public int LevExplore { get; set; }
    public int GroupId { get; set; }
}