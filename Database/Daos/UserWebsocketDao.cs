using Butterfly.Database.Interfaces;
using System.Data;

namespace Butterfly.Database.Daos
{
    class UserWebsocketDao
    {
        internal static DataRow GetOne(IQueryAdapter dbClient, string authTicket)
        {
            dbClient.SetQuery("SELECT user_id, is_staff, langue FROM `user_websocket` WHERE auth_ticket = @sso");
            dbClient.AddParameter("sso", authTicket);
            return dbClient.GetRow();
        }

        internal static void UpdateTicket(IQueryAdapter dbClient, int userId)
        {
            dbClient.RunQuery("UPDATE `user_websocket` SET auth_ticket = '' WHERE user_id = '" + userId + "'");
        }

        internal static void UpdateReset(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE `user_websocket` SET auth_ticket = '' WHERE auth_ticket != ''");
        }
    }
}