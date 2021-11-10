using Butterfly.Database.Interfaces;
using System.Data;

namespace Butterfly.Database.Daos
{
    class MessengerOfflineMessageDao
    {
        internal static void Insert(IQueryAdapter dbClient, int toId, int userId, string message)
        {
            dbClient.SetQuery("INSERT INTO messenger_offline_messages (to_id, from_id, message, timestamp) VALUES (@tid, @fid, @msg, UNIX_TIMESTAMP())");
            dbClient.AddParameter("tid", toId);
            dbClient.AddParameter("fid", userId);
            dbClient.AddParameter("msg", message);
            dbClient.RunQuery();
        }

        internal static DataTable GetAll(IQueryAdapter dbClient, int userId)
        {
            dbClient.SetQuery("SELECT * FROM messenger_offline_messages WHERE to_id = @id");
            dbClient.AddParameter("id", userId);
            return dbClient.GetTable();
        }

        internal static void Delete(IQueryAdapter dbClient, int userId)
        {
            dbClient.SetQuery("DELETE FROM messenger_offline_messages WHERE to_id = @id");
            dbClient.AddParameter("id", userId);
            dbClient.RunQuery();
        }
    }
}