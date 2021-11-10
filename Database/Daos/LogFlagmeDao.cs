using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class LogFlagmeDao
    {
        internal static void Insert(IQueryAdapter dbClient, int userId, string username, string newUsername)
        {
            dbClient.SetQuery("INSERT INTO logs_flagme (user_id, oldusername, newusername, time) VALUES (@userid, @oldusername, @newusername, '" + ButterflyEnvironment.GetUnixTimestamp() + "');");
            dbClient.AddParameter("userid", userId);
            dbClient.AddParameter("oldusername", username);
            dbClient.AddParameter("newusername", newUsername);
            dbClient.RunQuery();
        }
    }
}