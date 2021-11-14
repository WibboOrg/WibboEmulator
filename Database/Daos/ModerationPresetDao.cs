using Butterfly.Database.Interfaces;
using System.Data;

namespace Butterfly.Database.Daos
{
    class ModerationPresetDao
    {
        internal static DataTable GetAll(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT type, message FROM moderation_presets WHERE enabled = '1'");
            return dbClient.GetTable();
        }
    }
}