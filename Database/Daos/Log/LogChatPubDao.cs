namespace WibboEmulator.Database.Daos.Log;
using WibboEmulator.Database.Interfaces;

internal sealed class LogChatPubDao
{
    internal static void Insert(IQueryAdapter dbClient, int userId, string message, string username)
    {
        dbClient.SetQuery("INSERT INTO `log_chat_pub` (user_id, user_name, timestamp, message) VALUES ('" + userId + "', @username, UNIX_TIMESTAMP(), @message)");
        dbClient.AddParameter("message", message);
        dbClient.AddParameter("username", username);
        dbClient.RunQuery();
    }
}