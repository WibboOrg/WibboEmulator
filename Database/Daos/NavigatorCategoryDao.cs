using Butterfly.Database.Interfaces;
using System.Data;

namespace Butterfly.Database.Daos
{
    class NavigatorCategoryDao
    {
        internal static DataTable GetAll(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT * FROM navigator_categories ORDER BY id ASC");
            return dbClient.GetTable();
        }
    }
}