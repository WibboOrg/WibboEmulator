namespace Butterfly.Database.Daos
{
    class UserStatsDao
    {
        internal static void Query5(IQueryAdapter dbClient, int groupId)
        {
            dbClient.RunQuery("UPDATE user_stats SET group_id = '0' WHERE group_id = '" + groupId + "' LIMIT 1");
        }

        
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("UPDATE user_stats SET group_id = '0' WHERE id = @userId LIMIT 1");
            dbClient.AddParameter("userId", UserId);
            dbClient.RunQuery();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("UPDATE user_stats SET group_id = @groupId WHERE id = @userId LIMIT 1");
            dbClient.AddParameter("groupId", Session.GetHabbo().FavouriteGroupId);
            dbClient.AddParameter("userId", Session.GetHabbo().Id);
            dbClient.RunQuery();
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE user_stats SET achievement_score = achievement_score + '" + Value + "' WHERE id = '" + Session.GetHabbo().Id + "'");
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE user_stats SET achievement_score = achievement_score + '" + TargetLevelData.RewardPoints + "' WHERE id = '" + Session.GetHabbo().Id + "'");
        }

        
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE user_stats SET achievement_score = achievement_score + '" + WinWin + "' WHERE id = '" + Session.GetHabbo().Id + "'");
    }

    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE user_stats SET group_id = '" + this.FavouriteGroupId + "',  online_time = online_time + '" + TimeOnlineSec + "', quest_id = '" + this.CurrentQuestId + "', Respect = '" + this.Respect + "', daily_respect_points = '" + this.DailyRespectPoints + "', daily_pet_respect_points = '" + this.DailyPetRespectPoints + "' WHERE id = '" + this.Id + "'");
    }

    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE user_stats SET daily_respect_points = 5, daily_pet_respect_points = 5 WHERE id = '" + userId + "' LIMIT 1");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("UPDATE user_stats SET daily_respect_points = 20, daily_pet_respect_points = 20 WHERE id = '" + userId + "' LIMIT 1");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT * FROM user_stats WHERE id = '" + userId + "';");
        row2 = dbClient.GetRow();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("INSERT INTO user_stats (id) VALUES ('" + userId + "')");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT * FROM user_stats WHERE id =  '" + userId + "';");
        row2 = dbClient.GetRow();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT * FROM user_stats WHERE id = @id");
        dbClient.AddParameter("id", UserId);
        row2 = dbClient.GetRow();
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.RunQuery("INSERT INTO user_stats (id) VALUES ('" + UserId + "')");
    }
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT * FROM user_stats WHERE id = " + UserId);
        row2 = dbClient.GetRow();
    }
    }
}
