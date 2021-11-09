using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class MessengerRequestDao
    {
        internal static void Delete(IQueryAdapter dbClient, int userId)
        {
            dbClient.RunQuery("DELETE FROM messenger_requests WHERE from_id = '" + userId + "' OR to_id = '" + userId + "'");
        }

        internal static void Delete(IQueryAdapter dbClient, int userId, int sender)
        {
            dbClient.RunQuery("DELETE FROM messenger_requests WHERE (from_id = '" + userId + "' AND to_id = '" + sender + "') OR (to_id = '" + userId + "' AND from_id = '" + sender + "')");
        }

        internal static void Replace(IQueryAdapter dbClient, int userId, int sender)
        {
            dbClient.RunQuery("REPLACE INTO messenger_requests (from_id,to_id) VALUES (" + userId + "," + sender + ")");
        }
    }
}