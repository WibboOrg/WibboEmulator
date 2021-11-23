using Butterfly.Database.Interfaces;
using System.Data;

namespace Butterfly.Database.Daos
{
    class EmulatorCommandDao
    {
        internal static DataTable GetAll(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT `id`, `input`, `minrank`, `description_fr`, `description_en`, `description_br` FROM `emulator_command`");
            return dbClient.GetTable();
        }
    }
}