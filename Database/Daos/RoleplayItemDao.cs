using Butterfly.Database.Interfaces;
using System.Data;

namespace Butterfly.Database.Daos
{
    class RoleplayItemDao
    {
        internal static DataTable GetAll(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT id, name, desc, price, type, value, allowstack, category FROM roleplay_items");
            return dbClient.GetTable();
        }
    }
}