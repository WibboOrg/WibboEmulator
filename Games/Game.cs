namespace WibboEmulator.Games;
using System.Data;
using System.Diagnostics;
using WibboEmulator.Communication.Packets;
using WibboEmulator.Core;
using WibboEmulator.Database.Daos.Emulator;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.Achievements;
using WibboEmulator.Games.Animations;
using WibboEmulator.Games.Badges;
using WibboEmulator.Games.Banners;
using WibboEmulator.Games.Catalogs;
using WibboEmulator.Games.Chats;
using WibboEmulator.Games.Effects;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.LandingView;
using WibboEmulator.Games.Loots;
using WibboEmulator.Games.Moderations;
using WibboEmulator.Games.Navigators;
using WibboEmulator.Games.Permissions;
using WibboEmulator.Games.Quests;
using WibboEmulator.Games.Roleplays;
using WibboEmulator.Games.Rooms;

public static class Game
{
    private static Thread _gameLoop;
    private static readonly int CycleSleepTime = 25;
    private static bool _cycleEnded;
    private static bool _gameLoopActive;

    private static readonly Stopwatch ModuleWatch = new();

    public static void Initialize(IDbConnection dbClient)
    {
        DatabaseCleanup(dbClient);

        GameClientManager.Initialize();
        PermissionManager.Initialize(dbClient);
        ItemManager.Initialize(dbClient);
        CatalogManager.Initialize(dbClient);
        NavigatorManager.Initialize(dbClient);
        RoleplayManager.Initialize(dbClient);
        RoomManager.Initialize(dbClient);
        GroupManager.Initialize(dbClient);
        ModerationManager.Initialize(dbClient);
        QuestManager.Initialize(dbClient);
        LandingViewManager.Initialize(dbClient);
        HallOfFameManager.Initialize(dbClient);
        ChatManager.Initialize(dbClient);
        EffectManager.Initialize(dbClient);
        BadgeManager.Initialize(dbClient);
        AchievementManager.Initialize(dbClient);
        AnimationManager.Initialize(dbClient);
        LootManager.Initialize(dbClient);
        BannerManager.Initialize(dbClient);
        EconomyCenterManager.Initialize(dbClient);
        PacketManager.Initialize();

        ServerStatusUpdater.Initialize(dbClient);
    }

    public static void StartGameLoop()
    {
        _gameLoopActive = true;

        var receiver = new ThreadStart(MainGameLoop);
        _gameLoop = new Thread(receiver)
        {
            IsBackground = true
        };

        _gameLoop.Start();
    }

    private static void MainGameLoop()
    {
        while (_gameLoopActive)
        {
            _cycleEnded = false;

            try
            {
                ModuleWatch.Restart();

                ServerStatusUpdater.Process();

                if (ModuleWatch.ElapsedMilliseconds > 500)
                {
                    Console.WriteLine("High latency in LowPriorityWorker.Process ({0} ms)", ModuleWatch.ElapsedMilliseconds);
                }

                ModuleWatch.Restart();

                RoomManager.OnCycle(ModuleWatch);

                if (ModuleWatch.ElapsedMilliseconds > 500)
                {
                    Console.WriteLine("High latency in RoomManager ({0} ms)", ModuleWatch.ElapsedMilliseconds);
                }

                ModuleWatch.Restart();

                GameClientManager.OnCycle();

                if (ModuleWatch.ElapsedMilliseconds > 500)
                {
                    Console.WriteLine("High latency in GameClientManager ({0} ms)", ModuleWatch.ElapsedMilliseconds);
                }

                ModuleWatch.Restart();

                HallOfFameManager.OnCycle();

                if (ModuleWatch.ElapsedMilliseconds > 500)
                {
                    Console.WriteLine("High latency in HallOfFame ({0} ms)", ModuleWatch.ElapsedMilliseconds);
                }

                ModuleWatch.Restart();

                AnimationManager.OnCycle(ModuleWatch);

                if (ModuleWatch.ElapsedMilliseconds > 500)
                {
                    Console.WriteLine("High latency in AnimationManager ({0} ms)", ModuleWatch.ElapsedMilliseconds);
                }

                ModuleWatch.Restart();
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine("Canceled operation {0}", e);
            }
            catch (Exception e)
            {
                ExceptionLogger.LogThreadException(e.ToString(), "MainThread");
            }

            _cycleEnded = true;

            Thread.Sleep(CycleSleepTime);
        }

        Console.WriteLine("MainGameLoop end");
    }

    public static void DatabaseCleanup(IDbConnection dbClient)
    {
        if (!Debugger.IsAttached)
        {
            UserDao.UpdateAllOnline(dbClient);
            UserDao.UpdateAllTicket(dbClient);
            RoomDao.UpdateResetUsersNow(dbClient);
            EmulatorStatusDao.UpdateReset(dbClient);
        }
    }

    public static void StopGameLoop()
    {
        _gameLoopActive = false;
        while (!_cycleEnded)
        {
            Thread.Sleep(CycleSleepTime);
        }
    }
}
