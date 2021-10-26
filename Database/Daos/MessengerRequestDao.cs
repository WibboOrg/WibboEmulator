using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class MessengerRequestDao
    {
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE FROM messenger_requests WHERE from_id = '" + this.UserId + "' OR to_id = '" + this.UserId + "'");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE FROM messenger_requests WHERE (from_id = '" + this.UserId + "' AND to_id = '" + sender + "') OR (to_id = '" + this.UserId + "' AND from_id = '" + sender + "')");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("REPLACE INTO messenger_requests (from_id,to_id) VALUES (" + this.UserId + "," + num2 + ")");
        }
    }
}