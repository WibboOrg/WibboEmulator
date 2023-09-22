namespace WibboEmulator.Games.LandingView;
using System.Data;
using System.Diagnostics;
using WibboEmulator.Database.Daos.Emulator;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Users;

public class HallOfFameManager
{
    private static readonly int MAX_USERS = 10;
    private DateTime _lastUpdate;
    public List<User> UserRanking { get; private set; }

    public HallOfFameManager()
    {
        this.UserRanking = new List<User>();

        this._lastUpdate = DateTime.UnixEpoch.AddSeconds(Convert.ToInt32(WibboEnvironment.GetSettings().GetData<int>("hof.lastupdate")));
        this._hofStopwatch = new();
        this._hofStopwatch.Start();
    }

    public void Init(IQueryAdapter dbClient)
    {
        this.UserRanking.Clear();

        var dTable = UserDao.GetTop10ByGamePointMonth(dbClient);

        foreach (DataRow dRow in dTable.Rows)
        {
            var userId = Convert.ToInt32(dRow["id"]);
            var user = WibboEnvironment.GetUserById(userId);

            if (user != null)
            {
                this.UserRanking.Add(user);
            }
        }
    }

    private readonly Stopwatch _hofStopwatch;
    public void OnCycle()
    {
        if (this._hofStopwatch.ElapsedMilliseconds >= 60000)
        {
            this._hofStopwatch.Restart();

            if (this._lastUpdate.Month == DateTime.UtcNow.Month)
            {
                return;
            }

            this._lastUpdate = DateTime.UtcNow;

            foreach (var client in WibboEnvironment.GetGame().GetGameClientManager().GetClients.ToList())
            {
                if (client == null || client.User == null)
                {
                    continue;
                }

                client.User.GamePointsMonth = 0;
            }

            this.UserRanking.Clear();

            var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
            EmulatorSettingDao.Update(dbClient, "hof.lastupdate", WibboEnvironment.GetUnixTimestamp().ToString());
        }
    }

    public void UpdateRakings(User user)
    {
        if (user == null || user.Rank >= 6)
        {
            return;
        }

        if (this.UserRanking.Contains(user))
        {
            this.UserRanking.Remove(user);
        }

        this.UserRanking.Add(user);

        this.UserRanking = this.UserRanking.OrderByDescending(x => x.GamePointsMonth).Take(MAX_USERS).ToList();
    }
}
