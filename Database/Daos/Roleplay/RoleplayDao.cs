
using System.Data;
using WibboEmulator.Database.Interfaces;

namespace WibboEmulator.Database.Daos
{
    class RoleplayDao
    {
        internal static DataTable GetAll(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT owner_id, hopital_id, prison_id FROM `roleplay`");
            return dbClient.GetTable();
        }
    }
}