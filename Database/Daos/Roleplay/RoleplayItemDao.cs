using Wibbo.Database.Interfaces;
using System.Data;

namespace Wibbo.Database.Daos
{
    class RoleplayItemDao
    {
        internal static DataTable GetAll(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT `id`, `name`, `desc`, `price`, `type`, `value`, `allowstack`, `category` FROM `roleplay_item`");
            return dbClient.GetTable();
        }
    }
}