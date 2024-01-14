namespace WibboEmulator.Database.Daos.Log;
using System.Data;
using Dapper;

internal sealed class LogTradeDao
{
    internal static void Insert(IDbConnection dbClient, int oneId, int twoId, string logsOneString, string logsTwoString, int roomId) => dbClient.Execute(
        "INSERT INTO log_trade (user_one_id, user_two_id, user_one_items, user_two_items, room_id, time) VALUES (@UserOne, @UserTwo, @ItemsOne, @ItemsTwo, @RoomId, UNIX_TIMESTAMP())",
        new { UserOne = oneId, UserTwo = twoId, ItemsOne = logsOneString, ItemsTwo = logsTwoString, RoomId = roomId });
}