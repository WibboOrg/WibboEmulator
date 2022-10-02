using WibboEmulator.Communication.Packets.Outgoing.Handshake;
using WibboEmulator.Communication.Packets.Outgoing.Help;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Session;
using WibboEmulator.Core;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Chat.Logs;
using WibboEmulator.Games.Roleplay;
using WibboEmulator.Games.Roleplay.Player;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.GameClients.Achievements;
using WibboEmulator.Games.GameClients.Badges;
using WibboEmulator.Games.GameClients.Inventory;
using WibboEmulator.Games.GameClients.Messenger;
using WibboEmulator.Games.GameClients.Permissions;
using WibboEmulator.Games.GameClients.Wardrobes;
using System.Data;

namespace WibboEmulator.Games.GameClients
{
    public class User
    {
        private GameClient _clientInstance;

        public int Id;
        public string Username;
        public int Rank;
        public string Motto;
        public string Look;
        public string BackupLook;
        public string Gender;
        public string BackupGender;
        public bool LastMovFGate;
        public int Credits;
        public int WibboPoints;
        public int LimitCoins;
        public int AccountCreated;
        public int AchievementPoints;
        public int Duckets;
        public int Respect;
        public int DailyRespectPoints;
        public int DailyPetRespectPoints;
        public int CurrentRoomId;
        public int LoadingRoomId;
        public int HomeRoom;
        public int LastOnline;
        public bool IsTeleporting;
        public int TeleportingRoomID;
        public int TeleporterId;
        public List<int> ClientVolume;
        public string MachineId;
        public Language Langue;

        public bool ForceOpenGift;
        public int ForceUse = -1;
        public int ForceRot = -1;

        public List<int> RoomRightsList;
        public List<int> FavoriteRooms;
        public List<int> UsersRooms;
        public List<int> MutedUsers;
        public List<int> RatedRooms;
        public List<int> MyGroups;
        public Dictionary<int, int> Quests;
        public Dictionary<double, int> Visits;

        private MessengerComponent _messengerComponent;
        private BadgeComponent _badgeComponent;
        private AchievementComponent _achievementComponent;
        private InventoryComponent _inventoryComponent;
        private WardrobeComponent _wardrobeComponent;
        private ChatlogManager _chatMessageManager;

        public bool SpectatorMode;
        public bool Disconnected;
        public bool HasFriendRequestsDisabled;
        public int FavouriteGroupId;

        public int FloodCount;
        public DateTime FloodTime;
        public bool SpamEnable;
        public int SpamProtectionTime;
        public DateTime SpamFloodTime;
        public DateTime EveryoneTimer;
        public DateTime LastGiftPurchaseTime;

        public int CurrentQuestId;
        public int LastCompleted;
        public int LastQuestId;
        public bool InfoSaved;
        public bool AcceptTrading;
        public bool HideInRoom;
        public int PubDectectCount = 0;
        public DateTime OnlineTime;
        public bool PremiumProtect;
        public int ControlUserId;
        public string IP;
        public bool ViewMurmur = true;
        public bool HideOnline;
        public string LastPhotoId;

        public int GuideOtherUserId;
        public bool OnDuty;

        public int Mazo;
        public int MazoHighScore;

        public bool NewUser;
        public bool Nuxenable;
        public int PassedNuxCount;

        public bool AllowDoorBell;
        public bool CanChangeName;
        public int GiftPurchasingWarnings;
        public bool SessionGiftBlocked;

        public int RolePlayId;
        public double IgnoreAllExpireTime;

        public bool IgnoreRoomInvites;
        public bool CameraFollowDisabled;
        public bool OldChat;

        private PermissionComponent _permissions;
        public bool IgnoreAll
        {
            get
            {
                return this.IgnoreAllExpireTime > WibboEnvironment.GetUnixTimestamp();
            }
        }

        public bool InRoom => this.CurrentRoomId > 0;

