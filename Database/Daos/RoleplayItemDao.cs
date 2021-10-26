using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class RoleplayItemDao
    {
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT id, name, desc, price, type, value, allowstack, category FROM roleplay_items");
            DataTable table1 = dbClient.GetTable();
        }
    }
}