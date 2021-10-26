  internal static void Query3(IQueryAdapter dbClient, int groupId)
        {
            dbClient.RunQuery("DELETE FROM group_requests WHERE group_id = '" + groupId + "'");
        }
        
        internal static DataTable Query6(IQueryAdapter dbClient, int groupeId, string searchVal)
        {
            dbClient.SetQuery("SELECT users.id FROM group_requests INNER JOIN users ON group_requests.user_id = users.id WHERE group_requests.group_id = @gid AND users.username LIKE @username LIMIT 14;");
            dbClient.AddParameter("gid", groupeId);
            dbClient.AddParameter("username", searchVal.Replace("%", "\\%").Replace("_", "\\_") + "%");

            return dbClient.GetTable();
        }

        internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("INSERT INTO group_requests (user_id, group_id) VALUES (@uid, @gid)");
        dbClient.AddParameter("gid", this.Id);
        dbClient.AddParameter("uid", Id);
        dbClient.RunQuery();
    }
    
    internal static void Query8(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("DELETE FROM group_requests WHERE user_id=@uid AND group_id=@gid LIMIT 1");
        dbClient.AddParameter("gid", this.Id);
        dbClient.AddParameter("uid", Id);
        dbClient.RunQuery();
    }