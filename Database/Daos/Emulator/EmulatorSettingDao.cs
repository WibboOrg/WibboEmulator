namespace WibboEmulator.Database.Daos.Emulator;
using System.Data;
using WibboEmulator.Database.Interfaces;

internal class EmulatorSettingDao
{
    internal static DataTable GetAll(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT `key`, `value` FROM `emulator_setting`");
        return dbClient.GetTable();
    }
}