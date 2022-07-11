using WibboEmulator.Database.Interfaces;
using System.Data;

namespace WibboEmulator.Database.Daos
{
    class ModerationTicketDao
    {
        internal static DataTable GetAll(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT `id`, `score`, `type`, `status`, `sender_id`, `reported_id`, `moderator_id`, `message`, `room_id`, `room_name`, `timestamp` FROM `moderation_ticket` WHERE status = 'open'");
            return dbClient.GetTable();
        }

        internal static int Insert(IQueryAdapter dbClient, string message, string roomname, int category, int userId, int reportedUser, int roomId)
        {
            dbClient.SetQuery("INSERT INTO `moderation_ticket` (score,type,status,sender_id,reported_id,moderator_id,message,room_id,room_name,timestamp) VALUES (1,'" + category + "','open','" + userId + "','" + reportedUser + "','0',@message,'" + roomId + "',@name,'" + WibboEnvironment.GetUnixTimestamp() + "')");
            dbClient.AddParameter("message", message);
            dbClient.AddParameter("name", roomname);
            return Convert.ToInt32(dbClient.InsertQuery());
        }

        internal static void UpdateStatusPicked(IQueryAdapter dbClient, int moderatorId, int id)
        {
            dbClient.RunQuery("UPDATE `moderation_ticket` SET status = 'picked', moderator_id = '" + moderatorId + "', timestamp = '" + WibboEnvironment.GetUnixTimestamp() + "' WHERE id = '" + id + "'");
        }

        internal static void UpdateStatus(IQueryAdapter dbClient, string str, int id)
        {
            dbClient.RunQuery("UPDATE `moderation_ticket` SET status = '" + str + "' WHERE id = '" + id + "'");
        }

        internal static void UpdateStatusOpen(IQueryAdapter dbClient, int id)
        {
            dbClient.RunQuery("UPDATE `moderation_ticket` SET status = 'open' WHERE id = '" + id + "'");
        }

        internal static void UpdateStatusDeleted(IQueryAdapter dbClient, int id)
        {
            dbClient.RunQuery("UPDATE `moderation_ticket` SET status = 'deleted' WHERE id = '" + id + "'");
        }
    }
}