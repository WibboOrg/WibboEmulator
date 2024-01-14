namespace WibboEmulator.Database.Daos.Log;
using System.Data;
using Dapper;

internal sealed class LogMarketDao
{
    internal static void Insert(IDbConnection dbClient, int userId, int price, string content, int itemId) => dbClient.Execute(
        "INSERT INTO log_shop (user_id, date, price, content, catalog_item_id, type) VALUES (@UserId, UNIX_TIMESTAMP(), @Price, @Content, @ItemId, '20')",
        new { UserId = userId, Price = price, Content = content, ItemId = itemId });
}