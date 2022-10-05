using System.Data;
using WibboEmulator.Database.Interfaces;

namespace WibboEmulator.Database.Daos
{
    class EmulatorTextDao
    {
        internal static DataTable GetAll(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT `identifiant`, `value_fr`, `value_en`, `value_br` FROM `emulator_text`");
            return dbClient.GetTable();
        }
    }
}