using WibboEmulator.Database.Interfaces;
using System.Data;

namespace WibboEmulator.Database.Daos
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