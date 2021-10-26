using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class NavigatorCategoryDao
    {
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT * FROM navigator_categories ORDER BY id ASC");
            Table = dbClient.GetTable();
        }
    }
}