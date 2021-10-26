 using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class EmulatorAchievementDao
    {
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT id, category, group_name, level, reward_pixels, reward_points, progress_needed FROM achievements");
        }
    }
}