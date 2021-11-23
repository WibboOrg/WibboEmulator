using Butterfly.Database.Interfaces;
using System.Data;

namespace Butterfly.Database.Daos
{
    class EmulatorQuestDao
    {
        internal static DataTable GetAll(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT `id`, `category`, `series_number`, `goal_type`, `goal_data`, `name`, `reward`, `data_bit` FROM `emulator_quest`");

            return dbClient.GetTable();
        }
    }
}