using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.Game.Users.Messenger
{
    public class SearchResultFactory
    {
        public static List<SearchResult> GetSearchResult(string query)
        {
            List<SearchResult> list = new List<SearchResult>();

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DataTable table = UserDao.GetAllSearchUsers(dbClient, query);

                foreach (DataRow dataRow in table.Rows)
                {
                    list.Add(new SearchResult(Convert.ToInt32(dataRow["id"]), (string)dataRow["username"], (string)dataRow["motto"], (string)dataRow["last_online"], (string)dataRow["look"]));
                }
            }

            return list;
        }
    }
}