        public Room CurrentRoom
        {
            get
            {
                if (this.CurrentRoomId <= 0)
                {
                    return null;
                }
                else
                {
                    if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(this.CurrentRoomId, out Room room))
                        return null;

                    return room;
                }
            }
        }

        public User(int Id, string Username, int Rank, string Motto, string Look, string Gender, int Credits,
            int WPoint, int LimitCoins, int ActivityPoints, int HomeRoom, int Respect, int DailyRespectPoints,
            int DailyPetRespectPoints, bool HasFriendRequestsDisabled, int currentQuestID, int achievementPoints,
            int LastOnline, int FavoriteGroup, int accountCreated, bool accepttrading, string ip, bool HideInroom,
            bool HideOnline, int MazoHighScore, int Mazo, string clientVolume, bool nuxenable, string MachineId,
            bool ChangeName, Language Langue, double ignoreAllExpire, bool IgnoreRoomInvite, bool CameraFollowDisabled)
        {
            this.Id = Id;
            this.Username = Username;
            this.Rank = Rank;
            this.Motto = Motto;
            this.MachineId = MachineId;
            this.Look = WibboEnvironment.GetFigureManager().ProcessFigure(Look, Gender, true);
            this.Gender = Gender.ToLower();
            this.Credits = Credits;
            this.WibboPoints = WPoint;
            this.LimitCoins = LimitCoins;
            this.Duckets = ActivityPoints;
            this.AchievementPoints = achievementPoints;
            this.CurrentRoomId = 0;
            this.LoadingRoomId = 0;
            this.HomeRoom = HomeRoom;
            this.FavoriteRooms = new List<int>();
            this.RoomRightsList = new List<int>();
            this.UsersRooms = new List<int>();
            this.MutedUsers = new List<int>();
            this.RatedRooms = new List<int>();
            this.Respect = Respect;
            this.DailyRespectPoints = DailyRespectPoints;
            this.DailyPetRespectPoints = DailyPetRespectPoints;
            this.IsTeleporting = false;
            this.TeleporterId = 0;
            this.HasFriendRequestsDisabled = HasFriendRequestsDisabled;
            this.ClientVolume = new List<int>(3);
            this.CanChangeName = ChangeName;
            this.Langue = Langue;
            this.IgnoreAllExpireTime = ignoreAllExpire;

            if (clientVolume.Contains(','))
            {
                foreach (string Str in clientVolume.Split(','))
                {
                    if (int.TryParse(Str, out int Val))
                    {
                        this.ClientVolume.Add(int.Parse(Str));
                    }
                    else
                    {
                        this.ClientVolume.Add(100);
                    }
                }
            }
            else
            {
                this.ClientVolume.Add(100);
                this.ClientVolume.Add(100);
                this.ClientVolume.Add(100);
            }

            this.LastOnline = LastOnline;
            this.MyGroups = new List<int>();
            this.Quests = new Dictionary<int, int>();
            this.FavouriteGroupId = FavoriteGroup;

            this.AccountCreated = accountCreated;

            this.CurrentQuestId = currentQuestID;
            this.AcceptTrading = accepttrading;

            this.OnlineTime = DateTime.Now;
            this.PremiumProtect = (this.Rank > 1);

            this.ControlUserId = 0;
            this.IP = ip;
            this.SpectatorMode = false;
            this.Disconnected = false;
            this.HideInRoom = HideInroom;
            this.HideOnline = HideOnline;
            this.MazoHighScore = MazoHighScore;
            this.Mazo = Mazo;

            this.LastGiftPurchaseTime = DateTime.Now;

            this.Nuxenable = nuxenable;
            this.NewUser = nuxenable;
            this.Visits = new Dictionary<double, int>();

            this.IgnoreRoomInvites = IgnoreRoomInvite;
            this.CameraFollowDisabled = CameraFollowDisabled;
            this.OldChat = false;
        }

