using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class EmulatorStatusDao
    {
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT userpeak FROM server_status");
            UserPeak = dbClient.GetInteger();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE server_status SET users_online = '" + UsersOnline + "', rooms_loaded = '" + RoomsLoaded + "', userpeak = '" + UserPeak + "', stamp = UNIX_TIMESTAMP()");
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE server_status SET status = '1', users_online = '0', rooms_loaded = '0', stamp = '" + ButterflyEnvironment.GetUnixTimestamp() + "'");
        }
    }
}