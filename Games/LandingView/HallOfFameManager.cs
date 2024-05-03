namespace WibboEmulator.Games.LandingView;
using System.Data;
using System.Diagnostics;
using WibboEmulator.Core.Settings;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Emulator;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Users;

public static class HallOfFameManager
{
    private static readonly int MAX_USERS = 10;
    private static DateTime _lastUpdate = DateTime.UnixEpoch.AddSeconds(SettingsManager.GetData<int>("hof.lastupdate"));
    public static List<User> UserRanking { get; private set; } = [];

    public static void Initialize(IDbConnection dbClient)
    {
        HofStopwatch.Start();

        UserRanking.Clear();

        var userIds = UserDao.GetTop10ByGamePointMonth(dbClient);

        foreach (var userId in userIds)
        {
            var user = WibboEnvironment.GetUserById(userId);

            if (user != null)
            {
                UserRanking.Add(user);
            }
        }
    }

    private static readonly Stopwatch HofStopwatch = new();
    public static void OnCycle()
    {
        if (HofStopwatch.ElapsedMilliseconds >= 60000)
        {
            HofStopwatch.Restart();

            if (_lastUpdate.Month == DateTime.UtcNow.Month)
            {
                return;
            }

            _lastUpdate = DateTime.UtcNow;

            foreach (var client in GameClientManager.Clients.ToList())
            {
                if (client == null || client.User == null)
                {
                    continue;
                }

                client.User.GamePointsMonth = 0;
            }

            UserRanking.Clear();

            var dbClient = DatabaseManager.Connection;
            EmulatorSettingDao.Update(dbClient, "hof.lastupdate", WibboEnvironment.GetUnixTimestamp().ToString());
        }
    }

    public static void UpdateRakings(User user)
    {
        if (user == null || user.Rank >= 6)
        {
            return;
        }

        if (UserRanking.Contains(user))
        {
            UserRanking.Remove(user);
        }

        UserRanking.Add(user);

        UserRanking = UserRanking.OrderByDescending(x => x.GamePointsMonth).Take(MAX_USERS).ToList();
    }
}
