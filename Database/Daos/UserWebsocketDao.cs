using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class UserWebsocketDao
    {
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT user_id, is_staff, langue FROM user_websocket WHERE auth_ticket = @sso");
            dbClient.AddParameter("sso", AuthTicket);
            DataRow dUserInfo = dbClient.GetRow();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE user_websocket SET auth_ticket = '' WHERE user_id = '" + this.UserId + "'");
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE user_websocket SET auth_ticket = '' WHERE auth_ticket != ''");
        }
    }
}