namespace WibboEmulator.Games.Users;
using System.Data;
using System.Globalization;
using WibboEmulator.Communication.Packets.Outgoing.Handshake;
using WibboEmulator.Communication.Packets.Outgoing.Help;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Notifications;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Session;
using WibboEmulator.Core;
using WibboEmulator.Core.Language;
using WibboEmulator.Database.Daos.Guild;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.Chat.Logs;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Users.Achievements;
using WibboEmulator.Games.Users.Badges;
using WibboEmulator.Games.Users.Inventory;
using WibboEmulator.Games.Users.Messenger;
using WibboEmulator.Games.Users.Permissions;
using WibboEmulator.Games.Users.Wardrobes;

public class User : IDisposable
{
    private GameClient _clientInstance;
    private MessengerComponent _messengerComponent;
    private BadgeComponent _badgeComponent;
    private AchievementComponent _achievementComponent;
    private InventoryComponent _inventoryComponent;
    private WardrobeComponent _wardrobeComponent;
    private ChatlogManager _chatMessageManager;
    private PermissionComponent _permissions;

    public int Id { get; set; }
    public string Username { get; set; }
    public int Rank { get; set; }
    public string Motto { get; set; }
    public string Look { get; set; }
    public string BackupLook { get; set; }
    public string Gender { get; set; }
    public string BackupGender { get; set; }
    public bool LastMovFGate { get; set; }
    public int Credits { get; set; }
    public int WibboPoints { get; set; }
    public int LimitCoins { get; set; }
    public int AccountCreated { get; set; }
    public int AchievementPoints { get; set; }
    public int Duckets { get; set; }
    public int Respect { get; set; }
    public int DailyRespectPoints { get; set; }
    public int DailyPetRespectPoints { get; set; }
    public int CurrentRoomId { get; set; }
    public int LoadingRoomId { get; set; }
    public int HomeRoom { get; set; }
    public int LastOnline { get; set; }
    public bool IsTeleporting { get; set; }
    public int TeleportingRoomID { get; set; }
    public int TeleporterId { get; set; }
    public List<int> ClientVolume { get; set; }
    public string MachineId { get; set; }
    public Language Langue { get; set; }
    public bool ForceOpenGift { get; set; }
    public int ForceUse { get; set; } = -1;
    public int ForceRot { get; set; } = -1;
    public List<int> RoomRightsList { get; set; }
    public List<int> FavoriteRooms { get; set; }
    public List<int> UsersRooms { get; set; }
    public List<int> MutedUsers { get; set; }
    public List<int> RatedRooms { get; set; }
    public List<int> MyGroups { get; set; }
    public Dictionary<int, int> Quests { get; set; }
    public Dictionary<double, int> Visits { get; set; }
    public bool SpectatorMode { get; set; }
    public bool IsDisposed { get; set; }
    public bool HasFriendRequestsDisabled { get; set; }
    public int FavouriteGroupId { get; set; }
    public int FloodCount { get; set; }
    public DateTime FloodTime { get; set; }
    public bool SpamEnable { get; set; }
    public int SpamProtectionTime { get; set; }
    public DateTime SpamFloodTime { get; set; }
    public DateTime EveryoneTimer { get; set; }
    public DateTime LastGiftPurchaseTime { get; set; }
    public bool LoadRoomBlocked { get; set; }
    public int LoadRoomCount { get; set; }
    public DateTime LastLoadedRoomTime { get; set; }
    public int CurrentQuestId { get; set; }
    public int LastCompleted { get; set; }
    public int LastQuestId { get; set; }
    public bool InfoSaved { get; set; }
    public bool AcceptTrading { get; set; }
    public bool HideInRoom { get; set; }
    public int PubDetectCount { get; set; }
    public DateTime OnlineTime { get; set; }
    public bool PremiumProtect { get; set; }
    public int ControlUserId { get; set; }
    public string IP { get; set; }
    public bool ViewMurmur { get; set; } = true;
    public bool HideOnline { get; set; }
    public string LastPhotoId { get; set; }
    public int GuideOtherUserId { get; set; }
    public bool OnDuty { get; set; }
    public int Mazo { get; set; }
    public int MazoHighScore { get; set; }
    public bool NewUser { get; set; }
    public bool Nuxenable { get; set; }
    public int PassedNuxCount { get; set; }
    public bool AllowDoorBell { get; set; }
    public bool CanChangeName { get; set; }
    public int GiftPurchasingWarnings { get; set; }
    public bool SessionGiftBlocked { get; set; }
    public int RolePlayId { get; set; }
    public double IgnoreAllExpireTime { get; set; }
    public bool IgnoreRoomInvites { get; set; }
    public bool CameraFollowDisabled { get; set; }
    public bool OldChat { get; set; }
    public bool IgnoreAll => this.IgnoreAllExpireTime > WibboEnvironment.GetUnixTimestamp();
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
                if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(this.CurrentRoomId, out var room))
                {
                    return null;
                }

                return room;
            }
        }
    }

    public User(int id, string username, int rank, string motto, string look, string gender, int credits,
        int wpoint, int limitCoins, int activityPoints, int homeRoom, int respect, int dailyRespectPoints,
        int dailyPetRespectPoints, bool hasFriendRequestsDisabled, int currentQuestID, int achievementPoints,
        int lastOnline, int favoriteGroup, int accountCreated, bool accepttrading, string ip, bool hideInroom,
        bool hideOnline, int mazoHighScore, int mazo, string clientVolume, bool nuxenable, string machineId,
        bool changeName, Language langue, double ignoreAllExpire, bool ignoreRoomInvite, bool cameraFollowDisabled)
    {
        this.Id = id;
        this.Username = username;
        this.Rank = rank;
        this.Motto = motto;
        this.MachineId = machineId;
        this.Look = WibboEnvironment.GetFigureManager().ProcessFigure(look, gender, true);
        this.Gender = gender.ToLower(CultureInfo.CurrentCulture);
        this.Credits = credits;
        this.WibboPoints = wpoint;
        this.LimitCoins = limitCoins;
        this.Duckets = activityPoints;
        this.AchievementPoints = achievementPoints;
        this.CurrentRoomId = 0;
        this.LoadingRoomId = 0;
        this.HomeRoom = homeRoom;
        this.FavoriteRooms = new List<int>();
        this.RoomRightsList = new List<int>();
        this.UsersRooms = new List<int>();
        this.MutedUsers = new List<int>();
        this.RatedRooms = new List<int>();
        this.Respect = respect;
        this.DailyRespectPoints = dailyRespectPoints;
        this.DailyPetRespectPoints = dailyPetRespectPoints;
        this.IsTeleporting = false;
        this.TeleporterId = 0;
        this.HasFriendRequestsDisabled = hasFriendRequestsDisabled;
        this.ClientVolume = new List<int>(3);
        this.CanChangeName = changeName;
        this.Langue = langue;
        this.IgnoreAllExpireTime = ignoreAllExpire;

        if (clientVolume.Contains(','))
        {
            foreach (var str in clientVolume.Split(','))
            {
                if (int.TryParse(str, out var val))
                {
                    this.ClientVolume.Add(val);
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

        this.LastOnline = lastOnline;
        this.MyGroups = new List<int>();
        this.Quests = new Dictionary<int, int>();
        this.FavouriteGroupId = favoriteGroup;
        this.AccountCreated = accountCreated;
        this.CurrentQuestId = currentQuestID;
        this.AcceptTrading = accepttrading;
        this.OnlineTime = DateTime.Now;
        this.PremiumProtect = this.Rank > 1;
        this.ControlUserId = 0;
        this.IP = ip;
        this.SpectatorMode = false;
        this.IsDisposed = false;
        this.HideInRoom = hideInroom;
        this.HideOnline = hideOnline;
        this.MazoHighScore = mazoHighScore;
        this.Mazo = mazo;
        this.LastGiftPurchaseTime = DateTime.Now;
        this.Nuxenable = nuxenable;
        this.NewUser = nuxenable;
        this.Visits = new Dictionary<double, int>();
        this.IgnoreRoomInvites = ignoreRoomInvite;
        this.CameraFollowDisabled = cameraFollowDisabled;
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
        this._permissions = new PermissionComponent();

        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        this._badgeComponent.Init(dbClient);
        this._wardrobeComponent.Init(dbClient);
        this._achievementComponent.Init(dbClient);
        this._messengerComponent.Init(dbClient, this.HideOnline);
        this._chatMessageManager.LoadUserChatlogs(dbClient, this.Id);

        var dUserRooms = RoomDao.GetAllByOwner(dbClient, this.Username);
        foreach (DataRow dRow in dUserRooms.Rows)
        {
            this.UsersRooms.Add(Convert.ToInt32(dRow["id"]));
        }

        var dGroupMemberships = GuildMembershipDao.GetOneByUserId(dbClient, this.Id);
        foreach (DataRow dRow in dGroupMemberships.Rows)
        {
            this.MyGroups.Add(Convert.ToInt32(dRow["group_id"]));
        }

        var dQuests = UserQuestDao.GetAll(dbClient, this.Id);
        foreach (DataRow dataRow in dQuests.Rows)
        {
            var questId = Convert.ToInt32(dataRow["quest_id"]);
            var progress = Convert.ToInt32(dataRow["progress"]);
            this.Quests.Add(questId, progress);
        }

        var dFavorites = UserFavoriteDao.GetAll(dbClient, this.Id);
        foreach (DataRow dataRow in dFavorites.Rows)
        {
            var roomId = Convert.ToInt32(dataRow["room_id"]);
            this.FavoriteRooms.Add(roomId);
        }

        var dRoomRights = RoomRightDao.GetAllByUserId(dbClient, this.Id);
        foreach (DataRow dataRow in dRoomRights.Rows)
        {
            var roomId = Convert.ToInt32(dataRow["room_id"]);
            this.RoomRightsList.Add(roomId);
        }
    }

    public void PrepareRoom(int id, string password = "", bool overrideDoorbell = false)
    {
        if (this.GetClient() == null || this.GetClient().GetUser() == null)
        {
            return;
        }

        if (this.GetClient().GetUser().InRoom)
        {
            if (WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(this.GetClient().GetUser().CurrentRoomId, out var oldRoom))
            {
                oldRoom.RoomUserManager.RemoveUserFromRoom(this.GetClient(), false, false);
            }
        }

        if (this.GetClient().GetUser().IsTeleporting && this.GetClient().GetUser().TeleportingRoomID != id)
        {
            this.GetClient().GetUser().TeleportingRoomID = 0;
            this.GetClient().GetUser().IsTeleporting = false;
            this.GetClient().GetUser().TeleporterId = 0;
            this.GetClient().SendPacket(new CloseConnectionComposer());

            return;
        }

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(id, out _))
        {
            var timeSpan = DateTime.Now - this.LastLoadedRoomTime;
            if (timeSpan.TotalSeconds < 2)
            {
                this.LoadRoomCount++;
            }

            if (timeSpan.TotalSeconds > 60)
            {
                this.LoadRoomCount = 0;
                this.LoadRoomBlocked = false;
            }
            else if (this.LoadRoomCount > 5)
            {
                if (!this.LoadRoomBlocked)
                {
                    WibboEnvironment.GetGame().GetGameClientManager().SendMessageStaff(RoomNotificationComposer.SendBubble("mention", $"Attention {this.Username} charge trop vite les apparts!"));
                    this.LoadRoomBlocked = true;
                }
                this.GetClient().SendPacket(new CloseConnectionComposer());
                return;
            }

            this.LastLoadedRoomTime = DateTime.Now;
        }

        var room = WibboEnvironment.GetGame().GetRoomManager().LoadRoom(id);
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
                room.RoomData.Access = RoomAccess.Doorbell;
                room.CloseFullRoom = false;
            }

            if (this.GetClient().GetUser().Id != room.RoomData.OwnerId)
            {
                this.GetClient().SendPacket(new CantConnectComposer(1));

                this.GetClient().SendPacket(new CloseConnectionComposer());
                return;
            }
        }

        var ownerEnterNotAllowed = WibboEnvironment.GetSettings().GetData<string>("room.owner.enter.not.allowed").Split(',');

        if (!this.GetClient().GetUser().HasPermission("perm_access_apartments_all"))
        {
            if (!(this.GetClient().GetUser().HasPermission("perm_access_apartments") && !ownerEnterNotAllowed.Contains(room.RoomData.OwnerName)) && !room.CheckRights(this.GetClient(), true) && !(this.GetClient().GetUser().IsTeleporting && this.GetClient().GetUser().TeleportingRoomID == room.Id))
            {
                if (room.RoomData.Access == RoomAccess.Doorbell && !overrideDoorbell && !room.CheckRights(this.GetClient()))
                {
                    if (room.UserCount == 0)
                    {
                        this.GetClient().SendPacket(new FlatAccessDeniedComposer(""));
                    }
                    else
                    {
                        this.GetClient().SendPacket(new DoorbellComposer(""));
                        room.SendPacket(new DoorbellComposer(this.GetClient().GetUser().Username), true);
                        this.GetClient().GetUser().LoadingRoomId = id;
                        this.GetClient().GetUser().AllowDoorBell = false;
                    }
                    return;
                }
                else if (room.RoomData.Access == RoomAccess.Password && password.ToLower() != room.RoomData.Password.ToLower())
                {
                    this.GetClient().SendPacket(new GenericErrorComposer(-100002));
                    this.GetClient().SendPacket(new CloseConnectionComposer());
                    return;
                }
            }
        }

        if (room.RoomData.OwnerName == WibboEnvironment.GetSettings().GetData<string>("autogame.owner"))
        {
            if (room.RoomUserManager.GetUserByTracker(this.IP, this.GetClient().MachineId) != null)
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
            this.GetClient().GetUser().LoadingRoomId = id;
            this.GetClient().GetUser().AllowDoorBell = true;
        }

    }

    public bool EnterRoom(Room room)
    {
        var session = this.GetClient();
        if (session == null)
        {
            return false;
        }

        if (room == null)
        {
            return false;
        }

        session.SendPacket(new RoomReadyComposer(room.Id, room.RoomData.ModelName));

        if (room.RoomData.Wallpaper != "0.0")
        {
            session.SendPacket(new RoomPropertyComposer("wallpaper", room.RoomData.Wallpaper));
        }

        if (room.RoomData.Floor != "0.0")
        {
            session.SendPacket(new RoomPropertyComposer("floor", room.RoomData.Floor));
        }

        session.SendPacket(new RoomPropertyComposer("landscape", room.RoomData.Landscape));
        session.SendPacket(new RoomRatingComposer(room.RoomData.Score, !(session.GetUser().RatedRooms.Contains(room.Id) || room.RoomData.OwnerId == session.GetUser().Id)));

        session.SendPacket(room.GameMap.Model.SerializeRelativeHeightmap());
        session.SendPacket(room.GameMap.Model.GetHeightmap());

        return true;
    }

    public bool HasExactPermission(string fuse)
    {
        if (WibboEnvironment.GetGame().GetPermissionManager().RankExactRight(this.Rank, fuse))
        {
            return true;
        }

        return false;
    }

    public bool HasPermission(string fuse)
    {
        if (WibboEnvironment.GetGame().GetPermissionManager().RankHasRight(this.Rank, fuse))
        {
            return true;
        }

        return false;
    }

    public void Dispose()
    {
        if (this.IsDisposed)
        {
            return;
        }

        this.IsDisposed = true;

        WibboEnvironment.GetGame().GetGameClientManager().UnregisterClient(this.Id, this.Username);

        if (this.HasPermission("perm_mod"))
        {
            WibboEnvironment.GetGame().GetGameClientManager().RemoveUserStaff(this.Id);
        }

        ExceptionLogger.WriteLine(this.Username + " has logged out.");

        if (!this.InfoSaved)
        {
            this.InfoSaved = true;
            var timeOnline = DateTime.Now - this.OnlineTime;
            var timeOnlineSec = (int)timeOnline.TotalSeconds;
            using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
            UserDao.UpdateOffline(dbClient, this.Id, this.Duckets, this.Credits);
            UserStatsDao.UpdateAll(dbClient, this.Id, this.FavouriteGroupId, timeOnlineSec, this.CurrentQuestId, this.Respect, this.DailyRespectPoints, this.DailyPetRespectPoints);
        }

        if (this.InRoom && this.CurrentRoom != null)
        {
            this.CurrentRoom.RoomUserManager.RemoveUserFromRoom(this._clientInstance, false, false);
        }

        if (this.RolePlayId > 0)
        {
            var rpManager = WibboEnvironment.GetGame().GetRoleplayManager().GetRolePlay(this.RolePlayId);
            if (rpManager != null)
            {
                var rp = rpManager.GetPlayer(this.Id);
                if (rp != null)
                {
                    rpManager.RemovePlayer(this.Id);
                }
            }
            this.RolePlayId = 0;
        }

        if (this.GuideOtherUserId != 0)
        {
            var requester = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(this.GuideOtherUserId);
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

        GC.SuppressFinalize(this);
    }

    public GameClient GetClient() => this._clientInstance;

    public MessengerComponent GetMessenger() => this._messengerComponent;

    public WardrobeComponent GetWardrobeComponent() => this._wardrobeComponent;

    public AchievementComponent GetAchievementComponent() => this._achievementComponent;

    public BadgeComponent GetBadgeComponent() => this._badgeComponent;

    public InventoryComponent GetInventoryComponent() => this._inventoryComponent;

    public ChatlogManager GetChatMessageManager() => this._chatMessageManager;

    public PermissionComponent GetPermissions() => this._permissions;

    public int GetQuestProgress(int p)
    {
        _ = this.Quests.TryGetValue(p, out var num);
        return num;
    }
}
