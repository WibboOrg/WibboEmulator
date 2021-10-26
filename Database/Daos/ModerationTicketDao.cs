using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class ModerationTicketDao
    {
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT * FROM moderation_tickets WHERE status = 'open'");
            DataTable table = dbClient.GetTable();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("INSERT INTO moderation_tickets (score,type,status,sender_id,reported_id,moderator_id,message,room_id,room_name,timestamp) VALUES (1,'" + Category + "','open','" + Session.GetHabbo().Id + "','" + ReportedUser + "','0',@message,'" + roomData.Id + "',@name,'" + ButterflyEnvironment.GetUnixTimestamp() + "')");
            dbClient.AddParameter("message", Message);
            dbClient.AddParameter("name", roomname);
            Id = Convert.ToInt32(dbClient.InsertQuery());
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE moderation_tickets SET status = 'picked', moderator_id = '" + pModeratorId + "', timestamp = '" + ButterflyEnvironment.GetUnixTimestamp() + "' WHERE id = '" + this.Id + "'");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE moderation_tickets SET status = '" + str + "' WHERE id = '" + this.Id + "'");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE moderation_tickets SET status = 'open' WHERE id = '" + this.Id + "'");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE moderation_tickets SET status = 'deleted' WHERE id = '" + this.Id + "'");
        }
    }
}