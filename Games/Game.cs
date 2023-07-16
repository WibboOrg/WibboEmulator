namespace WibboEmulator.Games;
using System.Diagnostics;
using WibboEmulator.Communication.Packets;
using WibboEmulator.Core;
using WibboEmulator.Database.Daos.Emulator;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Achievements;
using WibboEmulator.Games.Animations;
using WibboEmulator.Games.Badges;
using WibboEmulator.Games.Banners;
using WibboEmulator.Games.Catalogs;
using WibboEmulator.Games.Chats;
using WibboEmulator.Games.Effects;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;
using WibboEmulator.Games.Helps;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.LandingView;
using WibboEmulator.Games.Loots;
using WibboEmulator.Games.Moderations;
using WibboEmulator.Games.Navigators;
using WibboEmulator.Games.Permissions;
using WibboEmulator.Games.Quests;
using WibboEmulator.Games.Roleplays;
using WibboEmulator.Games.Rooms;

public class Game : IDisposable
{
    private readonly GameClientManager _gameClientManager;
    private readonly PermissionManager _permissionManager;
    private readonly CatalogManager _catalogManager;
    private readonly NavigatorManager _navigatorManager;
    private readonly ItemDataManager _itemDataManager;
    private readonly RoomManager _roomManager;
    private readonly AchievementManager _achievementManager;
    private readonly ModerationManager _moderationManager;
    private readonly QuestManager _questManager;
    private readonly GroupManager _groupManager;
    private readonly LandingViewManager _landingViewManager;
    private readonly HallOfFameManager _hallOfFameManager;
    private readonly HelpManager _helpManager;
    private readonly PacketManager _packetManager;
    private readonly ChatManager _chatManager;
    private readonly EffectManager _effectManager;
    private readonly BadgeManager _badgeManager;
    private readonly RoleplayManager _roleplayManager;
    private readonly AnimationManager _animationManager;
    private readonly LootManager _lootManager;
    private readonly BannerManager _bannerManager;

    private Thread _gameLoop;
    private readonly int _cycleSleepTime = 25;
    private bool _cycleEnded;
    private bool _gameLoopActive;

    private readonly Stopwatch _moduleWatch;

    public Game()
    {
        this._gameClientManager = new GameClientManager();
        this._permissionManager = new PermissionManager();
        this._itemDataManager = new ItemDataManager();
        this._catalogManager = new CatalogManager();
        this._navigatorManager = new NavigatorManager();
        this._roleplayManager = new RoleplayManager();
        this._roomManager = new RoomManager();
        this._groupManager = new GroupManager();
        this._moderationManager = new ModerationManager();
        this._questManager = new QuestManager();
        this._landingViewManager = new LandingViewManager();
        this._hallOfFameManager = new HallOfFameManager();
        this._helpManager = new HelpManager();
        this._chatManager = new ChatManager();
        this._effectManager = new EffectManager();
        this._badgeManager = new BadgeManager();
        this._achievementManager = new AchievementManager();
        this._animationManager = new AnimationManager();
        this._lootManager = new LootManager();
        this._bannerManager = new BannerManager();
        this._packetManager = new PacketManager();

        this._moduleWatch = new Stopwatch();
    }

    public void Init(IQueryAdapter dbClient)
    {
        DatabaseCleanup(dbClient);

        this._permissionManager.Init(dbClient);
        this._itemDataManager.Init(dbClient);
        this._catalogManager.Init(dbClient, this._itemDataManager);
        this._navigatorManager.Init(dbClient);
        this._roleplayManager.Init(dbClient);
        this._roomManager.Init(dbClient);
        this._groupManager.Init(dbClient);
        this._moderationManager.Init(dbClient);
        this._questManager.Init(dbClient);
        this._landingViewManager.Init(dbClient);
        this._hallOfFameManager.Init(dbClient);
        this._chatManager.Init(dbClient);
        this._effectManager.Init(dbClient);
        this._badgeManager.Init();
        this._achievementManager.Init(dbClient);
        this._animationManager.Init(dbClient);
        this._lootManager.Init(dbClient);
        this._bannerManager.Init(dbClient);
        this._packetManager.Init();

        ServerStatusUpdater.Init(dbClient);
    }

    public LootManager GetLootManager() => this._lootManager;

