using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class GuildMembershipDao
    {
        internal static void Query2(IQueryAdapter dbClient, int groupId)
        {
            dbClient.RunQuery("DELETE FROM group_memberships WHERE group_id = '" + groupId + "'");
        }

internal static DataTable Query7(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT users.id FROM group_memberships INNER JOIN users ON group_memberships.user_id = users.id WHERE group_memberships.group_id = @gid AND group_memberships.rank > '0' AND users.username LIKE @username LIMIT 14;");
            dbClient.AddParameter("gid", GroupeId);
            dbClient.AddParameter("username", SearchVal.Replace("%", "\\%").Replace("_", "\\_") + "%");

            return dbClient.GetTable();
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT users.id AS id FROM group_memberships INNER JOIN users ON group_memberships.user_id = users.id WHERE group_memberships.group_id = @gid AND users.username LIKE @username LIMIT 14;");
            dbClient.AddParameter("gid", GroupeId);
            dbClient.AddParameter("username", SearchVal.Replace("%", "\\%").Replace("_", "\\_") + "%");
            MembresTable = dbClient.GetTable();
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT user_id, rank FROM group_memberships WHERE group_id = @id");
            dbClient.AddParameter("id", this.Id);
            GetMembers = dbClient.GetTable();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("UPDATE group_memberships SET rank = '1' WHERE user_id = @uid AND group_id = @gid LIMIT 1");
            dbClient.AddParameter("gid", this.Id);
            dbClient.AddParameter("uid", Id);
            dbClient.RunQuery();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("UPDATE group_memberships SET rank = '0' WHERE user_id = @uid AND group_id = @gid");
            dbClient.AddParameter("gid", this.Id);
            dbClient.AddParameter("uid", UserId);
            dbClient.RunQuery();
        }
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("UPDATE group_memberships SET rank = '0' WHERE user_id = @uid AND group_id = @gid");
        dbClient.AddParameter("gid", this.Id);
        dbClient.AddParameter("uid", Id);
        dbClient.RunQuery();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("INSERT INTO group_memberships (user_id, group_id) VALUES (@uid, @gid)");
        dbClient.AddParameter("gid", this.Id);
        dbClient.AddParameter("uid", Id);
        dbClient.RunQuery();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("DELETE FROM group_memberships WHERE user_id=@uid AND group_id=@gid LIMIT 1");
        dbClient.AddParameter("gid", this.Id);
        dbClient.AddParameter("uid", Id);
        dbClient.RunQuery();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("INSERT INTO group_memberships (user_id, group_id) VALUES (@uid, @gid)");
        dbClient.AddParameter("gid", this.Id);
        dbClient.AddParameter("uid", Id);
        dbClient.RunQuery();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT group_id FROM group_memberships WHERE user_id = '" + userId + "';");
        GroupMemberships = dbClient.GetTable();
    }
    }
}
