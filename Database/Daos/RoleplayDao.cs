
using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class RoleplayDao
    {
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT owner_id, hopital_id, prison_id FROM roleplay");
            DataTable table1 = dbClient.GetTable();
        }
    }
}