        public void Init(GameClient client)
        {
            this._clientInstance = client;

            this._badgeComponent = new BadgeComponent(this);
            this._achievementComponent = new AchievementComponent(this);
            this._inventoryComponent = new InventoryComponent(this);
            this._wardrobeComponent = new WardrobeComponent(this);
            this._messengerComponent = new MessengerComponent(this);
            this._chatMessageManager = new ChatlogManager();
            this._permissions = new PermissionComponent(this);

            using IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
            this._badgeComponent.Init(dbClient);
            this._wardrobeComponent.Init(dbClient);
            this._achievementComponent.Init(dbClient);
            this._messengerComponent.Init(dbClient, this.HideOnline);
            this._chatMessageManager.LoadUserChatlogs(dbClient, this.Id);

            DataTable dUserRooms = RoomDao.GetAllByOwner(dbClient, this.Username);
            foreach (DataRow dRow in dUserRooms.Rows)
            {
                this.UsersRooms.Add(Convert.ToInt32(dRow["id"]));
            }

            DataTable dGroupMemberships = GuildMembershipDao.GetOneByUserId(dbClient, this.Id);
            foreach (DataRow dRow in dGroupMemberships.Rows)
            {
                this.MyGroups.Add(Convert.ToInt32(dRow["group_id"]));
            }

            DataTable dQuests = UserQuestDao.GetAll(dbClient, this.Id);
            foreach (DataRow dataRow in dQuests.Rows)
            {
                int questId = Convert.ToInt32(dataRow["quest_id"]);
                int progress = Convert.ToInt32(dataRow["progress"]);
                this.Quests.Add(questId, progress);
            }

            DataTable dFavorites = UserFavoriteDao.GetAll(dbClient, this.Id);
            foreach (DataRow dataRow in dFavorites.Rows)
            {
                int roomId = Convert.ToInt32(dataRow["room_id"]);
                this.FavoriteRooms.Add(roomId);
            }

            DataTable dRoomRights = RoomRightDao.GetAllByUserId(dbClient, this.Id);
            foreach (DataRow dataRow in dRoomRights.Rows)
            {
                int roomId = Convert.ToInt32(dataRow["room_id"]);
                this.RoomRightsList.Add(roomId);
            }
        }

