using Butterfly.Database.Interfaces;
using System;
using System.Diagnostics;

namespace Butterfly.Core
{
    public class LowPriorityWorker
    {
        private static int UserPeak;
        private static bool isExecuted = false;

        private static string ColdTitle;

        public static void Init()
        {
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT userpeak FROM server_status");
                UserPeak = dbClient.GetInteger();
            }

            ColdTitle = string.Empty;

            lowPriorityProcessWatch = new Stopwatch();
            lowPriorityProcessWatch.Start();
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

                    Console.Title = "Butterfly | Démarré depuis : " + Uptime.Days + " jours " + Uptime.Hours + " heures " + Uptime.Minutes + " minutes | "
                         + UsersOnline + " Joueurs en ligne " + " | " + RoomsLoaded + " Appartement en ligne";

                    using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("UPDATE server_status SET users_online = '" + UsersOnline + "', rooms_loaded = '" + RoomsLoaded + "', userpeak = '" + UserPeak + "', stamp = UNIX_TIMESTAMP();INSERT INTO system_stats (online, time, room) VALUES ('" + UsersOnline + "', UNIX_TIMESTAMP(), '" + RoomsLoaded + "')");
                    }
                }
                catch (Exception e) { Logging.LogThreadException(e.ToString(), "Server status update task"); }
            }
        }
    }
}