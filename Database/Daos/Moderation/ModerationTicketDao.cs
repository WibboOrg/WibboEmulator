namespace WibboEmulator.Database.Daos.Moderation;
using System.Data;
using Dapper;

internal sealed class ModerationTicketDao
{
    internal static List<ModerationTicketEntity> GetAll(IDbConnection dbClient) => dbClient.Query<ModerationTicketEntity>(
        "SELECT `id`, `score`, `type`, `status`, `sender_id`, `reported_id`, `moderator_id`, `message`, `room_id`, `room_name`, `timestamp` FROM `moderation_ticket` WHERE status = 'open'"
    ).ToList();

    internal static int Insert(IDbConnection dbClient, string message, string roomname, int category, int userId, int reportedUser, int roomId) => dbClient.ExecuteScalar<int>(
        @"INSERT INTO moderation_ticket (score, type, status, sender_id, reported_id, moderator_id, message, room_id, room_name, timestamp) 
        VALUES (1, @Category, 'open', @UserId, @ReportedUser, 0, @Message, @RoomId, @RoomName, UNIX_TIMESTAMP());
        SELECT LAST_INSERT_ID();",
        new { Category = category, UserId = userId, ReportedUser = reportedUser, Message = message, RoomId = roomId, RoomName = roomname });

    internal static void UpdateStatusPicked(IDbConnection dbClient, int moderatorId, int id) => dbClient.Execute(
        "UPDATE `moderation_ticket` SET status = 'picked', moderator_id = '" + moderatorId + "', timestamp = '" + WibboEnvironment.GetUnixTimestamp() + "' WHERE id = '" + id + "'");

    internal static void UpdateStatus(IDbConnection dbClient, string str, int id) => dbClient.Execute(
        "UPDATE `moderation_ticket` SET status = '" + str + "' WHERE id = '" + id + "'");

    internal static void UpdateStatusOpen(IDbConnection dbClient, int id) => dbClient.Execute(
        "UPDATE `moderation_ticket` SET status = 'open' WHERE id = '" + id + "'");

    internal static void UpdateStatusDeleted(IDbConnection dbClient, int id) => dbClient.Execute(
        "UPDATE `moderation_ticket` SET status = 'deleted' WHERE id = '" + id + "'");
}

public class ModerationTicketEntity
{
    public int Id { get; set; }
    public int Score { get; set; }
    public int Type { get; set; }
    public string Status { get; set; }
    public int SenderId { get; set; }
    public int ReportedId { get; set; }
    public int ModeratorId { get; set; }
    public string Message { get; set; }
    public int RoomId { get; set; }
    public string RoomName { get; set; }
    public int Timestamp { get; set; }
}