namespace WibboEmulator.Database.Daos.Messenger;
using System.Data;
using Dapper;

internal sealed class MessengerRequestDao
{
    internal static void Delete(IDbConnection dbClient, int userId) => dbClient.Execute(
        "DELETE FROM `messenger_request` WHERE from_id = '" + userId + "' OR to_id = '" + userId + "'");

    internal static void Delete(IDbConnection dbClient, int userId, int sender) => dbClient.Execute(
        "DELETE FROM `messenger_request` WHERE (from_id = '" + userId + "' AND to_id = '" + sender + "') OR (to_id = '" + userId + "' AND from_id = '" + sender + "')");

    internal static void Replace(IDbConnection dbClient, int userId, int sender) => dbClient.Execute(
        "REPLACE INTO `messenger_request` (from_id,to_id) VALUES (" + userId + "," + sender + ")");

    internal static List<MessengerRequestEntity> GetAllFriendRequests(IDbConnection dbClient, int userId) => dbClient.Query<MessengerRequestEntity>(
        @"SELECT `messenger_request`.from_id, `messenger_request`.to_id, `user`.username 
        FROM `messenger_request` 
        JOIN `user` ON `user`.id = `messenger_request`.from_id 
        WHERE `messenger_request`.to_id = @UserId",
        new { UserId = userId }
    ).ToList();
}

public class MessengerRequestEntity
{
    public int FromId { get; set; }
    public int ToId { get; set; }
    public string UserName { get; set; }
}