        public void PrepareRoom(int Id, string Password = "", bool override_doorbell = false)
        {
            if (this.GetClient() == null || this.GetClient().GetUser() == null)
            {
                return;
            }

            if (this.GetClient().GetUser().InRoom)
            {
                if (WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(this.GetClient().GetUser().CurrentRoomId, out Room oldRoom))
                    oldRoom.GetRoomUserManager().RemoveUserFromRoom(this.GetClient(), false, false);
            }

            if (this.GetClient().GetUser().IsTeleporting && this.GetClient().GetUser().TeleportingRoomID != Id)
            {
                this.GetClient().GetUser().TeleportingRoomID = 0;
                this.GetClient().GetUser().IsTeleporting = false;
                this.GetClient().GetUser().TeleporterId = 0;
                this.GetClient().SendPacket(new CloseConnectionComposer());

                return;
            }

            Room room = WibboEnvironment.GetGame().GetRoomManager().LoadRoom(Id);
            if (room == null)
            {
                this.GetClient().SendPacket(new CloseConnectionComposer());
                return;
            }

            if (!this.GetClient().GetUser().HasPermission("perm_mod") && room.UserIsBanned(this.GetClient().GetUser().Id))
            {
                if (room.HasBanExpired(this.GetClient().GetUser().Id))
                {
                    room.RemoveBan(this.GetClient().GetUser().Id);
                }
                else
                {
                    this.GetClient().SendPacket(new CantConnectComposer(1));

                    this.GetClient().SendPacket(new CloseConnectionComposer());
                    return;
                }
            }

            if (room.RoomData.UsersNow >= room.RoomData.UsersMax && !this.GetClient().GetUser().HasPermission("perm_enter_full_rooms") && !this.GetClient().GetUser().HasPermission("perm_enter_full_rooms"))
            {
                if (room.CloseFullRoom)
                {
                    room.RoomData.State = 1;
                    room.CloseFullRoom = false;
                }

                if (this.GetClient().GetUser().Id != room.RoomData.OwnerId)
                {
                    this.GetClient().SendPacket(new CantConnectComposer(1));

                    this.GetClient().SendPacket(new CloseConnectionComposer());
                    return;
                }
            }

            string[] OwnerEnterNotAllowed = WibboEnvironment.GetSettings().GetData<string>("room.owner.enter.not.allowed").Split(',');

            if (!this.GetClient().GetUser().HasPermission("perm_access_apartments_all"))
            {
                if (!(this.GetClient().GetUser().HasPermission("perm_access_apartments") && !OwnerEnterNotAllowed.Contains(room.RoomData.OwnerName)) && !room.CheckRights(this.GetClient(), true) && !(this.GetClient().GetUser().IsTeleporting && this.GetClient().GetUser().TeleportingRoomID == room.Id))
                {
                    if (room.RoomData.State == 1 && (!override_doorbell && !room.CheckRights(this.GetClient())))
                    {
                        if (room.UserCount == 0)
                        {
                            this.GetClient().SendPacket(new FlatAccessDeniedComposer(""));
                        }
                        else
                        {
                            this.GetClient().SendPacket(new DoorbellComposer(""));
                            room.SendPacket(new DoorbellComposer(this.GetClient().GetUser().Username), true);
                            this.GetClient().GetUser().LoadingRoomId = Id;
                            this.GetClient().GetUser().AllowDoorBell = false;
                        }
                        return;
                    }
                    else if (room.RoomData.State == 2 && Password.ToLower() != room.RoomData.Password.ToLower())
                    {
                        this.GetClient().SendPacket(new GenericErrorComposer(-100002));
                        this.GetClient().SendPacket(new CloseConnectionComposer());
                        return;
                    }
                }
            }

            if (room.RoomData.OwnerName == WibboEnvironment.GetSettings().GetData<string>("game.owner"))
            {
                if (room.GetRoomUserManager().GetUserByTracker(this.IP, this.GetClient().MachineId) != null)
                {
                    this.GetClient().SendPacket(new CloseConnectionComposer());
                    return;
                }
            }

            if (!this.EnterRoom(room))
            {
                this.GetClient().SendPacket(new CloseConnectionComposer());
            }
            else
            {
                this.GetClient().GetUser().LoadingRoomId = Id;
                this.GetClient().GetUser().AllowDoorBell = true;
            }

        }

        public bool EnterRoom(Room Room)
        {
            GameClient Session = this.GetClient();
            if (Session == null)
            {
                return false;
            }

            if (Room == null)
            {
                return false;
            }

            Session.SendPacket(new RoomReadyComposer(Room.Id, Room.RoomData.ModelName));

            if (Room.RoomData.Wallpaper != "0.0")
            {
                Session.SendPacket(new RoomPropertyComposer("wallpaper", Room.RoomData.Wallpaper));
            }

            if (Room.RoomData.Floor != "0.0")
            {
                Session.SendPacket(new RoomPropertyComposer("floor", Room.RoomData.Floor));
            }

            Session.SendPacket(new RoomPropertyComposer("landscape", Room.RoomData.Landscape));
            Session.SendPacket(new RoomRatingComposer(Room.RoomData.Score, !(Session.GetUser().RatedRooms.Contains(Room.Id) || Room.RoomData.OwnerId == Session.GetUser().Id)));

            Session.SendPacket(Room.GetGameMap().Model.SerializeRelativeHeightmap());
            Session.SendPacket(Room.GetGameMap().Model.GetHeightmap());

            return true;
        }

        public bool HasExactPermission(string Fuse)
        {
            if (WibboEnvironment.GetGame().GetPermissionManager().RankExactRight(this.Rank, Fuse))
            {
                return true;
            }

            return false;
        }

        public bool HasPermission(string Fuse)
        {
            if (WibboEnvironment.GetGame().GetPermissionManager().RankHasRight(this.Rank, Fuse))
            {
                return true;
            }

            return false;
        }

