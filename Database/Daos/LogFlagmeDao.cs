using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class LogFlagmeDao
    {
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("INSERT INTO logs_flagme (user_id, oldusername, newusername, time) VALUES (@userid, @oldusername, @newusername, '" + ButterflyEnvironment.GetUnixTimestamp() + "');");
            dbClient.AddParameter("userid", Session.GetHabbo().Id);
            dbClient.AddParameter("oldusername", Session.GetHabbo().Username);
            dbClient.AddParameter("newusername", NewUsername);
            dbClient.RunQuery();
        }
    }
}