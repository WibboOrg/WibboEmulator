using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using System;
using System.Diagnostics;

namespace Butterfly.Core
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

            Console.Title = "Butterfly Emulator - start-up in progress...";
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
                    int UsersOnline = ButterflyEnvironment.GetGame().GetClientManager().Count;

                    ButterflyEnvironment.GetGame().GetAnimationManager().OnUpdateUsersOnline(UsersOnline);

                    if (UsersOnline > UserPeak)
                    {
                        UserPeak = UsersOnline;
                    }

                    int RoomsLoaded = ButterflyEnvironment.GetGame().GetRoomManager().Count;

                    TimeSpan Uptime = DateTime.Now - ButterflyEnvironment.ServerStarted;

                    Console.Title = "Butterfly | Démarré depuis : " + Uptime.Days + " jour(s) " + Uptime.Hours + " heures " + Uptime.Minutes + " minutes | "
                         + UsersOnline + " Joueur(s) en ligne " + " | " + RoomsLoaded + " Appartement(s) en ligne";

                    using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        EmulatorStatsDao.Insert(dbClient, UsersOnline, RoomsLoaded);
                        EmulatorStatusDao.UpdateScore(dbClient, UsersOnline, RoomsLoaded, UserPeak);
                    }
                }
                catch (Exception e) { ExceptionLogger.LogThreadException(e.ToString(), "Server status update task"); }
            }
        }
    }
}