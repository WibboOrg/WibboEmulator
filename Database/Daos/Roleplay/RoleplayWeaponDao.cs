using Wibbo.Database.Interfaces;
using System.Data;

namespace Wibbo.Database.Daos
{
    class RoleplayWeaponDao
    {
        internal static DataTable GetAll(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT `id`, `type`, `domage_min`, `domage_max`, `interaction`, `enable`, `freeze_time`, `distance` FROM `roleplay_weapon`");
            return dbClient.GetTable();
        }
    }
}