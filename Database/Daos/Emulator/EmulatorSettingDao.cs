using WibboEmulator.Database.Interfaces;
using System.Data;

namespace WibboEmulator.Database.Daos
{
    class EmulatorSettingDao
    {
        internal static DataTable GetAll(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT `key`, `value` FROM `emulator_setting`");
            return dbClient.GetTable();
        }
    }
}