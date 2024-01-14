namespace WibboEmulator.Database.Daos.Messenger;
using System.Data;
using Dapper;

internal sealed class MessengerOfflineMessageDao
{
    internal static void Insert(IDbConnection dbClient, int toId, int userId, string message) => dbClient.Execute(
        "INSERT INTO messenger_offline_message (to_id, from_id, message, timestamp) VALUES (@ToId, @FromId, @Message, UNIX_TIMESTAMP())",
        new { ToId = toId, FromId = userId, Message = message });

    internal static List<MessengerOfflineMessageEntity> GetAll(IDbConnection dbClient, int userId) => dbClient.Query<MessengerOfflineMessageEntity>(
        "SELECT `id`, `to_id`, `from_id`, `message`, `timestamp` FROM `messenger_offline_message` WHERE `to_id` = @Id",
        new { Id = userId }
    ).ToList();

    internal static void Delete(IDbConnection dbClient, int userId) => dbClient.Execute(
        "DELETE FROM messenger_offline_message WHERE to_id = @UserId",
        new { UserId = userId });
}

public class MessengerOfflineMessageEntity
{
    public int Id { get; set; }
    public int ToId { get; set; }
    public int FromId { get; set; }
    public string Message { get; set; }
    public int Timestamp { get; set; }
}