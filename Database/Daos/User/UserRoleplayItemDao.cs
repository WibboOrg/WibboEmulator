namespace WibboEmulator.Database.Daos;
using System.Data;
using WibboEmulator.Database.Interfaces;

internal class UserRoleplayItemDao
{
    internal static void Delete(IQueryAdapter dbClient, int userId, int roleplayId) => dbClient.RunQuery("DELETE FROM `user_roleplay_item` WHERE `user_id` = '" + userId + "' AND `rp_id` = '" + roleplayId + "'");

    internal static DataTable GetAll(IQueryAdapter dbClient, int userId, int roleplayId)
    {
        dbClient.SetQuery("SELECT `id`, `user_id`, `rp_id`, `item_id`, `count` FROM `user_roleplay_item` WHERE `user_id` = '" + userId + "' AND `rp_id` = '" + roleplayId + "'");
        return dbClient.GetTable();
    }

    internal static int Insert(IQueryAdapter dbClient, int userId, int roleplayId, int itemId, int count)
    {
        dbClient.SetQuery("INSERT INTO `user_roleplay_item` (`user_id`, `rp_id`, `item_id`, `count`) VALUES ('" + userId + "', '" + roleplayId + "', '" + itemId + "', '" + count + "')");

        return Convert.ToInt32(dbClient.InsertQuery());
    }

    internal static void UpdateAddCount(IQueryAdapter dbClient, int itemId, int count) => dbClient.RunQuery("UPDATE `user_roleplay_item` SET `count` = `count` + '" + count + "' WHERE `id` = '" + itemId + "' LIMIT 1");

    internal static void UpdateRemoveCount(IQueryAdapter dbClient, int itemId, int count) => dbClient.RunQuery("UPDATE `user_roleplay_item` SET `count` = `count` - '" + count + "' WHERE `id` = '" + itemId + "' LIMIT 1");

    internal static void Delete(IQueryAdapter dbClient, int itemId) => dbClient.RunQuery("DELETE FROM `user_roleplay_item` WHERE `id` = '" + itemId + "' LIMIT 1");
}