    public AnimationManager GetAnimationManager() => this._animationManager;

    public BadgeManager GetBadgeManager() => this._badgeManager;

    public EffectManager GetEffectManager() => this._effectManager;

    public ChatManager GetChatManager() => this._chatManager;

    public PacketManager GetPacketManager() => this._packetManager;

    public HelpManager GetHelpManager() => this._helpManager;

    public RoleplayManager GetRoleplayManager() => this._roleplayManager;

    public GameClientManager GetGameClientManager() => this._gameClientManager;

    public PermissionManager GetPermissionManager() => this._permissionManager;

    public CatalogManager GetCatalog() => this._catalogManager;

    public NavigatorManager GetNavigator() => this._navigatorManager;

    public ItemDataManager GetItemManager() => this._itemDataManager;

    public RoomManager GetRoomManager() => this._roomManager;

    public AchievementManager GetAchievementManager() => this._achievementManager;

    public ModerationManager GetModerationManager() => this._moderationManager;

    public QuestManager GetQuestManager() => this._questManager;

    public GroupManager GetGroupManager() => this._groupManager;

    public BannerManager GetBannerManager() => this._bannerManager;

    public LandingViewManager GetHotelView() => this._landingViewManager;

    public HallOfFameManager GetHallOFFame() => this._hallOfFameManager;

    public void StartGameLoop()
    {
        this._gameLoopActive = true;

        var receiver = new ThreadStart(this.MainGameLoop);
        this._gameLoop = new Thread(receiver)
        {
            IsBackground = true
        };

        this._gameLoop.Start();
    }

    private void MainGameLoop()
    {
        while (this._gameLoopActive)
        {
            this._cycleEnded = false;

            try
            {
                this._moduleWatch.Restart();

                ServerStatusUpdater.Process();

                if (this._moduleWatch.ElapsedMilliseconds > 500)
                {
                    Console.WriteLine("High latency in LowPriorityWorker.Process ({0} ms)", this._moduleWatch.ElapsedMilliseconds);
                }

                this._moduleWatch.Restart();

                this._roomManager.OnCycle(this._moduleWatch);

                if (this._moduleWatch.ElapsedMilliseconds > 500)
                {
                    Console.WriteLine("High latency in RoomManager ({0} ms)", this._moduleWatch.ElapsedMilliseconds);
                }

                this._moduleWatch.Restart();

                this._gameClientManager.OnCycle();

                if (this._moduleWatch.ElapsedMilliseconds > 500)
                {
                    Console.WriteLine("High latency in GameClientManager ({0} ms)", this._moduleWatch.ElapsedMilliseconds);
                }

                this._moduleWatch.Restart();

                this._hallOfFameManager.OnCycle();

                if (this._moduleWatch.ElapsedMilliseconds > 500)
                {
                    Console.WriteLine("High latency in HallOfFame ({0} ms)", this._moduleWatch.ElapsedMilliseconds);
                }

                this._moduleWatch.Restart();

                this._animationManager.OnCycle(this._moduleWatch);

                if (this._moduleWatch.ElapsedMilliseconds > 500)
                {
                    Console.WriteLine("High latency in AnimationManager ({0} ms)", this._moduleWatch.ElapsedMilliseconds);
                }

                this._moduleWatch.Restart();
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine("Canceled operation {0}", e);
            }
            catch (Exception e)
            {
                ExceptionLogger.LogThreadException(e.ToString(), "MainThread");
            }

            this._cycleEnded = true;

            Thread.Sleep(this._cycleSleepTime);
        }

        Console.WriteLine("MainGameLoop end");
    }

    public static void DatabaseCleanup(IQueryAdapter dbClient)
    {
        if (!Debugger.IsAttached)
        {
            UserDao.UpdateAllOnline(dbClient);
            UserDao.UpdateAllTicket(dbClient);
            RoomDao.UpdateResetUsersNow(dbClient);
            EmulatorStatusDao.UpdateReset(dbClient);
        }
    }

    public void StopGameLoop()
    {
        this._gameLoopActive = false;
        while (!this._cycleEnded)
        {
            Thread.Sleep(this._cycleSleepTime);
        }
    }

    public void Dispose() => GC.SuppressFinalize(this);
}
