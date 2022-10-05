namespace WibboEmulator.Database.Daos;
using System.Data;
using WibboEmulator.Database.Interfaces;

internal class RoleplayItemDao
{
    internal static DataTable GetAll(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT `id`, `name`, `desc`, `price`, `type`, `value`, `allowstack`, `category` FROM `roleplay_item`");
        return dbClient.GetTable();
    }
}