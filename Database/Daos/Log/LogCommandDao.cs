namespace WibboEmulator.Database.Daos.Log;
using WibboEmulator.Database.Interfaces;

internal sealed class LogCommandDao
{
    internal static void Insert(IQueryAdapter dbClient, int userId, string modName, int roomId, string target, string type, string description)
    {
        dbClient.SetQuery("INSERT INTO `log_command` (user_id, user_name, roomid, command, extra_data, timestamp) VALUES (@userid,@username,@roomid,@type,@desc, UNIX_TIMESTAMP())");
        dbClient.AddParameter("userid", userId);
        dbClient.AddParameter("username", modName);
        dbClient.AddParameter("roomid", roomId);
        dbClient.AddParameter("target", target);
        dbClient.AddParameter("type", type);
        dbClient.AddParameter("desc", description + " " + target);
        dbClient.RunQuery();
    }
}