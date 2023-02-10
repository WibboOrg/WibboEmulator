namespace WibboEmulator.Database.Daos.Messenger;
using WibboEmulator.Database.Interfaces;

internal sealed class MessengerFriendshipDao
{
    internal static int GetCount(IQueryAdapter dbClient, int userId)
    {
        dbClient.SetQuery("SELECT COUNT(0) FROM `messenger_friendship` WHERE (user_one_id = @userid)");
        dbClient.AddParameter("userid", userId);
        return dbClient.GetInteger();
    }

    internal static void UpdateRelation(IQueryAdapter dbClient, int type, int userId, int targetId)
    {
        dbClient.SetQuery("UPDATE `messenger_friendship` SET relation = '" + type + "' WHERE user_one_id = @id AND user_two_id = @target LIMIT 1");
        dbClient.AddParameter("id", userId);
        dbClient.AddParameter("target", targetId);
        dbClient.RunQuery();
    }

    internal static void Replace(IQueryAdapter dbClient, int userId, int friendId) => dbClient.RunQuery("REPLACE INTO `messenger_friendship` (user_one_id,user_two_id) VALUES ('" + userId + "','" + friendId + "')");

    internal static void Delete(IQueryAdapter dbClient, int userId, int friendId) => dbClient.RunQuery("DELETE FROM `messenger_friendship` WHERE (user_one_id = '" + userId + "' AND user_two_id = '" + friendId + "') OR (user_two_id = '" + userId + "' AND user_one_id = '" + friendId + "')");

    internal static bool HaveFriend(IQueryAdapter dbClient, int userId, int requestID)
    {
        dbClient.SetQuery("SELECT user_one_id FROM `messenger_friendship` WHERE user_one_id = @myID AND user_two_id = @friendID");
        dbClient.AddParameter("myID", userId);
        dbClient.AddParameter("friendID", requestID);
        return dbClient.FindsResult();
    }
}
