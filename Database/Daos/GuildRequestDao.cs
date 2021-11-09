using System.Data;
using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class GuildRequestDao
    {
        internal static void Delete(IQueryAdapter dbClient, int groupId)
        {
            dbClient.RunQuery("DELETE FROM group_requests WHERE group_id = '" + groupId + "'");
        }

        internal static DataTable GetAll(IQueryAdapter dbClient, int groupId)
        {
            dbClient.SetQuery("SELECT user_id FROM group_requests WHERE group_id = @id");
            dbClient.AddParameter("id", groupId);
            return dbClient.GetTable();
        }

        internal static DataTable GetAllBySearch(IQueryAdapter dbClient, int groupeId, string searchVal)
        {
            dbClient.SetQuery("SELECT users.id FROM group_requests INNER JOIN users ON group_requests.user_id = users.id WHERE group_requests.group_id = @gid AND users.username LIKE @username LIMIT 14;");
            dbClient.AddParameter("gid", groupeId);
            dbClient.AddParameter("username", searchVal.Replace("%", "\\%").Replace("_", "\\_") + "%");

            return dbClient.GetTable();
        }

        internal static void Insert(IQueryAdapter dbClient, int groupId, int userId)
        {
            dbClient.SetQuery("INSERT INTO group_requests (user_id, group_id) VALUES (@uid, @gid)");
            dbClient.AddParameter("gid", groupId);
            dbClient.AddParameter("uid", userId);
            dbClient.RunQuery();
        }

        internal static void Delete(IQueryAdapter dbClient, int groupId, int userId)
        {
            dbClient.SetQuery("DELETE FROM group_requests WHERE user_id=@uid AND group_id=@gid LIMIT 1");
            dbClient.AddParameter("gid", groupId);
            dbClient.AddParameter("uid", userId);
            dbClient.RunQuery();
        }
    }
}