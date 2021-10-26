using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class MessengerOfflineMessageDao
    {
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("INSERT INTO messenger_offline_messages (to_id, from_id, message, timestamp) VALUES (@tid, @fid, @msg, UNIX_TIMESTAMP())");
            dbClient.AddParameter("tid", ToId);
            dbClient.AddParameter("fid", this.GetClient().GetHabbo().Id);
            dbClient.AddParameter("msg", Message);
            dbClient.RunQuery();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT * FROM messenger_offline_messages WHERE to_id = @id");
            dbClient.AddParameter("id", this.UserId);
            GetMessages = dbClient.GetTable();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("DELETE FROM messenger_offline_messages WHERE to_id = @id");
            dbClient.AddParameter("id", this.UserId);
            dbClient.RunQuery();
        }
    }
}