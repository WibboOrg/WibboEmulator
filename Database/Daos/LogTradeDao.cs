using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class LogTradeDao
    {
        internal static void Insert(IQueryAdapter dbClient, int oneId, int twoId, string logsOneString, string logsTwoString, int roomId)
        {
            dbClient.SetQuery("INSERT INTO logs_trade (user_one_id, user_two_id, user_one_items, user_two_items, room_id, time) VALUES (@userone, @usertwo, @itemsone, @itemstwo, @roomid, UNIX_TIMESTAMP())");
            dbClient.AddParameter("userone", oneId);
            dbClient.AddParameter("usertwo", twoId);
            dbClient.AddParameter("itemsone", logsOneString);
            dbClient.AddParameter("itemstwo", logsTwoString);
            dbClient.AddParameter("roomid", roomId);
            dbClient.RunQuery();
        }
    }
}