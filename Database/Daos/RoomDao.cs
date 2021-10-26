using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class RoomDao
    {
        internal static void Query4(IQueryAdapter dbClient, int groupId)
        {
            dbClient.RunQuery("UPDATE rooms SET group_id = '0' WHERE group_id = '" + groupId + "' LIMIT 1");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE rooms SET score = '" + room.RoomData.Score + "' WHERE id = '" + room.Id + "'");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("UPDATE rooms SET '" + DecorationKey + "' = @extradata WHERE id = '" + room.Id + "' LIMIT 1");
            dbClient.AddParameter("extradata", userItem.ExtraData);
            dbClient.RunQuery();
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE rooms SET model_name = 'model_custom', wallthick = '" + WallThick + "', floorthick = '" + FloorThick + "' WHERE id = " + Room.Id + " LIMIT 1");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE FROM rooms WHERE id = '" + RoomId + "'");
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("UPDATE rooms SET caption = @caption, description = @description, password = @password, category = '" + CategoryId + "', state = '" + str5 + "', tags = @tags, users_max = '" + MaxUsers + "', allow_pets = '" + (AllowPets ? 1 : 0) + "', allow_pets_eat = '" + (AllowPetsEat ? 1 : 0) + "', allow_walkthrough = '" + (AllowWalkthrough ? 1 : 0) + "', allow_hidewall = '" + (room.RoomData.Hidewall ? 1 : 0) + "', floorthick = '" + room.RoomData.FloorThickness + "', wallthick = '" + room.RoomData.WallThickness + "', moderation_mute_fuse = '" + mutefuse + "', moderation_kick_fuse = '" + kickfuse + "', moderation_ban_fuse = '" + banfuse + "', chat_type = '" + ChatType + "', chat_balloon = '" + ChatBalloon + "', chat_speed = '" + ChatSpeed + "', chat_max_distance = '" + ChatMaxDistance + "', chat_flood_protection = '" + ChatFloodProtection + "', troc_status = '" + TrocStatus + "' WHERE id = '" + room.Id + "'");
            dbClient.AddParameter("caption", room.RoomData.Name);
            dbClient.AddParameter("description", room.RoomData.Description);
            dbClient.AddParameter("password", room.RoomData.Password);
            dbClient.AddParameter("tags", (stringBuilder).ToString());
            dbClient.RunQuery();
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("UPDATE rooms SET owner = @newname WHERE owner = @oldname");
            dbClient.AddParameter("newname", NewUsername);
            dbClient.AddParameter("oldname", Session.GetHabbo().Username);
            dbClient.RunQuery();
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT id FROM rooms WHERE owner = 'WibboGame'");
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE rooms SET users_now = '0' WHERE users_now > '0'");
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("INSERT INTO rooms (caption,description,owner,model_name,category,state, wallpaper, floor, landscape, allow_hidewall, wallthick, floorthick) SELECT @caption, @desc, @username, @model, category, state, wallpaper, floor, landscape, allow_hidewall, wallthick, floorthick FROM rooms WHERE id = '5328079'");
            dbClient.AddParameter("caption", this.GetHabbo().Username);
            dbClient.AddParameter("desc", ButterflyEnvironment.GetLanguageManager().TryGetValue("room.welcome.desc", this.Langue));
            dbClient.AddParameter("username", this.GetHabbo().Username);
            dbClient.AddParameter("model", "model_welcome");
            RoomId = Convert.ToInt32(dbClient.InsertQuery());
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT * FROM rooms WHERE owner = @username and state != 'invisible' ORDER BY users_now DESC");
            dbClient.AddParameter("username", SearchData.Remove(0, 6));
            GetRooms = dbClient.GetTable();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT * FROM rooms WHERE caption LIKE @query OR owner LIKE @query ORDER BY users_now DESC LIMIT 50");
            dbClient.AddParameter("query", SearchData.Replace("%", "\\%").Replace("_", "\\_") + "%");
            Table = dbClient.GetTable();
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("INSERT INTO rooms (caption, owner, description, model_name, wallpaper, floor, landscape, allow_hidewall, wallthick, floorthick, allow_rightsoverride, allow_hidewireds)" +
            "SELECT 'Appart " + OldRoomId + " copie', '" + Session.GetHabbo().Username + "', description, model_name, wallpaper, floor, landscape, allow_hidewall, wallthick, floorthick, allow_rightsoverride, allow_hidewireds FROM rooms WHERE id = '" + OldRoomId + "'; ");
            RoomId = Convert.ToInt32(dbClient.InsertQuery());
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE rooms SET model_name = 'model_custom' WHERE id = '" + Room.Id + "' LIMIT 1");
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE rooms SET allow_hidewireds = '" + (currentRoom.RoomData.HideWireds ? 1 : 0) + "' WHERE id = '" + currentRoom.Id + "'");
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