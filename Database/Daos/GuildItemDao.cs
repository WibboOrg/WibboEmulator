using Butterfly.Database.Interfaces;
using System.Data;

namespace Butterfly.Database.Daos
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