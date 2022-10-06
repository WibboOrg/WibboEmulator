namespace WibboEmulator.Database.Daos.Guild;
using System.Data;
using WibboEmulator.Database.Interfaces;

internal class GuildRequestDao
{
    internal static void Delete(IQueryAdapter dbClient, int groupId) => dbClient.RunQuery("DELETE FROM `guild_request` WHERE group_id = '" + groupId + "'");

    internal static DataTable GetAll(IQueryAdapter dbClient, int groupId)
    {
        dbClient.SetQuery("SELECT user_id FROM `guild_request` WHERE group_id = @id");
        dbClient.AddParameter("id", groupId);
        return dbClient.GetTable();
    }

    internal static DataTable GetAllBySearch(IQueryAdapter dbClient, int groupeId, string searchVal)
    {
        dbClient.SetQuery("SELECT `user`.id FROM `guild_request` INNER JOIN `user` ON `guild_request`.user_id = `user`.id WHERE `guild_request`.group_id = @gid AND `user`.username LIKE @username LIMIT 14");
        dbClient.AddParameter("gid", groupeId);
        dbClient.AddParameter("username", searchVal.Replace("%", "\\%").Replace("_", "\\_") + "%");

        return dbClient.GetTable();
    }

    internal static void Insert(IQueryAdapter dbClient, int groupId, int userId)
    {
        dbClient.SetQuery("INSERT INTO `guild_request` (user_id, group_id) VALUES (@uid, @gid)");
        dbClient.AddParameter("gid", groupId);
        dbClient.AddParameter("uid", userId);
        dbClient.RunQuery();
    }

    internal static void Delete(IQueryAdapter dbClient, int groupId, int userId)
    {
        dbClient.SetQuery("DELETE FROM `guild_request` WHERE user_id=@uid AND group_id=@gid LIMIT 1");
        dbClient.AddParameter("gid", groupId);
        dbClient.AddParameter("uid", userId);
        dbClient.RunQuery();
    }
}