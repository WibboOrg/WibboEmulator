using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using System.Diagnostics;

namespace WibboEmulator.Core
{
    public class ServerStatusUpdater
    {
        private static int UserPeak;
        private static bool isExecuted = false;

        public static void Init(IQueryAdapter dbClient)
        {
            UserPeak = EmulatorStatusDao.GetUserpeak(dbClient);

            lowPriorityProcessWatch = new Stopwatch();
            lowPriorityProcessWatch.Start();

            Console.WriteLine("Server Status Updater has been started.");
        }


        private static Stopwatch lowPriorityProcessWatch;
        public static void Process()
        {
            if (lowPriorityProcessWatch.ElapsedMilliseconds >= 60000 || !isExecuted)
            {
                isExecuted = true;
                lowPriorityProcessWatch.Restart();
                try
                {
                    int UsersOnline = WibboEnvironment.GetGame().GetGameClientManager().Count;

                    WibboEnvironment.GetGame().GetAnimationManager().OnUpdateUsersOnline(UsersOnline);

                    if (UsersOnline > UserPeak)
                    {
                        UserPeak = UsersOnline;
                    }

                    int RoomsLoaded = WibboEnvironment.GetGame().GetRoomManager().Count;

                    TimeSpan Uptime = DateTime.Now - WibboEnvironment.ServerStarted;

                    //Console.Title = "Butterfly | Démarré depuis : " + Uptime.Days + " jour(s) " + Uptime.Hours + " heures " + Uptime.Minutes + " minutes | "
                    //+ UsersOnline + " Joueur(s) en ligne " + " | " + RoomsLoaded + " Appartement(s) en ligne";

                    using IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
                    EmulatorStatsDao.Insert(dbClient, UsersOnline, RoomsLoaded);
                    EmulatorStatusDao.UpdateScore(dbClient, UsersOnline, RoomsLoaded, UserPeak);
                }
                catch (Exception e) { ExceptionLogger.LogThreadException(e.ToString(), "Server status update task"); }
            }
        }
    }
}