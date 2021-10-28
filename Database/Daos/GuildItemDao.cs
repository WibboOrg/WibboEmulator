using System.Data;
using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class GuildItemDao
    {
        internal static DataTable GetAll(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT id, type, firstvalue, secondvalue FROM groups_items WHERE enabled = '1'");
            return dbClient.GetTable();
        }
    }
}