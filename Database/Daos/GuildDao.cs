using Butterfly.Database.Interfaces;
using System;
using System.Data;

namespace Butterfly.Database.Daos
{
    class GuildDao
    {
        internal static void UpdateBadge(IQueryAdapter dbClient, int groupId, string badge)
        {
            dbClient.SetQuery("UPDATE groups SET badge = @badge WHERE id = @groupId LIMIT 1");
            dbClient.AddParameter("badge", badge);
            dbClient.AddParameter("groupId", groupId);
            dbClient.RunQuery();
        }

        internal static void UpdateColors(IQueryAdapter dbClient, int colour1, int colour2, int groupId)
        {
            dbClient.SetQuery("UPDATE groups SET colour1 = @colour1, colour2 = @colour2 WHERE id = @groupId LIMIT 1");
            dbClient.AddParameter("colour1", colour1);
            dbClient.AddParameter("colour2", colour2);
            dbClient.AddParameter("groupId", groupId);
            dbClient.RunQuery();
        }

        internal static void UpdateNameAndDesc(IQueryAdapter dbClient, int groupId, string name, string desc)
        {
            dbClient.SetQuery("UPDATE `groups` SET `name` = @name, `desc` = @desc WHERE `id` = @groupId LIMIT 1");
            dbClient.AddParameter("name", name);
            dbClient.AddParameter("desc", desc);
            dbClient.AddParameter("groupId", groupId);
            dbClient.RunQuery();
        }

        internal static void UpdateStateAndDeco(IQueryAdapter dbClient, int groupId, int groupState, int furniOptions)
        {
            dbClient.SetQuery("UPDATE groups SET state = @GroupState, admindeco = @AdminDeco WHERE id = @groupId LIMIT 1");
            dbClient.AddParameter("GroupState", groupState);
            dbClient.AddParameter("AdminDeco", furniOptions);
            dbClient.AddParameter("groupId", groupId);
            dbClient.RunQuery();
        }

        internal static void Delete(IQueryAdapter dbClient, int groupId)
        {
            dbClient.RunQuery("DELETE FROM groups WHERE id = '" + groupId + "'");
        }

        internal static DataRow GetOne(IQueryAdapter dbClient, int groupId)
        {
            dbClient.SetQuery("SELECT * FROM groups WHERE id = @id LIMIT 1");
            dbClient.AddParameter("id", groupId);
            return dbClient.GetRow();
        }

        internal static int Insert(IQueryAdapter dbClient, string name, string description, int creatorId, string badge, int roomId, int colour1, int colour2)
        {
            dbClient.SetQuery("INSERT INTO `groups` (`name`, `desc`, `badge`, `owner_id`, `created`, `room_id`, `state`, `colour1`, `colour2`, `admindeco`) VALUES (@name, @desc, @badge, @owner, UNIX_TIMESTAMP(), @room, '0', @colour1, @colour2, '1')");
            dbClient.AddParameter("name", name);
            dbClient.AddParameter("desc", description);
            dbClient.AddParameter("owner", creatorId);
            dbClient.AddParameter("badge", badge);
            dbClient.AddParameter("room", roomId);
            dbClient.AddParameter("colour1", colour1);
            dbClient.AddParameter("colour2", colour2);
            return Convert.ToInt32(dbClient.InsertQuery());
        }
    }
}