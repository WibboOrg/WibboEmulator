namespace WibboEmulator.Database.Daos.Guild;
using System.Data;
using WibboEmulator.Database.Interfaces;

internal class GuildMembershipDao
{
    internal static void Delete(IQueryAdapter dbClient, int groupId) => dbClient.RunQuery("DELETE FROM `guild_membership` WHERE group_id = '" + groupId + "'");

    internal static DataTable GetAllUserIdBySearchAndStaff(IQueryAdapter dbClient, int groupeId, string searchVal)
    {
        dbClient.SetQuery("SELECT `user`.id FROM `guild_membership` INNER JOIN `user` ON `guild_membership`.user_id = `user`.id WHERE `guild_membership`.group_id = @gid AND `guild_membership`.rank > '0' AND `user`.username LIKE @username LIMIT 14");
        dbClient.AddParameter("gid", groupeId);
        dbClient.AddParameter("username", searchVal.Replace("%", "\\%").Replace("_", "\\_") + "%");

        return dbClient.GetTable();
    }

    internal static DataTable GetAllUserIdBySearch(IQueryAdapter dbClient, int groupeId, string searchVal)
    {
        dbClient.SetQuery("SELECT `user`.id AS id FROM `guild_membership` INNER JOIN `user` ON `guild_membership`.user_id = `user`.id WHERE `guild_membership`.group_id = @gid AND `user`.username LIKE @username LIMIT 14");
        dbClient.AddParameter("gid", groupeId);
        dbClient.AddParameter("username", searchVal.Replace("%", "\\%").Replace("_", "\\_") + "%");
        return dbClient.GetTable();
    }

    internal static DataTable GetAll(IQueryAdapter dbClient, int id)
    {
        dbClient.SetQuery("SELECT user_id, rank FROM `guild_membership` WHERE group_id = @id");
        dbClient.AddParameter("id", id);
        return dbClient.GetTable();
    }

    internal static void UpdateRank(IQueryAdapter dbClient, int groupId, int userId, int rank)
    {
        dbClient.SetQuery("UPDATE `guild_membership` SET rank = @rank WHERE user_id = @uid AND group_id = @gid LIMIT 1");
        dbClient.AddParameter("gid", groupId);
        dbClient.AddParameter("uid", userId);
        dbClient.AddParameter("rank", rank);
        dbClient.RunQuery();
    }

    internal static void Delete(IQueryAdapter dbClient, int groupId, int userId)
    {
        dbClient.SetQuery("DELETE FROM `guild_membership` WHERE user_id=@uid AND group_id=@gid LIMIT 1");
        dbClient.AddParameter("gid", groupId);
        dbClient.AddParameter("uid", userId);
        dbClient.RunQuery();
    }

    internal static void Insert(IQueryAdapter dbClient, int groupId, int userId)
    {
        dbClient.SetQuery("INSERT INTO `guild_membership` (user_id, group_id) VALUES (@uid, @gid)");
        dbClient.AddParameter("gid", groupId);
        dbClient.AddParameter("uid", userId);
        dbClient.RunQuery();
    }

    internal static DataTable GetOneByUserId(IQueryAdapter dbClient, int userId)
    {
        dbClient.SetQuery("SELECT group_id FROM `guild_membership` WHERE user_id = '" + userId + "'");
        return dbClient.GetTable();
    }
}
