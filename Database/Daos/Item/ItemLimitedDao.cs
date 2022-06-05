using Wibbo.Database.Interfaces;

namespace Wibbo.Database.Daos
{
    class ItemLimitedDao
    {
        internal static void Insert(IQueryAdapter dbClient, int itemId, int limitedNumber, int limitedStack)
        {
            dbClient.RunQuery("INSERT INTO `item_limited` VALUES (" + itemId + "," + limitedNumber + "," + limitedStack + ")");
        }
    }
}