namespace WibboEmulator.Games.Users.Messenger;
using System.Data;
using WibboEmulator.Database.Daos;

public class SearchResultFactory
{
    public static List<SearchResult> GetSearchResult(string query)
    {
        var list = new List<SearchResult>();

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            var table = UserDao.GetAllSearchUsers(dbClient, query);

            foreach (DataRow dataRow in table.Rows)
            {
                list.Add(new SearchResult(Convert.ToInt32(dataRow["id"]), (string)dataRow["username"], (string)dataRow["motto"], Convert.ToInt32(dataRow["last_online"]), (string)dataRow["look"]));
            }
        }

        return list;
    }
}
