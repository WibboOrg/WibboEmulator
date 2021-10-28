using System;
using System.Data;
using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class ModerationTicketDao
    {
        internal static DataTable GetAll(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT * FROM moderation_tickets WHERE status = 'open'");
            return dbClient.GetTable();
        }

        internal static int Insert(IQueryAdapter dbClient, string message, string roomname, string category, int userId, int reportedUser, int roomId)
        {
            dbClient.SetQuery("INSERT INTO moderation_tickets (score,type,status,sender_id,reported_id,moderator_id,message,room_id,room_name,timestamp) VALUES (1,'" + category + "','open','" + userId + "','" + reportedUser + "','0',@message,'" + roomId + "',@name,'" + ButterflyEnvironment.GetUnixTimestamp() + "')");
            dbClient.AddParameter("message", message);
            dbClient.AddParameter("name", roomname);
            return Convert.ToInt32(dbClient.InsertQuery());
        }

        internal static void UpdateStatusPicked(IQueryAdapter dbClient, int moderatorId, int id)
        {
            dbClient.RunQuery("UPDATE moderation_tickets SET status = 'picked', moderator_id = '" + moderatorId + "', timestamp = '" + ButterflyEnvironment.GetUnixTimestamp() + "' WHERE id = '" + id + "'");
        }

        internal static void UpdateStatus(IQueryAdapter dbClient, string str, int id)
        {
            dbClient.RunQuery("UPDATE moderation_tickets SET status = '" + str + "' WHERE id = '" + id + "'");
        }

        internal static void UpdateStatusOpen(IQueryAdapter dbClient, int id)
        {
            dbClient.RunQuery("UPDATE moderation_tickets SET status = 'open' WHERE id = '" + id + "'");
        }

        internal static void UpdateStatusDeleted(IQueryAdapter dbClient, int id)
        {
            dbClient.RunQuery("UPDATE moderation_tickets SET status = 'deleted' WHERE id = '" + id + "'");
        }
    }
}