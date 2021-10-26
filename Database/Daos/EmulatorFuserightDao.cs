using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class EmulatorFuserightDao
    {
        internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT fuse, rank FROM fuserights");
        DataTable table1 = dbClient.GetTable();
    }
    }
}