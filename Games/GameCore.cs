using WibboEmulator.Communication.Packets;
using WibboEmulator.Core;
using WibboEmulator.Games.Achievements;
using WibboEmulator.Games.Animations;
using WibboEmulator.Games.Catalog;
using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Groups;
using WibboEmulator.Games.Help;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.LandingView;
using WibboEmulator.Games.Navigator;
using WibboEmulator.Games.Quests;
using WibboEmulator.Games.Roleplay;
using WibboEmulator.Games.Permissions;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Chat;
using WibboEmulator.Games.Moderation;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Effects;
using WibboEmulator.Games.Badges;
using WibboEmulator.Database.Daos;
using System.Diagnostics;
using WibboEmulator.Games.Loots;

namespace WibboEmulator.Games;

public class GameCore
{
    private readonly ClientManager _clientManager;
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
    private readonly HelpManager _helpManager;
    private readonly PacketManager _packetManager;
    private readonly ChatManager _chatManager;
    private readonly EffectManager _effectManager;
    private readonly BadgeManager _badgeManager;
    private readonly RoleplayManager _roleplayManager;
    private readonly AnimationManager _animationManager;
    private readonly LootManager _lootManager;

    private Thread _gameLoop;
    public bool GameLoopActive;

    private readonly Stopwatch _moduleWatch;

    public GameCore()
    {
        using IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();

        this._clientManager = new ClientManager();

        this._permissionManager = new PermissionManager();
        this._permissionManager.Init(dbClient);

        this._itemDataManager = new ItemDataManager();
        this._itemDataManager.Init(dbClient);

        this._catalogManager = new CatalogManager();
        this._catalogManager.Init(dbClient, this._itemDataManager);

        this._navigatorManager = new NavigatorManager();
        this._navigatorManager.Init(dbClient);

        this._roleplayManager = new RoleplayManager();
        this._roleplayManager.Init(dbClient);

        this._roomManager = new RoomManager();
        this._roomManager.Init(dbClient);

        this._groupManager = new GroupManager();
        this._groupManager.Init(dbClient);

        this._moderationManager = new ModerationManager();
        this._moderationManager.Init(dbClient);

        this._questManager = new QuestManager();
        this._questManager.Init(dbClient);

        this._landingViewManager = new LandingViewManager();
        this._landingViewManager.Init(dbClient);

        this._helpManager = new HelpManager();

        this._packetManager = new PacketManager();
        this._packetManager.Init(dbClient);

        this._chatManager = new ChatManager();
        this._chatManager.Init(dbClient);

        this._effectManager = new EffectManager();
        this._effectManager.Init(dbClient);

        this._badgeManager = new BadgeManager();
        this._badgeManager.Init(dbClient);

        this._achievementManager = new AchievementManager();
        this._achievementManager.Init(dbClient);

        this._animationManager = new AnimationManager();
        this._animationManager.Init(dbClient);

        this._lootManager = new LootManager();
        this._lootManager.Init(dbClient);

        DatabaseCleanup(dbClient);
        ServerStatusUpdater.Init(dbClient);

        this._moduleWatch = new Stopwatch();
    }

    #region Return values

    public LootManager GetLootManager()
    {
        return this._lootManager;
    }

    public AnimationManager GetAnimationManager()
    {
        return this._animationManager;
    }

    public BadgeManager GetBadgeManager()
    {
        return this._badgeManager;
    }

    public EffectManager GetEffectManager()
    {
        return this._effectManager;
    }

    public ChatManager GetChatManager()
    {
        return this._chatManager;
    }

    public PacketManager GetPacketManager()
    {
        return this._packetManager;
    }

    public HelpManager GetHelpManager()
    {
        return this._helpManager;
    }

    public RoleplayManager GetRoleplayManager()
    {
        return this._roleplayManager;
    }

    public ClientManager GetClientManager()
    {
        return this._clientManager;
    }

    public PermissionManager GetPermissionManager()
    {
        return this._permissionManager;
    }

    public CatalogManager GetCatalog()
    {
        return this._catalogManager;
    }

    public NavigatorManager GetNavigator()
    {
        return this._navigatorManager;
    }

    public ItemDataManager GetItemManager()
    {
        return this._itemDataManager;
    }

    public RoomManager GetRoomManager()
    {
        return this._roomManager;
    }

    public AchievementManager GetAchievementManager()
    {
        return this._achievementManager;
    }

    public ModerationManager GetModerationManager()
    {
        return this._moderationManager;
    }

    public QuestManager GetQuestManager()
    {
        return this._questManager;
    }

    public GroupManager GetGroupManager()
    {
        return this._groupManager;
    }

    public LandingViewManager GetHotelView()
    {
        return this._landingViewManager;
    }
    #endregion

    public void StartGameLoop()
    {
        this.GameLoopActive = true;

        this._gameLoop = new Thread(new ThreadStart(this.MainGameLoop))
        {
            Priority = ThreadPriority.Highest
        };

        this._gameLoop.Start();
    }

    private void MainGameLoop()
    {
        while (this.GameLoopActive)
        {
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

            Thread.Sleep(500);
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

    public void Destroy()
    {
        this.GameLoopActive = false;
        Console.WriteLine("Destroyed Hotel.");
    }
}