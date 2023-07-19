namespace WibboEmulator.Database.Daos.Log;
using WibboEmulator.Database.Interfaces;

internal sealed class LogMarketDao
{
    internal static void Insert(IQueryAdapter dbClient, int userId, int price, string content, int itemId)
    {
        dbClient.SetQuery("INSERT INTO `log_shop` (`user_id`, `date`, `price`, `content`, `catalog_item_id`, `type`) VALUES ('" + userId + "', UNIX_TIMESTAMP(), '" + price + "', @content, '" + itemId + "', '20')");
        dbClient.AddParameter("content", content);
        dbClient.RunQuery();
    }
}