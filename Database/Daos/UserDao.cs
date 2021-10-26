using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class UserDao
    {
        internal static int GetIdByName(IQueryAdapter dbClient, string name)
        {
            dbClient.SetQuery("SELECT `id` FROM `users` WHERE `username` = @username LIMIT 1");
            dbClient.AddParameter("username", name);

            return dbClient.GetInteger();
        }

        internal static string GetNameById(IQueryAdapter dbClient, int userId)
        {
            dbClient.SetQuery("SELECT `username` FROM `users` WHERE `id` = @id LIMIT 1");
            dbClient.AddParameter("id", userId);

            return dbClient.GetString();
        }

        internal static void UpdateWP(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE users SET vip_points = vip_points - " + TotalDiamondCost + " WHERE id = '" + Session.GetHabbo().Id + "'");
        }

         internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE users SET vip_points = vip_points - '" + Convert.ToInt32(Row["total_price"]) + "' WHERE id = '" + Session.GetHabbo().Id + "'");
        }

        
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE users SET vip_points = vip_points + '" + CreditsOwed + "' WHERE id = '" + Session.GetHabbo().Id + "'");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE users SET home_room = '" + RoomId + "' WHERE id = '" + Session.GetHabbo().Id + "'");
        }

        
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("UPDATE users SET motto = @motto WHERE id = '" + Session.GetHabbo().Id + "'");
            dbClient.AddParameter("motto", newMotto);
            dbClient.RunQuery();
        }

        
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE users SET vip_points = vip_points + '" + Value + "' WHERE id = '" + Session.GetHabbo().Id + "' LIMIT 1");
        }

        
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE users SET home_room = '0' WHERE id = '" + Session.GetHabbo().Id + "'");
        }
   
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE users SET volume = '" + Volume1 + "," + Volume2 + "," + Volume3 + "' WHERE id = '" + Session.GetHabbo().Id + "' LIMIT 1");
        }
      
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("UPDATE users SET username = @newname WHERE id = @userid");
            dbClient.AddParameter("newname", NewUsername);
            dbClient.AddParameter("userid", Session.GetHabbo().Id);
            dbClient.RunQuery();
        }

        
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("UPDATE users SET look = @look, gender = @gender WHERE id = '" + Session.GetHabbo().Id + "'");
            dbClient.AddParameter("look", Look);
            dbClient.AddParameter("gender", Gender);
            dbClient.RunQuery();
        }

         internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT credits FROM users WHERE id = @userid");
            dbClient.AddParameter("userid", Client.GetHabbo().Id);
            row = dbClient.GetRow();
        }

        
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE users SET online = '0' WHERE online = '1'");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE users SET auth_ticket = '' WHERE auth_ticket != ''");
        }

         internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("UPDATE users SET machine_id = @machineid WHERE id = '" + this._habbo.Id + "'");
            dbClient.AddParameter("machineid", (this.MachineId != null) ? this.MachineId : "");
            dbClient.RunQuery();
        }
       
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE users SET nux_enable = '0', home_room = '" + RoomId + "' WHERE id = '" + this.GetHabbo().Id + "'");
        }

        
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT username FROM users WHERE id = '" + Id + "'");
            username = dbClient.GetString();
        }

        
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("UPDATE users SET look = @look WHERE id = '" + Session.GetHabbo().Id + "'");
        dbClient.AddParameter("look", str3);
        dbClient.RunQuery();
    }

    
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE users SET game_points = game_points + 1, game_points_month = game_points_month + 1 WHERE id = '" + roomUserByHabbo.GetClient().GetHabbo().Id + "';");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("UPDATE users SET motto = @motto WHERE id = '" + clientByUsername.GetHabbo().Id + "'");
        dbClient.AddParameter("motto", clientByUsername.GetHabbo().Motto);
        dbClient.RunQuery();
    }

    
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE users SET mazoscore = '" + Habbo.MazoHighScore + "' WHERE id = '" + Habbo.Id + "'");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE users SET mazo = '" + Habbo.Mazo + "' WHERE id = '" + Habbo.Id + "'");
    }

    
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE users SET vip_points = vip_points - '" + Room.RoomData.SellPrice + "' WHERE id = '" + Session.GetHabbo().Id + "'");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE users SET vip_points = vip_points + '" + Room.RoomData.SellPrice + "' WHERE id = '" + Room.RoomData.OwnerId + "'");
    }
    
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT id, langue FROM users WHERE username = @owner");
        dbClient.AddParameter("owner", this.OwnerName);
        DataRow UserRow = dbClient.GetRow();
    }
    
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE users SET run_points = run_points + 1, run_points_month = run_points_month + 1 WHERE id = '" + User.GetClient().GetHabbo().Id + "'");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE users SET game_points = game_points + 1, game_points_month = game_points_month + 1 WHERE id = '" + User.GetClient().GetHabbo().Id + "'");
    }
    
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT id FROM users WHERE username = @owner");
        dbClient.AddParameter("owner", Data.OwnerName);
        i = Convert.ToInt32(dbClient.GetRow()[0]);
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT id, username FROM users WHERE id = '" + UserId + "'");
        row1 = dbClient.GetRow();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT username FROM users WHERE id = '" + Id + "'");
        username = dbClient.GetString();
    }
    
    
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE users SET online = '0', last_online = '" + ButterflyEnvironment.GetUnixTimestamp() + "', activity_points = '" + this.Duckets + "', credits = '" + this.Credits + "' WHERE id = '" + this.Id + "'");
    }
    
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT username FROM users WHERE id = '" + friendID + "'");
        row = dbClient.GetRow();
    }

    
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT id, block_newfriends FROM users WHERE username = @query");
        dbClient.AddParameter("query", UserQuery.ToLower());
        dataRow = dbClient.GetRow();
    }
    
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT id, username, look FROM users WHERE username LIKE @query LIMIT 50");
        dbClient.AddParameter("query", (query.Replace("%", "\\%").Replace("_", "\\_") + "%"));
        table = dbClient.GetTable();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT * FROM users WHERE auth_ticket = @sso LIMIT 1");
        dbClient.AddParameter("sso", sessionTicket);
        dUserInfo = dbClient.GetRow();
    }
    
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE users SET lastdailycredits = '" + lastDaily + "' WHERE id = '" + userId + "'");
    }
    
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE users SET online = '1', auth_ticket = ''  WHERE id = '" + userId + "'");
    }

    
    
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT users.id,users.username,messenger_friendships.relation FROM users JOIN messenger_friendships ON users.id = messenger_friendships.user_two_id WHERE messenger_friendships.user_one_id = '" + userId + "'");
        FrienShips = dbClient.GetTable();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT messenger_requests.from_id,messenger_requests.to_id,users.username FROM users JOIN messenger_requests ON users.id = messenger_requests.from_id WHERE messenger_requests.to_id = '" + userId + "'");
        Requests = dbClient.GetTable();
    }
    
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT * FROM users WHERE id = @id LIMIT 1");
        dbClient.AddParameter("id", UserId);
        row = dbClient.GetRow();
    }
    
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT users.id, messenger_friendships.relation FROM users JOIN messenger_friendships ON users.id = messenger_friendships.user_two_id WHERE messenger_friendships.user_one_id = '" + UserId + "' AND messenger_friendships.relation != '0'");
        FrienShips = dbClient.GetTable();
    }
    
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT volume FROM users WHERE id = '" + this.UserId + "'");
        DataRow dUserVolume = dbClient.GetRow();
    }
    }
}
