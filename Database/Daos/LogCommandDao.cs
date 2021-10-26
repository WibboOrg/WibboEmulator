using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class LogCommandDao
    {
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("INSERT INTO cmdlogs (user_id, user_name, roomid, command, extra_data, timestamp) VALUES (@userid,@username,@roomid,@type,@desc, UNIX_TIMESTAMP())");
            dbClient.AddParameter("userid", user_id);
            dbClient.AddParameter("username", modName);
            dbClient.AddParameter("roomid", roomid);
            dbClient.AddParameter("target", target);
            dbClient.AddParameter("type", type);
            dbClient.AddParameter("desc", description + " " + target);
            dbClient.RunQuery();
        }
    }
}