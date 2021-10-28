using System.Data;
using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class LogChatDao
    {
        internal static DataTable GetAllByUserId(IQueryAdapter dbClient, int userId)
        {
            dbClient.SetQuery("SELECT user_id, user_name, room_id, type, message FROM chatlogs WHERE user_id = '" + userId + "' ORDER BY id DESC LIMIT 100");
            return dbClient.GetTable();
        }

        internal static DataTable GetAllByRoomId(IQueryAdapter dbClient, int roomId)
        {
            dbClient.SetQuery("SELECT user_id, user_name, room_id, type, message FROM chatlogs WHERE room_id = '" + roomId + "' ORDER BY id DESC LIMIT 100");
            return dbClient.GetTable();
        }

        internal static void Insert(IQueryAdapter dbClient, int userId, int roomId, string message, int type, string username)
        {
            dbClient.SetQuery("INSERT INTO chatlogs (user_id, room_id, user_name, timestamp, message, type) VALUES ('" + userId + "', '" + roomId + "', @username, UNIX_TIMESTAMP(), @message, @type)");
            dbClient.AddParameter("message", message);
            dbClient.AddParameter("type", type);
            dbClient.AddParameter("username", username);
            dbClient.RunQuery();
        }
    }
}