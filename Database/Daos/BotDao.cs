internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("UPDATE bots SET room_id = '0' WHERE id = @id LIMIT 1");
            dbClient.AddParameter("id", BotId);
            dbClient.RunQuery();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE bots SET room_id = '" + Room.Id + "', x = '" + X + "', y = '" + Y + "' WHERE id = '" + Bot.Id + "'");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("UPDATE bots SET look = @look, gender = '" + Session.GetHabbo().Gender + "' WHERE id = '" + Bot.BotData.Id + "' LIMIT 1");
            dbClient.AddParameter("look", Session.GetHabbo().Look);
            dbClient.RunQuery();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("UPDATE bots SET chat_enabled = @AutomaticChat, chat_seconds = @SpeakingInterval, is_mixchat = @MixChat, chat_text = @ChatText WHERE id = @id LIMIT 1");
            dbClient.AddParameter("id", BotId);
            dbClient.AddParameter("AutomaticChat", ButterflyEnvironment.BoolToEnum(Convert.ToBoolean(AutomaticChat)));
            dbClient.AddParameter("SpeakingInterval", Convert.ToInt32(SpeakingInterval));
            dbClient.AddParameter("MixChat", ButterflyEnvironment.BoolToEnum(Convert.ToBoolean(MixChat)));
            dbClient.AddParameter("ChatText", Text);
            dbClient.RunQuery();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE bots SET walk_enabled = '" + ButterflyEnvironment.BoolToEnum(Bot.BotData.WalkingEnabled) + "' WHERE id = '" + Bot.BotData.Id + "'");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE bots SET is_dancing = '" + ButterflyEnvironment.BoolToEnum(Bot.BotData.IsDancing) + "' WHERE id = '" + Bot.BotData.Id + "'");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("UPDATE bots SET name = @name WHERE id = '" + Bot.BotData.Id + "' LIMIT 1");
            dbClient.AddParameter("name", DataString);
            dbClient.RunQuery();
        }
        
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE bots SET room_id = '0' WHERE room_id = '" + RoomId + "'");
        }

          internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("INSERT INTO bots (user_id,name,motto,look,gender) VALUES ('" + OwnerId + "', '" + CataBot.Name + "', '" + CataBot.Motto + "', '" + CataBot.Figure + "', '" + CataBot.Gender + "')");
            int Id = Convert.ToInt32(dbClient.InsertQuery());
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT id,user_id,name,motto,look,gender FROM bots WHERE user_id = '" + OwnerId + "' AND id = '" + Id + "' LIMIT 1");
            DataRow BotData = dbClient.GetRow();
        }

        internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("INSERT INTO bots (user_id, name, motto, gender, look, room_id, walk_enabled, x, y, z, rotation, chat_enabled, chat_text, chat_seconds, is_dancing, is_mixchat) " +
        "SELECT '" + Session.GetHabbo().Id + "', name, motto, gender, look, '" + RoomId + "', walk_enabled, x, y, z, rotation, chat_enabled, chat_text, chat_seconds, is_dancing, is_mixchat FROM bots WHERE room_id = '" + OldRoomId + "'");
    }

    
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE bots SET enable = '" + IntValue + "' WHERE id = '" + Bot.BotData.Id + "'");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE bots SET handitem = '" + IntValue + "' WHERE id = '" + Bot.BotData.Id + "'");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE bots SET rotation = '" + Bot.RotBody + "' WHERE id = '" + Bot.BotData.Id + "'");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE bots SET status = '0' WHERE id = '" + Bot.BotData.Id + "'");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE bots SET status = '1' WHERE id = '" + Bot.BotData.Id + "'");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE bots SET status = '0' WHERE id = '" + Bot.BotData.Id + "'");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE bots SET status = '2' WHERE id = '" + Bot.BotData.Id + "'");
    }

    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT * FROM bots WHERE room_id = '" + this.Id + "'");
        table = dbClient.GetTable();
    }
    
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("DELETE FROM bots WHERE room_id = '0' AND user_id = '" + this.UserId + "'");
    }
    
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT * FROM bots WHERE user_id = '" + this.UserId + "' AND room_id = '0'");
        dBots = dbClient.GetTable();
    }