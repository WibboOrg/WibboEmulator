using System.Data;
using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class EmulatorQuestDao
    {
        internal static DataTable GetAll(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT id, category, series_number, goal_type, goal_data, name, reward, data_bit FROM quests");

            return dbClient.GetTable();
        }
    }
}