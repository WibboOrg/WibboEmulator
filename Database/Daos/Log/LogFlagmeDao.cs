namespace WibboEmulator.Database.Daos.Log;
using System.Data;
using Dapper;
internal sealed class LogFlagmeDao
{
    internal static void Insert(IDbConnection dbClient, int userId, string username, string newUsername) => dbClient.Execute(
        "INSERT INTO log_flagme (user_id, oldusername, newusername, time) VALUES (@UserId, @OldUsername, @NewUsername, UNIX_TIMESTAMP())",
        new { UserId = userId, OldUsername = username, NewUsername = newUsername });
}