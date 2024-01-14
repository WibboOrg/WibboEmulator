namespace WibboEmulator.Database.Daos.Log;
using System.Data;
using Dapper;

internal sealed class LogStaffDao
{
    internal static void Insert(IDbConnection dbClient, string name, string action) => dbClient.Execute(
        "INSERT INTO log_staff (pseudo, action, date) VALUES (@Name, @Action, UNIX_TIMESTAMP())",
        new { Name = name, Action = action });
}