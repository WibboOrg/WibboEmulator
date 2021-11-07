using Butterfly.Communication.Packets;
using Butterfly.Core;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.Achievements;
using Butterfly.HabboHotel.Animations;
using Butterfly.HabboHotel.Catalog;
using Butterfly.HabboHotel.Effects;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Groups;
using Butterfly.HabboHotel.Help;
using Butterfly.HabboHotel.Items;
using Butterfly.HabboHotel.LandingView;
using Butterfly.HabboHotel.Navigators;
using Butterfly.HabboHotel.Quests;
using Butterfly.HabboHotel.Roleplay;
using Butterfly.HabboHotel.Roles;
using Butterfly.HabboHotel.Rooms;
using Butterfly.HabboHotel.Rooms.Chat;
using Butterfly.HabboHotel.Support;
using Butterfly.HabboHotel.WebClients;
using System;
using System.Diagnostics;
using System.Threading;

namespace Butterfly.HabboHotel
{
    public class Game
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
        private readonly GroupManager _groupManager;
        private readonly LandingViewManager _landingViewManager;
        private readonly HelpManager _helpManager;
        private readonly PacketManager _packetManager;
        private readonly ChatManager _chatManager;
        private readonly EffectManager _effectManager;
        private readonly RoleplayManager _roleplayManager;
        private readonly AnimationManager _animationManager;

        private Thread gameLoop;
        public static bool gameLoopEnabled = true;
        public bool gameLoopActive;
        public bool gameLoopEnded;

        private readonly Stopwatch moduleWatch;

        public Game()
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
            this._roomManager.LoadModels();

            this._groupManager = new GroupManager();
            this._groupManager.Init();

            this._moderationManager = new ModerationManager();
            this._moderationManager.Init();

            this._questManager = new QuestManager();
            this._questManager.Init();

            this._landingViewManager = new LandingViewManager();
            this._helpManager = new HelpManager();
            this._packetManager = new PacketManager();
            this._chatManager = new ChatManager();

            this._effectManager = new EffectManager();
            this._effectManager.Init();

            this._achievementManager = new AchievementManager();

            this._animationManager = new AnimationManager();
            this._animationManager.Init();

            DatabaseCleanup();
            LowPriorityWorker.Init();

            this.moduleWatch = new Stopwatch();
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
            this.gameLoopActive = true;

            this.gameLoop = new Thread(new ThreadStart(this.MainGameLoop))
            {
                Priority = ThreadPriority.Highest
            };

            this.gameLoop.Start();
        }

        private void MainGameLoop()
        {
            while (this.gameLoopActive)
            {
                try
                {
                    if (gameLoopEnabled)
                    {
                        this.moduleWatch.Restart();

                        LowPriorityWorker.Process();

                        if (this.moduleWatch.ElapsedMilliseconds > 500)
                        {
                            Console.WriteLine("High latency in LowPriorityWorker.Process ({0} ms)", this.moduleWatch.ElapsedMilliseconds);
                        }

                        this.moduleWatch.Restart();

                        this._roomManager.OnCycle(this.moduleWatch);

                        if (this.moduleWatch.ElapsedMilliseconds > 500)
                        {
                            Console.WriteLine("High latency in RoomManager ({0} ms)", this.moduleWatch.ElapsedMilliseconds);
                        }

                        this.moduleWatch.Restart();

                        this._animationManager.OnCycle(this.moduleWatch);

                        if (this.moduleWatch.ElapsedMilliseconds > 500)
                        {
                            Console.WriteLine("High latency in AnimationManager ({0} ms)", this.moduleWatch.ElapsedMilliseconds);
                        }

                        this.moduleWatch.Restart();
                    }
                }
                catch (OperationCanceledException e)
                {
                    Console.WriteLine("Canceled operation {0}", e);
                }

                Thread.Sleep(500);
            }

            Console.WriteLine("MainGameLoop end");
            this.gameLoopEnded = true;
        }

        public static void DatabaseCleanup()
        {
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE users SET online = '0' WHERE online = '1'");
                dbClient.RunQuery("UPDATE users SET auth_ticket = '' WHERE auth_ticket != ''");
                UserWebsocketDao.UpdateReset(dbClient);
                RoomDao.UpdateResetUsersNow(dbClient);
                EmulatorStatusDao.UpdateReset(dbClient);
            }
        }

        public void Destroy()
        {
            DatabaseCleanup();
            this.GetClientManager();
            Console.WriteLine("Destroyed Habbo Hotel.");
        }
    }
}