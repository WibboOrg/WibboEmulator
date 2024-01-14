namespace WibboEmulator.Games.Users.Messenger;

using WibboEmulator.Database.Daos.User;

public class SearchResultFactory
{
    public static List<SearchResult> GetSearchResult(string query)
    {
        var list = new List<SearchResult>();

        using (var dbClient = WibboEnvironment.GetDatabaseManager().Connection())
        {
            var userList = UserDao.GetAllSearchUsers(dbClient, query);

            foreach (var user in userList)
            {
                list.Add(new SearchResult(user.Id, user.Username, user.Motto, user.LastOnline, user.Look));
            }
        }

        return list;
    }
}
