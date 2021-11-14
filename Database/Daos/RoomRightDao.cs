using Butterfly.Database.Interfaces;
using System.Data;

namespace Butterfly.Database.Daos
{
    class RoomRightDao
    {
        internal static void Insert(IQueryAdapter dbClient, int roomId, int userId)
        {
            dbClient.RunQuery("INSERT INTO `room_right` (room_id, user_id) VALUES ('" + roomId + "', '" + userId + "')");
        }

        internal static void Delete(IQueryAdapter dbClient, int roomId)
        {
            dbClient.RunQuery("DELETE FROM `room_right` WHERE room_id = '" + roomId + "'");
        }

        internal static void Delete(IQueryAdapter dbClient, int roomId, int userId)
        {
            dbClient.SetQuery("DELETE FROM `room_right` WHERE user_id = @uid AND room_id = @rid LIMIT 1");
            dbClient.AddParameter("uid", userId);
            dbClient.AddParameter("rid", roomId);
            dbClient.RunQuery();
        }

        internal static DataTable GetAllByRoomId(IQueryAdapter dbClient, int roomId)
        {
            dbClient.SetQuery("SELECT user_id FROM `room_right` WHERE room_id = '" + roomId + "'");
            return dbClient.GetTable();
        }

        internal static DataTable GetAllByUserId(IQueryAdapter dbClient, int userId)
        {
            dbClient.SetQuery("SELECT room_id FROM `room_right` WHERE user_id = '" + userId + "'");
            return dbClient.GetTable();
        }
    }
}