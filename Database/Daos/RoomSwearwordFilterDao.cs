using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class RoomSwearwordFilterDao
    {
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT word FROM room_swearword_filter");
            DataTable Data = dbClient.GetTable();
        }
    }
}