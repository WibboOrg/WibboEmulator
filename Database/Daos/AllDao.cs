using Butterfly.Database.Interfaces;
using System.Data;

namespace Butterfly.Database.Daos
{
    class AllDao
    {
        
        
        
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT base_id,extra_data FROM user_presents WHERE item_id = @presentId LIMIT 1");
            dbClient.AddParameter("presentId", Present.Id);
            Data = dbClient.GetRow();
        }
        
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE FROM user_presents WHERE item_id = '" + Present.Id + "' LIMIT 1");
        }
        
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("INSERT INTO logs_flagme (user_id, oldusername, newusername, time) VALUES (@userid, @oldusername, @newusername, '" + ButterflyEnvironment.GetUnixTimestamp() + "');");
            dbClient.AddParameter("userid", Session.GetHabbo().Id);
            dbClient.AddParameter("oldusername", Session.GetHabbo().Username);
            dbClient.AddParameter("newusername", NewUsername);
            dbClient.RunQuery();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT COUNT(0) FROM messenger_friendships WHERE (user_one_id = @userid);");
            dbClient.AddParameter("userid", userID);
            friendCount = dbClient.GetInteger();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("UPDATE messenger_friendships SET relation = '" + Type + "' WHERE user_one_id=@id AND user_two_id=@target LIMIT 1");
            dbClient.AddParameter("id", Session.GetHabbo().Id);
            dbClient.AddParameter("target", User);
            dbClient.RunQuery();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT slot_id,look,gender FROM user_wardrobe WHERE user_id = '" + Session.GetHabbo().Id + "' LIMIT 24");
            DataTable WardrobeData = dbClient.GetTable();
        }
        
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("INSERT INTO user_photos (user_id,photo,time) VALUES ('" + Client.GetHabbo().Id + "', @photoid, '" + Time + "')");
            dbClient.AddParameter("photoid", PhotoId);
            dbClient.RunQuery();
        }
       
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT identifiant, value_fr, value_en, value_br FROM system_locale");
            table = dbClient.GetTable();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT userpeak FROM server_status");
            UserPeak = dbClient.GetInteger();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE server_status SET users_online = '" + UsersOnline + "', rooms_loaded = '" + RoomsLoaded + "', userpeak = '" + UserPeak + "', stamp = UNIX_TIMESTAMP()");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("INSERT INTO system_stats (online, time, room) VALUES ('" + UsersOnline + "', UNIX_TIMESTAMP(), '" + RoomsLoaded + "')");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT id, category, group_name, level, reward_pixels, reward_points, progress_needed FROM achievements");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("REPLACE INTO user_achievement VALUES ('" + Session.GetHabbo().Id + "', @group, '" + NewLevel + "', '" + NewProgress + "')");
            dbClient.AddParameter("group", AchievementGroup);
            dbClient.RunQuery();
        }
        
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("REPLACE INTO user_achievement VALUES ('" + Session.GetHabbo().Id + "', @group, '" + NewLevel + "', '" + NewProgress + "')");
            dbClient.AddParameter("group", AchievementGroup);
            dbClient.RunQuery();
        }
        
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT id,item_id,catalog_name,cost_credits,cost_pixels,cost_diamonds,amount,page_id,limited_sells,limited_stack,offer_active,badge FROM catalog_items ORDER by ID DESC");
            DataTable CatalogueItems = dbClient.GetTable();
        }
      
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE catalog_vouchers SET current_uses = current_uses + '1' WHERE voucher = '" + this._code + "' LIMIT 1");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT voucher,type,value,current_uses,max_uses FROM catalog_vouchers WHERE enabled = '1'");
            GetVouchers = dbClient.GetTable();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT user_id, user_name, room_id, type, message FROM chatlogs WHERE user_id = '" + UserId + "' ORDER BY id DESC LIMIT 100");
            DataTable table = dbClient.GetTable();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT user_id, user_name, room_id, type, message FROM chatlogs WHERE room_id = '" + RoomId + "' ORDER BY id DESC LIMIT 100");
            table = dbClient.GetTable();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT id, only_staff FROM systeme_effects ORDER by id ASC");
            DataTable table = dbClient.GetTable();
        }
       
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE server_status SET status = '1', users_online = '0', rooms_loaded = '0', stamp = '" + ButterflyEnvironment.GetUnixTimestamp() + "'");
        }
       
        
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("INSERT INTO chatlogs (user_id, room_id, user_name, timestamp, message, type) VALUES ('" + this.GetHabbo().Id + "', '" + RoomId + "', @username, UNIX_TIMESTAMP(), @message, @type)");
            dbClient.AddParameter("message", Message);
            dbClient.AddParameter("type", type);
            dbClient.AddParameter("username", this.GetHabbo().Username);
            dbClient.RunQuery();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("INSERT INTO chatlogs_pub (user_id, user_name, timestamp, message) VALUES ('" + this.GetHabbo().Id + "', @pseudo, UNIX_TIMESTAMP(), @message)");
            dbClient.AddParameter("message", "A vérifié: " + type + Message);
            dbClient.AddParameter("pseudo", this.GetHabbo().Username);
            dbClient.RunQuery();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("INSERT INTO chatlogs_pub (user_id,user_name,timestamp,message) VALUES ('" + this.GetHabbo().Id + "',@pseudo,UNIX_TIMESTAMP(),@message)");
            dbClient.AddParameter("message", "Pub numero " + PubCount + ": " + type + Message);
            dbClient.AddParameter("pseudo", this.GetHabbo().Username);
            dbClient.RunQuery();
        }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT id,type,firstvalue,secondvalue FROM groups_items WHERE enabled = '1'");
        DataTable dItems = dbClient.GetTable();
    }

    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT enabled,current_preset,preset_one,preset_two,preset_three FROM room_items_moodlight WHERE item_id = '" + ItemId + "' LIMIT 1");
        Row = dbClient.GetRow();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("INSERT INTO room_items_moodlight (item_id,enabled,current_preset,preset_one,preset_two,preset_three) VALUES ('" + ItemId + "','0','1','#000000,255,0','#000000,255,0','#000000,255,0')");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT enabled,current_preset,preset_one,preset_two,preset_three FROM room_items_moodlight WHERE item_id=" + ItemId + " LIMIT 1");
        Row = dbClient.GetRow();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE room_items_moodlight SET enabled = 1 WHERE item_id = '" + this.ItemId + "' LIMIT 1");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE room_items_moodlight SET enabled = 0 WHERE item_id = '" + this.ItemId + "' LIMIT 1");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("UPDATE room_items_moodlight SET preset_" + Pr + " = '@color," + Intensity + "," + ButterflyEnvironment.BoolToEnum(BgOnly) + "' WHERE item_id = '" + this.ItemId + "' LIMIT 1");
        dbClient.AddParameter("color", Color);
        dbClient.RunQuery();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT id, sprite_id, item_name, type, width, length, stack_height, can_stack, is_walkable, can_sit, allow_recycle, allow_trade, allow_gift, allow_inventory_stack, interaction_type, interaction_modes_count, vending_ids, height_adjustable, effect_id, is_rare FROM furniture");
        DataTable ItemData = dbClient.GetTable();
    }
    
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("INSERT INTO items_limited VALUES (" + Item.Id + "," + LimitedNumber + "," + LimitedStack + ")");
    }
    
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("INSERT INTO items_limited VALUES (" + ItemId + "," + LimitedNumber + "," + LimitedStack + ")");
    }
    
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("INSERT INTO tele_links (tele_one_id, tele_two_id) VALUES (" + Item1Id + ", " + Item2Id + "), (" + Item2Id + ", " + Item1Id + ")");
        dbClient.RunQuery();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("INSERT INTO room_items_moodlight (item_id, enabled, current_preset, preset_one, preset_two, preset_three) VALUES (@id, '0', 1, @preset, @preset, @preset)");
        dbClient.AddParameter("id", Item.Id);
        dbClient.AddParameter("preset", "#000000,255,0");
        dbClient.RunQuery();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT tele_two_id FROM tele_links WHERE tele_one_id = '" + TeleId + "'");
        DataRow row = dbClient.GetRow();
    }
    
    internal static void Query8(IQueryAdapter dbClient)
    {
        DbClient.SetQuery("SELECT * from hotelview_promos WHERE hotelview_promos.enabled = '1' ORDER BY hotelview_promos.index ASC");
        DataTable dTable = DbClient.GetTable();
    }
   
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT * FROM navigator_categories ORDER BY id ASC");
        Table = dbClient.GetTable();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT room_id,image_url,enabled, langue, game FROM navigator_publics ORDER BY order_num ASC");
        DataTable GetPublics = dbClient.GetTable();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT id, category, series_number, goal_type, goal_data, name, reward, data_bit FROM quests");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE user_quests SET progress = '" + num + "' WHERE user_id = '" + Session.GetHabbo().Id + "' AND quest_id = '" + quest.Id + "'");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("REPLACE INTO user_quests VALUES (" + Session.GetHabbo().Id + ", " + quest.Id + ", 0)");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("REPLACE INTO user_quests VALUES (" + Session.GetHabbo().Id + ", " + nextQuestInSeries.Id + ", 0)");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("DELETE FROM user_quests WHERE user_id = '" + Session.GetHabbo().Id + "' AND quest_id = '" + quest.Id + "'");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT * FROM roleplay_enemy");
        DataTable table1 = dbClient.GetTable();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("INSERT INTO roleplay_enemy (id, type) VALUES ('" + BotId + "', 'bot')");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("INSERT INTO roleplay_enemy (id, type, weapon_far_id) VALUES ('" + PetId + "', 'pet', '0');");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("DELETE FROM roleplay_enemy WHERE id = '" + BotId + "'");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("DELETE FROM roleplay_enemy WHERE id = '" + PetId + "'");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT id, name, desc, price, type, value, allowstack, category FROM roleplay_items");
        DataTable table1 = dbClient.GetTable();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("DELETE FROM user_rpitems WHERE user_id = '" + this._id + "' AND rp_id = '" + this._rpId + "'");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("DELETE FROM user_rp WHERE user_id = '" + this._id + "' AND roleplay_id = '" + this._rpId + "'");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE user_rp SET health='" + this.Health + "', energy='" + this.Energy + "', hygiene='" + this.Hygiene + "', money='" + this.Money + "', money_1='" + this.Money1 + "', money_2='" + this.Money2 + "', money_3='" + this.Money3 + "', money_4='" + this.Money4 + "', munition='" + this.Munition + "', exp='" + this.Exp + "', weapon_far='" + this.WeaponGun.Id + "', weapon_cac='" + this.WeaponCac.Id + "' WHERE user_id='" + this._id + "' AND roleplay_id = '" + this._rpId + "' LIMIT 1");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT * FROM user_rpitems WHERE user_id = '" + this._id + "' AND rp_id = '" + this._rpId + "'");
        dbClient.AddParameter("userid", this._id);
        Table = dbClient.GetTable();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("INSERT INTO user_rpitems (user_id, rp_id, item_id, count) VALUES ('" + this._id + "', '" + this._rpId + "', '" + pItemId + "', '" + pCount + "')");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE user_rpitems SET count = count + '" + pCount + "' WHERE id = '" + Item.Id + "' LIMIT 1");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE user_rpitems SET count = count - '" + Count + "' WHERE id = '" + Item.Id + "' LIMIT 1");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("DELETE FROM user_rpitems WHERE id = '" + Item.Id + "' LIMIT 1");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE user_rp SET health='" + this.Health + "', energy='" + this.Energy + "', hygiene='" + this.Hygiene + "', money='" + this.Money + "', money_1='" + this.Money1 + "', money_2='" + this.Money2 + "', money_3='" + this.Money3 + "', money_4='" + this.Money4 + "', munition='" + this.Munition + "', exp='" + this.Exp + "', weapon_far='" + this.WeaponGun.Id + "', weapon_cac='" + this.WeaponCac.Id + "' WHERE user_id='" + this._id + "' AND roleplay_id = '" + this._rpId + "' LIMIT 1");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT * FROM user_rp WHERE user_id = '" + UserId + "' AND roleplay_id = '" + this._id + "'");
        DataRow dRow = dbClient.GetRow();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("INSERT INTO user_rp (user_id, roleplay_id) VALUES ('" + UserId + "', '" + this._id + "')");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT owner_id, hopital_id, prison_id FROM roleplay");
        DataTable table1 = dbClient.GetTable();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT * FROM roleplay_weapon");
        DataTable table1 = dbClient.GetTable();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT fuse, rank FROM fuserights");
        DataTable table1 = dbClient.GetTable();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT * FROM system_commands");
        table = dbClient.GetTable();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("INSERT INTO user_photos (user_id,photo,time) VALUES ('" + Session.GetHabbo().Id + "', @photoid, '" + Time + "');");
        dbClient.AddParameter("photoid", PhotoId);
        dbClient.RunQuery();
    }
    
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("SELECT item_id FROM catalog_items WHERE page_id IN (SELECT id FROM catalog_pages WHERE min_rank <= '" + Session.GetHabbo().Rank + "') AND cost_pixels = '0' AND cost_diamonds = '0' AND limited_sells = '0' AND limited_stack = '0' AND offer_active = '1' GROUP BY item_id");
    }
    
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("INSERT INTO room_items_moodlight (item_id, enabled, current_preset, preset_one, preset_two, preset_three)" +
            "SELECT '" + ItemId + "', enabled, current_preset, preset_one, preset_two, preset_three FROM room_items_moodlight WHERE item_id = '" + OldItemId + "'");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("INSERT INTO wired_items (trigger_id, trigger_data_2, trigger_data, all_user_triggerable, triggers_item) " +
                             "SELECT '" + ItemId + "', trigger_data_2, trigger_data, all_user_triggerable, triggers_item FROM wired_items WHERE trigger_id = '" + OldItemId + "'");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT tele_two_id FROM tele_links WHERE tele_one_id = '" + oldId + "'");
        DataRow rowTele = dbClient.GetRow();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("INSERT INTO tele_links (tele_one_id, tele_two_id) VALUES ('" + newId + "', '" + newIdTwo + "');");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT triggers_item FROM wired_items WHERE trigger_id = '" + id + "' AND triggers_item != ''");
        DataRow wiredRow = dbClient.GetRow();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("UPDATE wired_items SET triggers_item=@triggeritems WHERE trigger_id = '" + id + "' LIMIT 1");
        dbClient.AddParameter("triggeritems", triggerItems);
        dbClient.RunQuery();
    }
   
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("INSERT INTO bans (bantype,value,reason,expire,added_by,added_date) VALUES (@rawvar, @var, @reason, '" + expireTime + "', @mod, UNIX_TIMESTAMP())");
        dbClient.AddParameter("rawvar", "ignoreall");
        dbClient.AddParameter("var", clientByUsername.GetHabbo().Username);
        dbClient.AddParameter("reason", reason);
        dbClient.AddParameter("mod", Session.GetHabbo().Username);
        dbClient.RunQuery();
    }
    
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT look FROM user_wardrobe WHERE user_id IN (SELECT user_id FROM (SELECT user_id FROM user_wardrobe WHERE user_id >= ROUND(RAND() * (SELECT max(user_id) FROM user_wardrobe)) LIMIT 1) tmp) ORDER BY RAND() LIMIT 1");
        Session.GetHabbo().Look = dbClient.GetString();
    }
    
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT word FROM room_swearword_filter");
        DataTable Data = dbClient.GetTable();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT word FROM word_filter_retro");
        DataTable Data2 = dbClient.GetTable();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("INSERT INTO word_filter_retro (word) VALUES (@word)");
        dbClient.AddParameter("word", Word);
        dbClient.RunQuery();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT id, command FROM system_commands_pets");
        table = dbClient.GetTable();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT id, name, required_right FROM room_chat_styles");
        Table = dbClient.GetTable();
    }
 
    
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT id, door_x, door_y, door_z, door_dir, heightmap FROM room_models");
        DataTable table = dbClient.GetTable();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("INSERT INTO logs_trade (user_one_id, user_two_id, user_one_items, user_two_items, room_id, time) VALUES (@userone, @usertwo, @itemsone, @itemstwo, @roomid, UNIX_TIMESTAMP())");
        dbClient.AddParameter("userone", this.oneId);
        dbClient.AddParameter("usertwo", this.twoId);
        dbClient.AddParameter("itemsone", LogsOneString);
        dbClient.AddParameter("itemstwo", LogsTwoString);
        dbClient.AddParameter("roomid", this.RoomId);
        dbClient.RunQuery();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE roleplay_enemy SET health = '" + ParamInt + "' WHERE id = '" + RPEnemyConfig.Id + "'");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE roleplay_enemy SET weapon_far_id = '" + ParamInt + "' WHERE id = '" + RPEnemyConfig.Id + "'");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE roleplay_enemy SET weapon_cac_id = '" + ParamInt + "' WHERE id = '" + RPEnemyConfig.Id + "'");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE roleplay_enemy SET dead_timer = '" + ParamInt + "' WHERE id = '" + RPEnemyConfig.Id + "'");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE roleplay_enemy SET loot_item_id = '" + ParamInt + "' WHERE id = '" + RPEnemyConfig.Id + "'");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE roleplay_enemy SET money_drop = '" + ParamInt + "' WHERE id = '" + RPEnemyConfig.Id + "'");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE roleplay_enemy SET team_id = '" + ParamInt + "' WHERE id = '" + RPEnemyConfig.Id + "'");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE roleplay_enemy SET aggro_distance = '" + ParamInt + "' WHERE id = '" + RPEnemyConfig.Id + "'");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE roleplay_enemy SET zone_distance = '" + ParamInt + "' WHERE id = '" + RPEnemyConfig.Id + "'");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE roleplay_enemy SET reset_position = '" + ButterflyEnvironment.BoolToEnum(RPEnemyConfig.ResetPosition) + "' WHERE id = '" + RPEnemyConfig.Id + "'");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE roleplay_enemy SET lost_aggro_distance = '" + ParamInt + "' WHERE id = '" + RPEnemyConfig.Id + "'");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE roleplay_enemy SET zombie_mode = '" + ButterflyEnvironment.BoolToEnum(RPEnemyConfig.ZombieMode) + "' WHERE id = '" + RPEnemyConfig.Id + "'");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT id, caption FROM moderation_topics");
        ModerationTopics = dbClient.GetTable();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT type,message FROM moderation_presets WHERE enabled = '1'");
        DataTable table = dbClient.GetTable();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT * FROM moderation_resolution");
        DataTable table = dbClient.GetTable();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT * FROM moderation_tickets WHERE status = 'open'");
        DataTable table = dbClient.GetTable();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("INSERT INTO moderation_tickets (score,type,status,sender_id,reported_id,moderator_id,message,room_id,room_name,timestamp) VALUES (1,'" + Category + "','open','" + Session.GetHabbo().Id + "','" + ReportedUser + "','0',@message,'" + roomData.Id + "',@name,'" + ButterflyEnvironment.GetUnixTimestamp() + "')");
        dbClient.AddParameter("message", Message);
        dbClient.AddParameter("name", roomname);
        Id = Convert.ToInt32(dbClient.InsertQuery());
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("INSERT INTO cmdlogs (user_id, user_name, roomid, command, extra_data, timestamp) VALUES (@userid,@username,@roomid,@type,@desc, UNIX_TIMESTAMP())");
        dbClient.AddParameter("userid", user_id);
        dbClient.AddParameter("username", modName);
        dbClient.AddParameter("roomid", roomid);
        dbClient.AddParameter("target", target);
        dbClient.AddParameter("type", type);
        dbClient.AddParameter("desc", description + " " + target);
        dbClient.RunQuery();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE moderation_tickets SET status = 'picked', moderator_id = '" + pModeratorId + "', timestamp = '" + ButterflyEnvironment.GetUnixTimestamp() + "' WHERE id = '" + this.Id + "'");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE moderation_tickets SET status = '" + str + "' WHERE id = '" + this.Id + "'");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE moderation_tickets SET status = 'open' WHERE id = '" + this.Id + "'");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE moderation_tickets SET status = 'deleted' WHERE id = '" + this.Id + "'");
    }
    
    
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("DELETE FROM messenger_requests WHERE from_id = '" + this.UserId + "' OR to_id = '" + this.UserId + "'");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("DELETE FROM messenger_requests WHERE (from_id = '" + this.UserId + "' AND to_id = '" + sender + "') OR (to_id = '" + this.UserId + "' AND from_id = '" + sender + "')");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("REPLACE INTO messenger_friendships (user_one_id,user_two_id) VALUES ('" + this.UserId + "','" + friendID + "')");
        dbClient.RunQuery("REPLACE INTO messenger_friendships (user_one_id,user_two_id) VALUES ('" + friendID + "','" + this.UserId + "')");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("DELETE FROM messenger_friendships WHERE (user_one_id = '" + this.UserId + "' AND user_two_id = '" + friendID + "') OR (user_two_id = '" + this.UserId + "' AND user_one_id = '" + friendID + "')");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT user_one_id FROM messenger_friendships WHERE user_one_id = @myID AND user_two_id = @friendID");
        dbClient.AddParameter("myID", this.UserId);
        dbClient.AddParameter("friendID", requestID);
        return dbClient.FindsResult();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("REPLACE INTO messenger_requests (from_id,to_id) VALUES (" + this.UserId + "," + num2 + ")");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("INSERT INTO messenger_offline_messages (to_id, from_id, message, timestamp) VALUES (@tid, @fid, @msg, UNIX_TIMESTAMP())");
        dbClient.AddParameter("tid", ToId);
        dbClient.AddParameter("fid", this.GetClient().GetHabbo().Id);
        dbClient.AddParameter("msg", Message);
        dbClient.RunQuery();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT * FROM messenger_offline_messages WHERE to_id = @id");
        dbClient.AddParameter("id", this.UserId);
        GetMessages = dbClient.GetTable();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("DELETE FROM messenger_offline_messages WHERE to_id = @id");
        dbClient.AddParameter("id", this.UserId);
        dbClient.RunQuery();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT id FROM bans WHERE expire > @nowtime AND ((bantype = 'user' AND value = @username) OR (bantype = 'ip' AND value = @IP1) OR (bantype = 'ip' AND value = @IP2) OR (bantype = 'machine' AND value = @machineid)) LIMIT 1");
        dbClient.AddParameter("nowtime", ButterflyEnvironment.GetUnixTimestamp());
        dbClient.AddParameter("username", dUserInfo["username"]);
        dbClient.AddParameter("IP1", ip);
        dbClient.AddParameter("IP2", dUserInfo["ip_last"]);
        dbClient.AddParameter("machineid", machineid);
        DataRow IsBanned = dbClient.GetRow();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT expire FROM bans WHERE bantype = 'ignoreall' AND value = @username");
        dbClient.AddParameter("username", dUserInfo["username"]);
        DataRow IgnoreAll = dbClient.GetRow();
    }
    
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT group, level, progress FROM user_achievement WHERE user_id = '" + userId + "';");
        Achievement = dbClient.GetTable();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT * FROM user_quests WHERE user_id = '" + userId + "';");
        Quests = dbClient.GetTable();
    }
    
}
}
