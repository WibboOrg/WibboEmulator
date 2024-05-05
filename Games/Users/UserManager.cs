namespace WibboEmulator.Games.Users;

using System.Collections.Concurrent;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Users.Authentificator;
using WibboEmulator.Utilities;

public static class UserManager
{
    private static readonly ConcurrentDictionary<int, User> UsersCached = new();

    public static int UsernameAvailable(string username)
    {
        if (username.Length is < 3 or > 15 || !username.IsValidAlphaNumeric())
        {
            return -1;
        }

        return UsernameExists(username) ? 0 : 1;
    }

    public static bool UsernameExists(string username)
    {
        using var dbClient = DatabaseManager.Connection;
        var integer = UserDao.GetIdByName(dbClient, username);
        return integer > 0;
    }

    public static string GetUsernameById(int id)
    {
        var user = GetUserById(id);
        if (user != null)
        {
            return user.Username;
        }

        using var dbClient = DatabaseManager.Connection;
        return UserDao.GetNameById(dbClient, id);
    }

    public static User GetUserById(int userId)
    {
        if (userId <= 0)
        {
            return null;
        }

        var client = GameClientManager.GetClientByUserID(userId);
        if (client != null)
        {
            var user = client.User;
            if (user != null && user.Id > 0)
            {
                _ = UsersCached.TryRemove(userId, out _);
                return user;
            }
        }
        else
        {
            if (UsersCached.TryGetValue(userId, out var cachedUser))
            {
                return cachedUser;
            }
            else
            {
                using var dbClient = DatabaseManager.Connection;
                var user = UserFactory.GetUserData(dbClient, userId);

                if (user != null)
                {
                    user.InitializeProfile(dbClient);
                    _ = UsersCached.TryAdd(userId, user);
                    return user;
                }
            }
        }

        return null;
    }
}
