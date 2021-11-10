using Butterfly.Database.Interfaces;
using System.Data;

namespace Butterfly.Database.Daos
{
    class UserFavoriteDao
    {
        internal static void Insert(IQueryAdapter dbClient, int userId, int roomId)
        {
            dbClient.RunQuery("INSERT INTO user_favorites (user_id,room_id) VALUES ('" + userId + "','" + roomId + "')");
        }

        internal static void Delete(IQueryAdapter dbClient, int userId, int roomId)
        {
            dbClient.RunQuery("DELETE FROM user_favorites WHERE user_id = '" + userId + "' AND room_id = '" + roomId + "'");
        }

        internal static void Delete(IQueryAdapter dbClient, int roomId)
        {
            dbClient.RunQuery("DELETE FROM user_favorites WHERE room_id = '" + roomId + "'");
        }

        internal static DataTable GetAll(IQueryAdapter dbClient, int userId)
        {
            dbClient.SetQuery("SELECT room_id FROM user_favorites WHERE user_id = '" + userId + "'");
            return dbClient.GetTable();
        }
    }
}