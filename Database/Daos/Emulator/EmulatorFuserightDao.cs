using Wibbo.Database.Interfaces;
using System.Data;

namespace Wibbo.Database.Daos
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