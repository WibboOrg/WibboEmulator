using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class EmulatorStatsDao
    {
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("INSERT INTO system_stats (online, time, room) VALUES ('" + UsersOnline + "', UNIX_TIMESTAMP(), '" + RoomsLoaded + "')");
        }
    }
}