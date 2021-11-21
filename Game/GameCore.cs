using Butterfly.Communication.Packets;
using Butterfly.Core;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Achievement;
using Butterfly.Game.Animation;
using Butterfly.Game.Catalog;
using Butterfly.Game.GameClients;
using Butterfly.Game.Guilds;
using Butterfly.Game.Help;
using Butterfly.Game.Items;
using Butterfly.Game.LandingView;
using Butterfly.Game.Navigator;
using Butterfly.Game.Quests;
using Butterfly.Game.Roleplay;
using Butterfly.Game.Role;
using Butterfly.Game.Rooms;
using Butterfly.Game.Rooms.Chat;
using Butterfly.Game.Moderation;
using Butterfly.Game.User.Effect;
using Butterfly.Game.WebClients;
using System;
using System.Diagnostics;
using System.Threading;

namespace Butterfly.Game
{
    public class GameCore
    {
        private readonly GameClientManager _clientManager;
        private readonly WebClientManager _clientWebManager;
        private readonly RoleManager _roleManager;
        private readonly CatalogManager _catalogManager;
        private readonly NavigatorManager _navigatorManager;
        private readonly ItemDataManager _itemDataManager;
        private readonly RoomManager _roomManager;
        private readonly AchievementManager _achievementManager;
        private readonly ModerationManager _moderationManager;
        private readonly QuestManager _questManager;
        private readonly GuildManager _groupManager;
        private readonly LandingViewManager _landingViewManager;
        private readonly HelpManager _helpManager;
        private readonly PacketManager _packetManager;
        private readonly ChatManager _chatManager;
        private readonly EffectManager _effectManager;
        private readonly RoleplayManager _roleplayManager;
        private readonly AnimationManager _animationManager;

        private Thread _gameLoop;
        public static bool GameLoopEnabled = true;
        public bool GameLoopActive;
        public bool GameLoopEnded;

        private readonly Stopwatch _moduleWatch;

        public GameCore()
        {
            this._clientManager = new GameClientManager();
            this._clientWebManager = new WebClientManager();

            this._roleManager = new RoleManager();
            this._roleManager.Init();

            this._itemDataManager = new ItemDataManager();
            this._itemDataManager.Init();

            this._catalogManager = new CatalogManager();
            this._catalogManager.Init(this._itemDataManager);

            this._navigatorManager = new NavigatorManager();
            this._navigatorManager.Init();

            this._roleplayManager = new RoleplayManager();
            this._roleplayManager.Init();

            this._roomManager = new RoomManager();
            this._roomManager.Init();

            this._groupManager = new GuildManager();
            this._groupManager.Init();

            this._moderationManager = new ModerationManager();
            this._moderationManager.Init();

            this._questManager = new QuestManager();
            this._questManager.Init();

            this._landingViewManager = new LandingViewManager();
            this._landingViewManager.Init();

            this._helpManager = new HelpManager();

            this._packetManager = new PacketManager();
            this._packetManager.Init();

            this._chatManager = new ChatManager();
            this._chatManager.Init();

            this._effectManager = new EffectManager();
            this._effectManager.Init();

            this._achievementManager = new AchievementManager();
            this._achievementManager.Init();

            this._animationManager = new AnimationManager();
            this._animationManager.Init();

            DatabaseCleanup();
            LowPriorityWorker.Init();

            this._moduleWatch = new Stopwatch();
        }

        #region Return values

        public AnimationManager GetAnimationManager()
        {
            return this._animationManager;
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

        public GameClientManager GetClientManager()
        {
            return this._clientManager;
        }

        public WebClientManager GetClientWebManager()
        {
            return this._clientWebManager;
        }

        public RoleManager GetRoleManager()
        {
            return this._roleManager;
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

        public GuildManager GetGroupManager()
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
                    if (GameLoopEnabled)
                    {
                        this._moduleWatch.Restart();

                        LowPriorityWorker.Process();

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
                }
                catch (OperationCanceledException e)
                {
                    Console.WriteLine("Canceled operation {0}", e);
                }

                Thread.Sleep(500);
            }

            Console.WriteLine("MainGameLoop end");
            this.GameLoopEnded = true;
        }

        public static void DatabaseCleanup()
        {
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                UserDao.UpdateAllOnline(dbClient);
                UserDao.UpdateAllTicket(dbClient);
                UserWebsocketDao.UpdateReset(dbClient);
                RoomDao.UpdateResetUsersNow(dbClient);
                EmulatorStatusDao.UpdateReset(dbClient);
            }
        }

        public void Destroy()
        {
            DatabaseCleanup();
            Console.WriteLine("Destroyed Habbo Hotel.");
        }
    }
}