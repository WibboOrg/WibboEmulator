using System.Data;
using WibboEmulator.Database.Interfaces;

namespace WibboEmulator.Database.Daos
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