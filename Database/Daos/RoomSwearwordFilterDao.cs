using Butterfly.Database.Interfaces;
using System.Data;

namespace Butterfly.Database.Daos
{
    class RoomSwearwordFilterDao
    {
        internal static DataTable GetAll(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT word FROM `room_swearword_filter`");
            return dbClient.GetTable();
        }
    }
}