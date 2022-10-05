namespace WibboEmulator.Database.Daos;
using System.Data;
using WibboEmulator.Database.Interfaces;

internal class EmulatorEffectDao
{
    internal static DataTable GetAll(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT `id`, `only_staff` FROM `emulator_effect` ORDER BY `id` ASC");
        return dbClient.GetTable();
    }
}