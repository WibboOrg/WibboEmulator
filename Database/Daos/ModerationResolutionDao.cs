using Butterfly.Database.Interfaces;
using System.Data;

namespace Butterfly.Database.Daos
{
    class ModerationResolutionDao
    {
        internal static DataTable GetAll(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT `id`, `type`, `title`, `subtitle`, `ban_hours`, `enable_mute`, `mute_hours`, `reminder`, `message` FROM `moderation_resolution`");
            return dbClient.GetTable();
        }
    }
}