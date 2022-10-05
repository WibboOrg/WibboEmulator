using System.Data;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;

namespace WibboEmulator.Games.GameClients.Messenger
{
    public class SearchResultFactory
    {
        public static List<SearchResult> GetSearchResult(string query)
        {
            List<SearchResult> list = new List<SearchResult>();

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DataTable table = UserDao.GetAllSearchUsers(dbClient, query);

                foreach (DataRow dataRow in table.Rows)
                {
                    list.Add(new SearchResult(Convert.ToInt32(dataRow["id"]), (string)dataRow["username"], (string)dataRow["motto"], Convert.ToInt32(dataRow["last_online"]), (string)dataRow["look"]));
                }
            }

            return list;
        }
    }
}