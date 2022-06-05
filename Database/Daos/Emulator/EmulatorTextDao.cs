using Wibbo.Database.Interfaces;
using System.Data;

namespace Wibbo.Database.Daos
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