using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class EmulatorQuestDao
    {
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT id, category, series_number, goal_type, goal_data, name, reward, data_bit FROM quests");
        }
    }
}