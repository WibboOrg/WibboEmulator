using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class ItemLimitedDao
    {
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("INSERT INTO items_limited VALUES (" + Item.Id + "," + LimitedNumber + "," + LimitedStack + ")");
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("INSERT INTO items_limited VALUES (" + ItemId + "," + LimitedNumber + "," + LimitedStack + ")");
        }
    }
}