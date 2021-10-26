using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class ModerationResolutionDao
    {
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT * FROM moderation_resolution");
            DataTable table = dbClient.GetTable();
        }
    }
}