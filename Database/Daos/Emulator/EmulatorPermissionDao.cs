namespace WibboEmulator.Database.Daos.Emulator;
using System.Data;
using WibboEmulator.Database.Interfaces;

internal class EmulatorPermissionDao
{
    internal static DataTable GetAll(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT `rank`, `permission` FROM `emulator_permission`");
        return dbClient.GetTable();
    }
}
