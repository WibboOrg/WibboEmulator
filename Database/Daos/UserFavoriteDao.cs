
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("INSERT INTO user_favorites (user_id,room_id) VALUES ('" + Session.GetHabbo().Id + "','" + num + "')");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE FROM user_favorites WHERE user_id = '" + Session.GetHabbo().Id + "' AND room_id = '" + RoomId + "'");
        }
        
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE FROM user_favorites WHERE room_id = '" + RoomId + "'");
        }
        internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT room_id FROM user_favorites WHERE user_id = '" + userId + "';");
        Favorites = dbClient.GetTable();
    }