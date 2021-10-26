using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class LogChatDao
    {
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT user_id, user_name, room_id, type, message FROM chatlogs WHERE user_id = '" + UserId + "' ORDER BY id DESC LIMIT 100");
            DataTable table = dbClient.GetTable();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT user_id, user_name, room_id, type, message FROM chatlogs WHERE room_id = '" + RoomId + "' ORDER BY id DESC LIMIT 100");
            table = dbClient.GetTable();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("INSERT INTO chatlogs (user_id, room_id, user_name, timestamp, message, type) VALUES ('" + this.GetHabbo().Id + "', '" + RoomId + "', @username, UNIX_TIMESTAMP(), @message, @type)");
            dbClient.AddParameter("message", Message);
            dbClient.AddParameter("type", type);
            dbClient.AddParameter("username", this.GetHabbo().Username);
            dbClient.RunQuery();
        }
    }
}