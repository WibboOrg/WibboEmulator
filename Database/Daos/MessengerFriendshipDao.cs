using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class MessengerFriendshipDao
    {
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT COUNT(0) FROM messenger_friendships WHERE (user_one_id = @userid);");
            dbClient.AddParameter("userid", userID);
            friendCount = dbClient.GetInteger();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("UPDATE messenger_friendships SET relation = '" + Type + "' WHERE user_one_id=@id AND user_two_id=@target LIMIT 1");
            dbClient.AddParameter("id", Session.GetHabbo().Id);
            dbClient.AddParameter("target", User);
            dbClient.RunQuery();
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("REPLACE INTO messenger_friendships (user_one_id,user_two_id) VALUES ('" + this.UserId + "','" + friendID + "')");
            dbClient.RunQuery("REPLACE INTO messenger_friendships (user_one_id,user_two_id) VALUES ('" + friendID + "','" + this.UserId + "')");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE FROM messenger_friendships WHERE (user_one_id = '" + this.UserId + "' AND user_two_id = '" + friendID + "') OR (user_two_id = '" + this.UserId + "' AND user_one_id = '" + friendID + "')");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT user_one_id FROM messenger_friendships WHERE user_one_id = @myID AND user_two_id = @friendID");
            dbClient.AddParameter("myID", this.UserId);
            dbClient.AddParameter("friendID", requestID);
            return dbClient.FindsResult();
        }
    }
}