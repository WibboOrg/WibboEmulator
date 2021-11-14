using Butterfly.Database.Interfaces;
using System.Data;

namespace Butterfly.Database.Daos
{
    class EmulatorFuserightDao
    {
        internal static DataTable GetAll(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT `fuse`, `rank` FROM `emulator_fuseright`");
            return dbClient.GetTable();
        }
    }
}