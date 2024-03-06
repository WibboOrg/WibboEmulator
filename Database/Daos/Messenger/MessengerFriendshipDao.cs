namespace WibboEmulator.Database.Daos.Messenger;
using System.Data;
using Dapper;

internal sealed class MessengerFriendshipDao
{
    internal static int GetCount(IDbConnection dbClient, int userId) => dbClient.ExecuteScalar<int>(
        "SELECT COUNT(0) FROM messenger_friendship WHERE user_one_id = @UserId",
        new { UserId = userId });

    internal static void UpdateRelation(IDbConnection dbClient, int type, int userId, int targetId) => dbClient.Execute(
        "UPDATE messenger_friendship SET relation = @Type WHERE user_one_id = @UserId AND user_two_id = @TargetId LIMIT 1",
        new { Type = type, UserId = userId, TargetId = targetId });

    internal static void Replace(IDbConnection dbClient, int userId, int friendId) => dbClient.Execute(
        "REPLACE INTO `messenger_friendship` (user_one_id,user_two_id) VALUES ('" + userId + "','" + friendId + "')");

    internal static void Delete(IDbConnection dbClient, int userId, int friendId) => dbClient.Execute(
        "DELETE FROM `messenger_friendship` WHERE (user_one_id = '" + userId + "' AND user_two_id = '" + friendId + "') OR (user_two_id = '" + userId + "' AND user_one_id = '" + friendId + "')");

    internal static bool HaveFriend(IDbConnection dbClient, int userId, int requestID) => dbClient.ExecuteScalar<int>(
        "SELECT user_one_id FROM messenger_friendship WHERE user_one_id = @MyId AND user_two_id = @FriendId",
        new { MyId = userId, FriendId = requestID }) != 0;

    internal static List<MessengerFriendshipEntity> GetAllFriendShips(IDbConnection dbClient, int userId) => dbClient.Query<MessengerFriendshipEntity>(
        @"SELECT `user`.id, `user`.username, `messenger_friendship`.relation
        FROM `messenger_friendship`
        JOIN `user` ON `user`.id = `messenger_friendship`.user_two_id
        WHERE `messenger_friendship`.user_one_id = @UserId",
        new { UserId = userId }
    ).ToList();

    internal static List<MessengerFriendshipEntity> GetAllFriendRelations(IDbConnection dbClient, int userId) => dbClient.Query<MessengerFriendshipEntity>(
        @"SELECT `user`.id, `user`.username, `messenger_friendship`.relation
        FROM `messenger_friendship`
        JOIN `user` ON `user`.id = `messenger_friendship`.user_two_id
        WHERE `messenger_friendship`.user_one_id = @UserId AND `messenger_friendship`.relation != 0",
        new { UserId = userId }
    ).ToList();
}

public class MessengerFriendshipEntity
{
    public int Id { get; set; }
    public int UserOneId { get; set; }
    public int UserTwoId { get; set; }
    public int Relation { get; set; }
    public string UserName { get; set; }
}
