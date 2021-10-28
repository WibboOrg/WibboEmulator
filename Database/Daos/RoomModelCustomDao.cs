using System.Data;
using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class RoomModelCustomDao
    {
        internal static void Replace(IQueryAdapter dbClient, int roomId, int doorX, int doorY, double doorZ, int doorDirection, string map, int wallHeight)
        {
            dbClient.SetQuery("REPLACE INTO room_models_customs VALUES (@id, @doorX, @doorY, @doorZ, @doorDir, @heightmap, @murheight)");
            dbClient.AddParameter("id", roomId);
            dbClient.AddParameter("doorX", doorX);
            dbClient.AddParameter("doorY", doorY);
            dbClient.AddParameter("doorZ", doorZ);
            dbClient.AddParameter("doorDir", doorDirection);
            dbClient.AddParameter("heightmap", map);
            dbClient.AddParameter("murheight", wallHeight);
            dbClient.RunQuery();
        }

        internal static void InsertDuplicate(IQueryAdapter dbClient, int roomId, int oldRoomId)
        {
            dbClient.RunQuery("INSERT INTO room_models_customs (room_id, door_x, door_y, door_z, door_dir, heightmap, wall_height) " +
                "SELECT '" + roomId + "', door_x, door_y, door_z, door_dir, heightmap, wall_height FROM room_models_customs WHERE room_id = '" + oldRoomId + "'");
        }
        
        internal static DataRow GetOne(IQueryAdapter dbClient, int roomId)
        {
            dbClient.SetQuery("SELECT door_x, door_y, door_z, door_dir, heightmap, wall_height FROM room_models_customs WHERE room_id = '" + roomId + "'");
            return dbClient.GetRow();
        }
    }
}