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
            using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("SELECT id, username, look FROM users WHERE username LIKE @query LIMIT 50");
                queryreactor.AddParameter("query", (query.Replace("%", "\\%").Replace("_", "\\_") + "%"));
                table = queryreactor.GetTable();
            }

            foreach (DataRow dataRow in table.Rows)
            {
                list.Add(new SearchResult(Convert.ToInt32(dataRow["id"]), (string)dataRow["username"], (string)dataRow["look"]));
            }

            return list;
        }
    }
}
