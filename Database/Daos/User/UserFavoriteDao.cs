using Wibbo.Database.Interfaces;
using System.Data;

namespace Wibbo.Database.Daos
{
    class UserFavoriteDao
    {
        internal static void Insert(IQueryAdapter dbClient, int userId, int roomId)
        {
            dbClient.RunQuery("INSERT INTO `user_favorite` (`user_id`, `room_id`) VALUES ('" + userId + "','" + roomId + "')");
        }

        internal static void Delete(IQueryAdapter dbClient, int userId, int roomId)
        {
            dbClient.RunQuery("DELETE FROM `user_favorite` WHERE `user_id` = '" + userId + "' AND `room_id` = '" + roomId + "'");
        }

        internal static void Delete(IQueryAdapter dbClient, int roomId)
        {
            dbClient.RunQuery("DELETE FROM `user_favorite` WHERE `room_id` = '" + roomId + "'");
        }

        internal static DataTable GetAll(IQueryAdapter dbClient, int userId)
        {
            dbClient.SetQuery("SELECT `room_id` FROM `user_favorite` WHERE `user_id` = '" + userId + "'");
            return dbClient.GetTable();
        }
    }
}