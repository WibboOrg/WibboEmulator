namespace WibboEmulator.Core;
using System.Diagnostics;
using WibboEmulator.Database.Daos.Emulator;
using WibboEmulator.Database.Interfaces;

public class ServerStatusUpdater
{
    private static int _userPeak;
    private static bool _isExecuted;

    public static void Init(IQueryAdapter dbClient)
    {
        _userPeak = EmulatorStatusDao.GetUserpeak(dbClient);

        _lowPriorityProcessWatch = new Stopwatch();
        _lowPriorityProcessWatch.Start();

        Console.WriteLine("Server Status Updater has been started.");
    }


    private static Stopwatch _lowPriorityProcessWatch;
    public static void Process()
    {
        if (_lowPriorityProcessWatch.ElapsedMilliseconds >= 60000 || !_isExecuted)
        {
            _isExecuted = true;
            _lowPriorityProcessWatch.Restart();
            try
            {
                var usersOnline = WibboEnvironment.GetGame().GetGameClientManager().Count;

                WibboEnvironment.GetGame().GetAnimationManager().OnUpdateUsersOnline(usersOnline);

                if (usersOnline > _userPeak)
                {
                    _userPeak = usersOnline;
                }

                var roomsLoaded = WibboEnvironment.GetGame().GetRoomManager().Count;

                var uptime = DateTime.Now - WibboEnvironment.ServerStarted;

                using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
                EmulatorStatsDao.Insert(dbClient, usersOnline, roomsLoaded);
                EmulatorStatusDao.UpdateScore(dbClient, usersOnline, roomsLoaded, _userPeak);
            }
            catch (Exception e) { ExceptionLogger.LogThreadException(e.ToString(), "Server status update task"); }
        }
    }
}
