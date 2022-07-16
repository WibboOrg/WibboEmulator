using WibboEmulator.Database.Interfaces;
using System.Data;

namespace WibboEmulator.Database.Daos
{
    class EmulatorPermissionDao
    {
        internal static DataTable GetAll(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT `rank`, `permission` FROM `emulator_permission`");
            return dbClient.GetTable();
        }
    }
}
