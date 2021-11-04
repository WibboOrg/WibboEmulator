using System.Data;
using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class EmulatorAchievementDao
    {
        internal static DataTable GetAll(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT id, category, group_name, level, reward_pixels, reward_points, progress_needed FROM achievements");
            return dbClient.GetTable();
        }
    }
}