using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class GuildItemDao
    {
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT id, type, firstvalue, secondvalue FROM groups_items WHERE enabled = '1'");
            DataTable dItems = dbClient.GetTable();
        }
    }
}