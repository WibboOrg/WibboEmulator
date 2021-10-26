
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("INSERT INTO room_rights (room_id, user_id) VALUES ('" + room.Id + "', '" + UserId + "')");
        }
       
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE FROM room_rights WHERE room_id = '" + room.Id + "'");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("DELETE FROM room_rights WHERE user_id = @uid AND room_id = @rid LIMIT 1");
            dbClient.AddParameter("uid", Session.GetHabbo().Id);
            dbClient.AddParameter("rid", Room.Id);
            dbClient.RunQuery();
        }
        
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE FROM room_rights WHERE room_id = '" + RoomId + "'");
        }
        
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("DELETE FROM room_rights WHERE room_id = '" + Room.Id + "'");
    }
    
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT room_rights.user_id FROM room_rights WHERE room_id = '" + this.RoomData.Id + "'");
        dataTable = dbClient.GetTable();
    }

    
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT room_id FROM room_rights WHERE user_id = '" + userId + "';");
        RoomRights = dbClient.GetTable();
    }