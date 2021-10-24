using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class UserDao
    {
        internal static int GetIdByName(IQueryAdapter dbClient, string name)
        {
            dbClient.SetQuery("SELECT id FROM users WHERE username = @username LIMIT 1");
            dbClient.AddParameter("username", name);

            return dbClient.GetInteger();
        }

        internal static string GetNameById(IQueryAdapter dbClient, int userId)
        {
            dbClient.SetQuery("SELECT `username` FROM `users` WHERE `id` = @id LIMIT 1");
            dbClient.AddParameter("id", userId);

            return dbClient.GetString();
        }
    }
}
