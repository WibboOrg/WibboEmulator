using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class ModerationPresetDao
    {
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT type,message FROM moderation_presets WHERE enabled = '1'");
            DataTable table = dbClient.GetTable();
        }
    }
}