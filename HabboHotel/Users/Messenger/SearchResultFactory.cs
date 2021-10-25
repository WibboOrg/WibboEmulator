using Butterfly.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.HabboHotel.Users.Messenger
{
    public class SearchResultFactory
    {
        public static List<SearchResult> GetSearchResult(string query)
        {
            List<SearchResult> list = new List<SearchResult>();
            DataTable table;
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT id, username, look FROM users WHERE username LIKE @query LIMIT 50");
                dbClient.AddParameter("query", (query.Replace("%", "\\%").Replace("_", "\\_") + "%"));
                table = dbClient.GetTable();
            }

            foreach (DataRow dataRow in table.Rows)
            {
                list.Add(new SearchResult(Convert.ToInt32(dataRow["id"]), (string)dataRow["username"], (string)dataRow["look"]));
            }

            return list;
        }
    }
}
