namespace WibboEmulator.Database.Daos.Log;
using System.Data;
using Dapper;

internal sealed class LogCommandDao
{
    internal static void Insert(IDbConnection dbClient, int userId, string modName, int roomId, string target, string type, string description) => dbClient.Execute(
        "INSERT INTO log_command (user_id, user_name, roomid, command, extra_data, timestamp) VALUES (@UserId, @Username, @RoomId, @Type, @Description, UNIX_TIMESTAMP())",
        new { UserId = userId, Username = modName, RoomId = roomId, Type = type, Description = description + " " + target });
}