using Wibbo.Database.Interfaces;
using System.Data;

namespace Wibbo.Database.Daos
{
    class GuildItemDao
    {
        internal static DataTable GetAll(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT `id`, `type`, `firstvalue`, `secondvalue` FROM `guild_item` WHERE `enabled` = '1'");
            return dbClient.GetTable();
        }
    }
}