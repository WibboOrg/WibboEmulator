using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class RoleplayWeaponDao
    {
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT * FROM roleplay_weapon");
            DataTable table1 = dbClient.GetTable();
        }
    }
}