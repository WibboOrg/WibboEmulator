using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class EmulatorStatsDao
    {
        internal static void Insert(IQueryAdapter dbClient, int usersOnline, int roomsLoaded)
        {
            dbClient.RunQuery("INSERT INTO system_stats (online, time, room) VALUES ('" + usersOnline + "', UNIX_TIMESTAMP(), '" + roomsLoaded + "')");
        }
    }
}