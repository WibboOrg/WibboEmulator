using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class LogChatPubDao
    {
        internal static void Insert(IQueryAdapter dbClient, int userId, string message, string username)
        {
            dbClient.SetQuery("INSERT INTO chatlogs_pub (user_id, user_name, timestamp, message) VALUES ('" + userId + "', @username, UNIX_TIMESTAMP(), @message)");
            dbClient.AddParameter("message", message);
            dbClient.AddParameter("username", username);
            dbClient.RunQuery();
        }
    }
}