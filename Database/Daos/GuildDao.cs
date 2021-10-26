using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class GuildDao
    {
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("UPDATE groups SET badge = @badge WHERE id = @groupId LIMIT 1");
            dbClient.AddParameter("badge", Group.Badge);
            dbClient.AddParameter("groupId", Group.Id);
            dbClient.RunQuery();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("UPDATE groups SET colour1 = @colour1, colour2 = @colour2 WHERE id = @groupId LIMIT 1");
            dbClient.AddParameter("colour1", Colour1);
            dbClient.AddParameter("colour2", Colour2);
            dbClient.AddParameter("groupId", Group.Id);
            dbClient.RunQuery();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("UPDATE groups SET name= @name, desc = @desc WHERE id = @groupId LIMIT 1");
            dbClient.AddParameter("name", Name);
            dbClient.AddParameter("desc", Desc);
            dbClient.AddParameter("groupId", GroupId);
            dbClient.RunQuery();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("UPDATE groups SET state = @GroupState, admindeco = @AdminDeco WHERE id = @groupId LIMIT 1");
            dbClient.AddParameter("GroupState", (Group.GroupType == GroupType.OPEN ? 0 : Group.GroupType == GroupType.LOCKED ? 1 : 2).ToString());
            dbClient.AddParameter("AdminDeco", (FurniOptions == 1 ? 1 : 0).ToString());
            dbClient.AddParameter("groupId", Group.Id);
            dbClient.RunQuery();
        }
        internal static void Query1(IQueryAdapter dbClient, int groupId)
        {
            dbClient.RunQuery("DELETE FROM groups WHERE id = '" + groupId + "'");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT * FROM groups WHERE id = @id LIMIT 1");
            dbClient.AddParameter("id", id);
            DataRow Row = dbClient.GetRow();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("INSERT INTO groups (name, desc, badge, owner_id, created, room_id, state, colour1, colour2, admindeco) VALUES (@name, @desc, @badge, @owner, UNIX_TIMESTAMP(), @room, '0', @colour1, @colour2, '1')");
            dbClient.AddParameter("name", Group.Name);
            dbClient.AddParameter("desc", Group.Description);
            dbClient.AddParameter("owner", Group.CreatorId);
            dbClient.AddParameter("badge", Group.Badge);
            dbClient.AddParameter("room", Group.RoomId);
            dbClient.AddParameter("colour1", Group.Colour1);
            dbClient.AddParameter("colour2", Group.Colour2);
            Group.Id = Convert.ToInt32(dbClient.InsertQuery());
        }
    }
}