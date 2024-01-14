namespace WibboEmulator.Database.Daos.Log;
using System.Data;
using Dapper;

internal sealed class LogSlotMachineDao
{
    internal static void Insert(IDbConnection dbClient, int userId, int amount, bool isWin) => dbClient.Execute(
        "INSERT INTO log_slotmachine (user_id, amount, is_win, date) VALUES (@UserId, @Amount, @IsWin, UNIX_TIMESTAMP())",
        new { UserId = userId, Amount = amount, IsWin = isWin ? 1 : 0 });
}