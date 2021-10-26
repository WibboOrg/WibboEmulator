using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class LogTradeDao
    {
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("INSERT INTO logs_trade (user_one_id, user_two_id, user_one_items, user_two_items, room_id, time) VALUES (@userone, @usertwo, @itemsone, @itemstwo, @roomid, UNIX_TIMESTAMP())");
            dbClient.AddParameter("userone", this.oneId);
            dbClient.AddParameter("usertwo", this.twoId);
            dbClient.AddParameter("itemsone", LogsOneString);
            dbClient.AddParameter("itemstwo", LogsTwoString);
            dbClient.AddParameter("roomid", this.RoomId);
            dbClient.RunQuery();
        }
    }
}