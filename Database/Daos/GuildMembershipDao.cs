using System.Data;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class GuildMembershipDao
    {
        internal static void Delete(IQueryAdapter dbClient, int groupId)
        {
            dbClient.RunQuery("DELETE FROM group_memberships WHERE group_id = '" + groupId + "'");
        }

        internal static DataTable GetAllUserIdBySearchAndStaff(IQueryAdapter dbClient, int groupeId, string searchVal)
        {
            dbClient.SetQuery("SELECT users.id FROM group_memberships INNER JOIN users ON group_memberships.user_id = users.id WHERE group_memberships.group_id = @gid AND group_memberships.rank > '0' AND users.username LIKE @username LIMIT 14;");
            dbClient.AddParameter("gid", groupeId);
            dbClient.AddParameter("username", searchVal.Replace("%", "\\%").Replace("_", "\\_") + "%");

            return dbClient.GetTable();
        }

        internal static DataTable GetAllUserIdBySearch(IQueryAdapter dbClient, int groupeId, string searchVal)
        {
            dbClient.SetQuery("SELECT users.id AS id FROM group_memberships INNER JOIN users ON group_memberships.user_id = users.id WHERE group_memberships.group_id = @gid AND users.username LIKE @username LIMIT 14;");
            dbClient.AddParameter("gid", groupeId);
            dbClient.AddParameter("username", searchVal.Replace("%", "\\%").Replace("_", "\\_") + "%");
            return dbClient.GetTable();
        }

        internal static DataTable GetAll(IQueryAdapter dbClient, string id)
        {
            dbClient.SetQuery("SELECT user_id, rank FROM group_memberships WHERE group_id = @id");
            dbClient.AddParameter("id", id);
            return dbClient.GetTable();
        }

        internal static void UpdateRank1(IQueryAdapter dbClient, int groupId, int userId)
        {
            dbClient.SetQuery("UPDATE group_memberships SET rank = '1' WHERE user_id = @uid AND group_id = @gid LIMIT 1");
            dbClient.AddParameter("gid", groupId);
            dbClient.AddParameter("uid", userId);
            dbClient.RunQuery();
        }

        internal static void UpdateRank0(IQueryAdapter dbClient, int groupId, int userId)
        {
            dbClient.SetQuery("UPDATE group_memberships SET rank = '0' WHERE user_id = @uid AND group_id = @gid");
            dbClient.AddParameter("gid", groupId);
            dbClient.AddParameter("uid", userId);
            dbClient.RunQuery();
        }

        internal static void Delete(IQueryAdapter dbClient, int groupId, int userId)
        {
            dbClient.SetQuery("DELETE FROM group_memberships WHERE user_id=@uid AND group_id=@gid LIMIT 1");
            dbClient.AddParameter("gid", groupId);
            dbClient.AddParameter("uid", userId);
            dbClient.RunQuery();
        }

        internal static void Insert(IQueryAdapter dbClient, int groupId, int userId)
        {
            dbClient.SetQuery("INSERT INTO group_memberships (user_id, group_id) VALUES (@uid, @gid)");
            dbClient.AddParameter("gid", groupId);
            dbClient.AddParameter("uid", userId);
            dbClient.RunQuery();
        }

        internal static DataTable GetOneByUserId(IQueryAdapter dbClient, int userId)
        {
            dbClient.SetQuery("SELECT group_id FROM group_memberships WHERE user_id = '" + userId + "';");
            return dbClient.GetTable();
        }
    }
}
