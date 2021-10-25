using Butterfly.Database.Interfaces;
using System.Data;

namespace Butterfly.Database.Daos
{
    class AllDao
    {
        internal static void Query1(IQueryAdapter dbClient, int groupId)
        {
            dbClient.RunQuery("DELETE FROM `groups` WHERE `id` = '" + groupId + "'");
        }

        internal static void Query2(IQueryAdapter dbClient, int groupId)
        {
            dbClient.RunQuery("DELETE FROM `group_memberships` WHERE `group_id` = '" + groupId + "'");
        }

        internal static void Query3(IQueryAdapter dbClient, int groupId)
        {
            dbClient.RunQuery("DELETE FROM `group_requests` WHERE `group_id` = '" + groupId + "'");
        }

        internal static void Query4(IQueryAdapter dbClient, int groupId)
        {
            dbClient.RunQuery("UPDATE `rooms` SET `group_id` = '0' WHERE `group_id` = '" + groupId + "' LIMIT 1");
        }

        internal static void Query5(IQueryAdapter dbClient, int groupId)
        {
            dbClient.RunQuery("UPDATE `user_stats` SET `group_id` = '0' WHERE `group_id` = '" + groupId + "' LIMIT 1");
        }

        internal static DataTable Query6(IQueryAdapter dbClient, int groupeId, string searchVal)
        {
            dbClient.SetQuery("SELECT users.id FROM group_requests INNER JOIN users ON group_requests.user_id = users.id WHERE group_requests.group_id = @gid AND users.username LIKE @username LIMIT 14;");
            dbClient.AddParameter("gid", groupeId);
            dbClient.AddParameter("username", searchVal.Replace("%", "\\%").Replace("_", "\\_") + "%");
            
            return dbClient.GetTable();
        }
    }
}
