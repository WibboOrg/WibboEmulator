namespace WibboEmulator.Database.Daos.Item;
using System.Data;
using Dapper;

internal sealed class ItemLimitedDao
{
    internal static void Insert(IDbConnection dbClient, int itemId, int limitedNumber, int limitedStack) => dbClient.Execute(
        "INSERT INTO `item_limited` VALUES (" + itemId + "," + limitedNumber + "," + limitedStack + ")");
}