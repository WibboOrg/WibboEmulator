using Butterfly.Database.Daos;
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
                table = UserDao.GetAllSearchUsers(dbClient, query);
            }

            foreach (DataRow dataRow in table.Rows)
            {
                list.Add(new SearchResult(Convert.ToInt32(dataRow["id"]), (string)dataRow["username"], (string)dataRow["look"]));
            }

            return list;
        }
    }
}
