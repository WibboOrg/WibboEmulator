using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class RoomModelCustomDao
    {
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("REPLACE INTO room_models_customs VALUES (@id, @doorX, @doorY, @doorZ, @doorDir, @heightmap, @murheight)");
            dbClient.AddParameter("id", Room.Id);
            dbClient.AddParameter("doorX", DoorX);
            dbClient.AddParameter("doorY", DoorY);
            dbClient.AddParameter("doorZ", DoorZ);
            dbClient.AddParameter("doorDir", DoorDirection);
            dbClient.AddParameter("heightmap", Map);
            dbClient.AddParameter("murheight", WallHeight);
            dbClient.RunQuery();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("INSERT INTO room_models_customs (room_id, door_x, door_y, door_z, door_dir, heightmap, wall_height) " +
                "SELECT '" + RoomId + "', door_x, door_y, door_z, door_dir, heightmap, wall_height FROM room_models_customs WHERE room_id = '" + OldRoomId + "'");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("REPLACE INTO room_models_customs VALUES (@id, @doorX, @doorY, @doorZ, @doorDir, @heightmap, @murheight)");
            dbClient.AddParameter("id", Room.Id);
            dbClient.AddParameter("doorX", Room.GetGameMap().Model.DoorX);
            dbClient.AddParameter("doorY", Room.GetGameMap().Model.DoorY);
            dbClient.AddParameter("doorZ", Room.GetGameMap().Model.DoorZ);
            dbClient.AddParameter("doorDir", Room.GetGameMap().Model.DoorOrientation);
            dbClient.AddParameter("heightmap", Map);
            dbClient.AddParameter("murheight", Room.GetGameMap().Model.MurHeight);
            dbClient.RunQuery();
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("REPLACE INTO room_models_customs VALUES (@id, @doorX, @doorY, @doorZ, @doorDir, @heightmap, @murheight)");
            dbClient.AddParameter("id", Room.Id);
            dbClient.AddParameter("doorX", Room.GetGameMap().Model.DoorX);
            dbClient.AddParameter("doorY", Room.GetGameMap().Model.DoorY);
            dbClient.AddParameter("doorZ", Room.GetGameMap().Model.DoorZ);
            dbClient.AddParameter("doorDir", Room.GetGameMap().Model.DoorOrientation);
            dbClient.AddParameter("heightmap", Map);
            dbClient.AddParameter("murheight", Room.GetGameMap().Model.MurHeight);
            dbClient.RunQuery();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT door_x, door_y, door_z, door_dir, heightmap, wall_height FROM room_models_customs WHERE room_id = '" + roomID + "'");
            row = dbClient.GetRow();
        }
    }
}