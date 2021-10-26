using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class RoomModelDao
    {
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT id, door_x, door_y, door_z, door_dir, heightmap FROM room_models");
            DataTable table = dbClient.GetTable();
        }
    }
}