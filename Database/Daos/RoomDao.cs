using System;
using System.Data;
using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class RoomDao
    {
        internal static void UpdateGroupId(IQueryAdapter dbClient, int groupId)
        {
            dbClient.RunQuery("UPDATE rooms SET group_id = '0' WHERE group_id = '" + groupId + "' LIMIT 1");
        }

        internal static void UpdateScore(IQueryAdapter dbClient, int roomId, int score)
        {
            dbClient.RunQuery("UPDATE rooms SET score = '" + score + "' WHERE id = '" + roomId + "'");
        }

        internal static void UpdateDecoration(IQueryAdapter dbClient, int roomId, int decorationKey, string extraData)
        {
            dbClient.SetQuery("UPDATE rooms SET '" + decorationKey + "' = @extradata WHERE id = '" + roomId + "' LIMIT 1");
            dbClient.AddParameter("extradata", extraData);
            dbClient.RunQuery();
        }

        internal static void UpdateModel(IQueryAdapter dbClient, int roomId, int wallThick, int floorThick)
        {
            dbClient.RunQuery("UPDATE rooms SET model_name = 'model_custom', wallthick = '" + wallThick + "', floorthick = '" + floorThick + "' WHERE id = " + roomId + " LIMIT 1");
        }

        internal static void Delete(IQueryAdapter dbClient, int roomId)
        {
            dbClient.RunQuery("DELETE FROM rooms WHERE id = '" + roomId + "'");
        }

        internal static void UpdateAll(IQueryAdapter dbClient, int roomId, string name, string description, string password, string tags, int categoryId, int state, int maxUsers, bool allowPets, bool allowPetsEat, bool allowWalkthrough, bool hidewall, , int floorThickness, int wallThickness, int mutefuse, int kickfuse, int banfuse, int chatType, int chatBalloon, int chatSpeed, int chatMaxDistance, int chatFloodProtection, int trocStatus)
        {
            dbClient.SetQuery("UPDATE rooms SET caption = @caption, description = @description, password = @password, category = '" + categoryId + "', state = '" + state + "', tags = @tags, users_max = '" + maxUsers + "', allow_pets = '" + (allowPets ? 1 : 0) + "', allow_pets_eat = '" + (allowPetsEat ? 1 : 0) + "', allow_walkthrough = '" + (allowWalkthrough ? 1 : 0) + "', allow_hidewall = '" + (hidewall ? 1 : 0) + "', floorthick = '" + floorThickness + "', wallthick = '" + wallThickness + "', moderation_mute_fuse = '" + mutefuse + "', moderation_kick_fuse = '" + kickfuse + "', moderation_ban_fuse = '" + banfuse + "', chat_type = '" + chatType + "', chat_balloon = '" + chatBalloon + "', chat_speed = '" + chatSpeed + "', chat_max_distance = '" + chatMaxDistance + "', chat_flood_protection = '" + chatFloodProtection + "', troc_status = '" + trocStatus + "' WHERE id = '" + roomId + "'");
            dbClient.AddParameter("caption", name);
            dbClient.AddParameter("description", description);
            dbClient.AddParameter("password", password);
            dbClient.AddParameter("tags", tags);
            dbClient.RunQuery();
        }

        internal static void UpdateOwner(IQueryAdapter dbClient, string newUsername, string username)
        {
            dbClient.SetQuery("UPDATE rooms SET owner = @newname WHERE owner = @oldname");
            dbClient.AddParameter("newname", newUsername);
            dbClient.AddParameter("oldname", username);
            dbClient.RunQuery();
        }

        internal static DataTable GetAllByOwner(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT id FROM rooms WHERE owner = 'WibboGame'");

            return dbClient.GetTable();
        }

        internal static void UpdateUsersNow(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE rooms SET users_now = '0' WHERE users_now > '0'");
        }

        internal static int InsertDuplicate(IQueryAdapter dbClient, string username, string desc)
        {
            dbClient.SetQuery("INSERT INTO rooms (caption,description,owner,model_name,category,state, wallpaper, floor, landscape, allow_hidewall, wallthick, floorthick) SELECT @caption, @desc, @username, @model, category, state, wallpaper, floor, landscape, allow_hidewall, wallthick, floorthick FROM rooms WHERE id = '5328079'");
            dbClient.AddParameter("caption", username);
            dbClient.AddParameter("desc", desc);
            dbClient.AddParameter("username", username);
            dbClient.AddParameter("model", "model_welcome");
            return Convert.ToInt32(dbClient.InsertQuery());
        }

        internal static DataTable GetAllSearchByUsername(IQueryAdapter dbClient, string searchData)
        {
            dbClient.SetQuery("SELECT * FROM rooms WHERE owner = @username and state != 'invisible' ORDER BY users_now DESC");
            dbClient.AddParameter("username", searchData.Remove(0, 6));
            return dbClient.GetTable();
        }

        internal static DataTable GetAllSearch(IQueryAdapter dbClient, string searchData)
        {
            dbClient.SetQuery("SELECT * FROM rooms WHERE caption LIKE @query OR owner LIKE @query ORDER BY users_now DESC LIMIT 50");
            dbClient.AddParameter("query", searchData.Replace("%", "\\%").Replace("_", "\\_") + "%");
            return dbClient.GetTable();
        }

        internal static int InsertDuplicate(IQueryAdapter dbClient, int roomId, string username)
        {
            dbClient.SetQuery("INSERT INTO rooms (caption, owner, description, model_name, wallpaper, floor, landscape, allow_hidewall, wallthick, floorthick, allow_rightsoverride, allow_hidewireds)" +
            "SELECT 'Appart " + roomId + " copie', '" + username + "', description, model_name, wallpaper, floor, landscape, allow_hidewall, wallthick, floorthick, allow_rightsoverride, allow_hidewireds FROM rooms WHERE id = '" + roomId + "'; ");
            return Convert.ToInt32(dbClient.InsertQuery());
        }

        internal static void Query8(IQueryAdapter dbClient, int roomId)
        {
            dbClient.RunQuery("UPDATE rooms SET model_name = 'model_custom' WHERE id = '" + roomId + "' LIMIT 1");
        }

        internal static void Query8(IQueryAdapter dbClient, int roomId, bool hideWireds)
        {
            dbClient.RunQuery("UPDATE rooms SET allow_hidewireds = '" + (hideWireds ? 1 : 0) + "' WHERE id = '" + roomId + "'");
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE rooms SET model_name = 'model_custom' WHERE id = '" + Room.Id + "' LIMIT 1");
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("UPDATE rooms SET owner = @newowner WHERE id = '" + Room.Id + "'");
            dbClient.AddParameter("newowner", Session.GetHabbo().Username);
            dbClient.RunQuery();
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE rooms SET price = '0' WHERE id = '" + Room.Id + "' LIMIT 1");
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE rooms SET price = '0' WHERE id = '" + Room.Id + "' LIMIT 1");
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("UPDATE rooms SET price= @price WHERE id = @roomid LIMIT 1");
            dbClient.AddParameter("roomid", Room.Id);
            dbClient.AddParameter("price", Prix);
            dbClient.RunQuery();
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE rooms SET users_max = '" + MaxUsers + "' WHERE id = '" + this.Id + "'");
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT * FROM rooms WHERE id = '" + RoomId + "'");
            Row = dbClient.GetRow();
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("INSERT INTO rooms (caption,description,owner,model_name,category,users_max,troc_status) VALUES (@caption, @desc, @username, @model, @cat, @usmax, '" + TradeSettings + "')");
            dbClient.AddParameter("caption", Name);
            dbClient.AddParameter("desc", Desc);
            dbClient.AddParameter("username", Session.GetHabbo().Username);
            dbClient.AddParameter("model", Model);
            dbClient.AddParameter("cat", Category);
            dbClient.AddParameter("usmax", MaxVisitors);
            RoomId = Convert.ToInt32(dbClient.InsertQuery());
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE rooms SET users_now = '" + count + "' WHERE id = '" + this._room.Id + "'");
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE rooms SET state = 'locked' WHERE id = '" + room.Id + "'");
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE rooms SET caption = 'Cet appart ne respect par les conditions dutilisation', description = 'Cet appart ne respect par les conditions dutilisation', tags = '' WHERE id = '" + room.Id + "'");
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT * FROM rooms WHERE owner = @name ORDER BY id ASC");
            dbClient.AddParameter("name", this.Username);
            table = dbClient.GetTable();
        }
    }
}