namespace WibboEmulator.Database.Daos.Log;
using System.Data;
using Dapper;

internal sealed class LogChatDao
{
    internal static List<LogChatEntity> GetAllByUserId(IDbConnection dbClient, int userId) => dbClient.Query<LogChatEntity>(
        "SELECT user_id, user_name, room_id, type, timestamp, message FROM `log_chat` WHERE user_id = '" + userId + "' ORDER BY id DESC LIMIT 100"
    ).ToList();

    internal static List<LogChatEntity> GetAllByRoomId(IDbConnection dbClient, int roomId) => dbClient.Query<LogChatEntity>(
        "SELECT user_id, user_name, room_id, type, timestamp, message FROM `log_chat` WHERE room_id = '" + roomId + "' ORDER BY id DESC LIMIT 100"
    ).ToList();

    internal static void Insert(IDbConnection dbClient, int userId, int roomId, string message, string type, string username) => dbClient.Execute(
        "INSERT INTO log_chat (user_id, room_id, user_name, timestamp, message, type) VALUES (@UserId, @RoomId, @Username, UNIX_TIMESTAMP(), @Message, @Type)",
        new { UserId = userId, RoomId = roomId, Username = username, Message = message, Type = type });
}

public class LogChatEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int RoomId { get; set; }
    public string UserName { get; set; }
    public int Timestamp { get; set; }
    public string Message { get; set; }
    public string Type { get; set; }
}