        public void OnDisconnect()
        {
            if (this.Disconnected)
            {
                return;
            }

            this.Disconnected = true;

            WibboEnvironment.GetGame().GetClientManager().UnregisterClient(this.Id, this.Username);

            if (this.HasPermission("perm_mod"))
            {
                WibboEnvironment.GetGame().GetClientManager().RemoveUserStaff(this.Id);
            }

            ExceptionLogger.WriteLine(this.Username + " has logged out.");

            if (!this.InfoSaved)
            {
                this.InfoSaved = true;
                TimeSpan TimeOnline = DateTime.Now - this.OnlineTime;
                int TimeOnlineSec = (int)TimeOnline.TotalSeconds;
                using IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
                UserDao.UpdateOffline(dbClient, this.Id, this.Duckets, this.Credits);
                UserStatsDao.UpdateAll(dbClient, this.Id, this.FavouriteGroupId, TimeOnlineSec, this.CurrentQuestId, this.Respect, this.DailyRespectPoints, this.DailyPetRespectPoints);
            }

            if (this.InRoom && this.CurrentRoom != null)
            {
                this.CurrentRoom.GetRoomUserManager().RemoveUserFromRoom(this._clientInstance, false, false);
            }

            if (this.RolePlayId > 0)
            {
                RolePlayerManager RPManager = WibboEnvironment.GetGame().GetRoleplayManager().GetRolePlay(this.RolePlayId);
                if (RPManager != null)
                {
                    RolePlayer Rp = RPManager.GetPlayer(this.Id);
                    if (Rp != null)
                    {
                        RPManager.RemovePlayer(this.Id);
                    }
                }
                this.RolePlayId = 0;
            }

            if (this.GuideOtherUserId != 0)
            {
                GameClient requester = WibboEnvironment.GetGame().GetClientManager().GetClientByUserID(this.GuideOtherUserId);
                if (requester != null)
                {
                    requester.SendPacket(new OnGuideSessionEndedComposer(1));

                    requester.GetUser().GuideOtherUserId = 0;
                }
            }
            if (this.OnDuty)
            {
                WibboEnvironment.GetGame().GetHelpManager().RemoveGuide(this.Id);
            }

            if (this._messengerComponent != null)
            {
                this._messengerComponent.AppearOffline = true;
                this._messengerComponent.Dispose();
            }

            if (this._inventoryComponent != null)
            {
                this._inventoryComponent.Dispose();
                this._inventoryComponent = null;
            }

            if (this._badgeComponent != null)
            {
                this._badgeComponent.Dispose();
                this._badgeComponent = null;
            }

            if (this._wardrobeComponent != null)
            {
                this._wardrobeComponent.Dispose();
                this._wardrobeComponent = null;
            }

            if (this._achievementComponent != null)
            {
                this._achievementComponent.Dispose();
                this._achievementComponent = null;
            }

            if (this.UsersRooms != null)
            {
                this.UsersRooms.Clear();
            }

            if (this.RoomRightsList != null)
            {
                this.RoomRightsList.Clear();
            }

            if (this.FavoriteRooms != null)
            {
                this.FavoriteRooms.Clear();
            }

            this._clientInstance = null;
        }

        public GameClient GetClient()
        {
            return this._clientInstance;
        }
        
        public MessengerComponent GetMessenger()
        {
            return this._messengerComponent;
        }

        public WardrobeComponent GetWardrobeComponent()
        {
            return this._wardrobeComponent;
        }

        public AchievementComponent GetAchievementComponent()
        {
            return this._achievementComponent;
        }

        public BadgeComponent GetBadgeComponent()
        {
            return this._badgeComponent;
        }

        public InventoryComponent GetInventoryComponent() => this._inventoryComponent;

        public ChatlogManager GetChatMessageManager()
        {
            return this._chatMessageManager;
        }

        public PermissionComponent GetPermissions()
        {
            return this._permissions;
        }

        public int GetQuestProgress(int p)
        {
            this.Quests.TryGetValue(p, out int num);
            return num;
        }
    }
}
