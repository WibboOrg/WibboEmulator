
using Wibbo.Database.Interfaces;
using System.Data;

namespace Wibbo.Database.Daos
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