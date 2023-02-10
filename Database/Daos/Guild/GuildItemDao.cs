namespace WibboEmulator.Database.Daos.Guild;
using System.Data;
using WibboEmulator.Database.Interfaces;

internal sealed class GuildItemDao
{
    internal static DataTable GetAll(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT `id`, `type`, `firstvalue`, `secondvalue` FROM `guild_item` WHERE `enabled` = '1'");
        return dbClient.GetTable();
    }
}