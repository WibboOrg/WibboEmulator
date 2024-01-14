namespace WibboEmulator.Database.Daos.Log;
using System.Data;
using Dapper;

internal sealed class LogChatPubDao
{
    internal static void Insert(IDbConnection dbClient, int userId, string message, string username) => dbClient.Execute(
        "INSERT INTO log_chat_pub (user_id, user_name, timestamp, message) VALUES (@UserId, @Username, UNIX_TIMESTAMP(), @Message)",
        new { UserId = userId, Username = username, Message = message });
}