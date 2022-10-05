using System.Data;
using WibboEmulator.Database.Interfaces;

namespace WibboEmulator.Database.Daos
{
    class EmulatorEffectDao
    {
        internal static DataTable GetAll(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT `id`, `only_staff` FROM `emulator_effect` ORDER BY `id` ASC");
            return dbClient.GetTable();
        }
    }
}