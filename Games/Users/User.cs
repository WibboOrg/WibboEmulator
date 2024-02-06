namespace WibboEmulator.Games.Users;
using System.Data;
using WibboEmulator.Communication.Packets.Outgoing.Help;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Session;
using WibboEmulator.Core;
using WibboEmulator.Core.Language;
using WibboEmulator.Database.Daos.Guild;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.Chats.Logs;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Users.Achievements;
using WibboEmulator.Games.Users.Badges;
using WibboEmulator.Games.Users.Inventory;
using WibboEmulator.Games.Users.Messenger;
using WibboEmulator.Games.Users.Permissions;
using WibboEmulator.Games.Users.Wardrobes;
using WibboEmulator.Games.Users.Premium;
using WibboEmulator.Utilities;
using WibboEmulator.Database.Daos.Log;
using WibboEmulator.Communication.Packets.Outgoing.WibboTool;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Notifications;
using WibboEmulator.Games.Banners;

public class User : IDisposable, IEquatable<User>
{
    public GameClient Client { get; private set; }
    public MessengerComponent Messenger { get; private set; }
    public WardrobeComponent WardrobeComponent { get; private set; }
    public AchievementComponent AchievementComponent { get; private set; }
    public BadgeComponent BadgeComponent { get; private set; }
    public InventoryComponent InventoryComponent { get; private set; }
    public ChatlogManager ChatMessageManager { get; private set; }
    public PermissionComponent Permissions { get; private set; }
    public PremiumComponent Premium { get; private set; }
    public BannerComponent Banner { get; private set; }

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
    public int GamePointsMonth { get; set; }
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
    public DateTime HereTimer { get; set; }
    public DateTime EveryoneTimer { get; set; }
    public DateTime CommandFunTimer { get; set; }
    public DateTime LastGiftPurchaseTime { get; set; }
    public DateTime LastLTDPurchaseTime { get; set; }
    public DateTime LastCreditsTime { get; set; } = DateTime.Now;
    public SpamDetect LastLoadedRoomTime { get; } = new(TimeSpan.FromSeconds(2), TimeSpan.FromMinutes(1), 5);

    public Banner BannerSelected { get; set; }
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
    public bool IsFirstConnexionToday { get; set; }
    public int GiftPurchasingWarnings { get; set; }
    public bool SessionGiftBlocked { get; set; }
    public int RolePlayId { get; set; }
    public int IgnoreAllExpireTime { get; set; }
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
        bool hideOnline, int mazoHighScore, int mazo, string clientVolume, bool nuxenable, bool isFirstConnexionToday,
        Language langue, int ignoreAllExpire, bool ignoreRoomInvite, bool cameraFollowDisabled, int gamePointsMonth, int bannerId)
    {
        this.Id = id;
        this.Username = username;
        this.Rank = rank;
        this.Motto = motto;
        this.Look = WibboEnvironment.GetFigureManager().ProcessFigure(look, gender, true);
        this.Gender = gender.ToUpper();
        this.Credits = credits;
        this.WibboPoints = wpoint;
        this.LimitCoins = limitCoins;
        this.Duckets = activityPoints;
        this.GamePointsMonth = gamePointsMonth;
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
        this.CanChangeName = this.HasPermission("change_name") && isFirstConnexionToday;
        this.IsFirstConnexionToday = isFirstConnexionToday;
        this.Langue = langue;
        this.IgnoreAllExpireTime = ignoreAllExpire;
        this.BannerSelected = WibboEnvironment.GetGame().GetBannerManager().GetBannerById(bannerId);

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

    public void Initialize(IDbConnection dbClient, GameClient client)
    {
        this.Client = client;

        this.InventoryComponent = new InventoryComponent(this);
        this.Permissions = new PermissionComponent();
        this.BadgeComponent = new BadgeComponent(this);
        this.AchievementComponent = new AchievementComponent(this);
        this.WardrobeComponent = new WardrobeComponent(this);
        this.Messenger = new MessengerComponent(this);
        this.ChatMessageManager = new ChatlogManager();
        this.Premium = new PremiumComponent(this);
        this.Banner = new BannerComponent(this);

        this.BadgeComponent.Initialize(dbClient);
        this.AchievementComponent.Initialize(dbClient);
        this.WardrobeComponent.Initialize(dbClient);
        this.Messenger.Initialize(dbClient, this.HideOnline);
        this.ChatMessageManager.LoadUserChatlogs(dbClient, this.Id);
        this.Premium.Initialize(dbClient);
        this.Banner.Initialize(dbClient);

        var userRoomIdList = RoomDao.GetAllIdByOwner(dbClient, this.Username);
        foreach (var userRoomId in userRoomIdList)
        {
            this.UsersRooms.Add(userRoomId);
        }

        var guildMembershipList = GuildMembershipDao.GetAllByUserId(dbClient, this.Id);
        foreach (var guildMembership in guildMembershipList)
        {
            this.MyGroups.Add(guildMembership);
        }

        var userQuestList = UserQuestDao.GetAll(dbClient, this.Id);
        foreach (var userQuest in userQuestList)
        {
            var questId = userQuest.QuestId;
            var progress = userQuest.Progress;
            this.Quests.Add(questId, progress);
        }

        var userFavoriteList = UserFavoriteDao.GetAll(dbClient, this.Id);
        foreach (var userFavorite in userFavoriteList)
        {
            this.FavoriteRooms.Add(userFavorite);
        }

        var roomRightList = RoomRightDao.GetAllByUserId(dbClient, this.Id);
        foreach (var roomRight in roomRightList)
        {
            this.RoomRightsList.Add(roomRight);
        }
    }

    public bool CheckChatMessage(string message, string type, int roomId = 0)
    {
        if (message.Length <= 3 || this.HasPermission("god"))
        {
            return false;
        }

        var staffUsers = WibboEnvironment.GetGame().GetGameClientManager().GetStaffUsers()
            .Where(s => s != null && s.User != null)
            .ToList();

        using var dbClient = WibboEnvironment.GetDatabaseManager().Connection();

        LogChatDao.Insert(dbClient, this.Id, roomId, message, type, this.Username);

        if (!WibboEnvironment.GetGame().GetChatManager().GetFilter().Ispub(message))
        {
            if (WibboEnvironment.GetGame().GetChatManager().GetFilter().CheckMessageWord(message))
            {
                LogChatPubDao.Insert(dbClient, this.Id, "A vérifié: " + type + message, this.Username);

                foreach (var staffUser in staffUsers)
                {
                    staffUser.SendPacket(new AddChatlogsComposer(this.Id, this.Username, type + message));
                }

                return false;
            }

            return false;
        }

        var pubCount = this.PubDetectCount++;

        if (type == "<CMD>")
        {
            pubCount = 4;
        }

        LogChatPubDao.Insert(dbClient, this.Id, "Pub numero " + pubCount + ": " + type + message, this.Username);

        if (pubCount is < 3 and > 0)
        {
            this.Client.SendNotification(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("notif.antipub.warn.1", this.Langue), pubCount));
        }
        else if (pubCount == 3)
        {
            this.Client.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.antipub.warn.2", this.Langue));
        }
        else if (pubCount == 4)
        {
            WibboEnvironment.GetGame().GetGameClientManager().BanUser(this.Client, "Robot", 86400, "Notre Robot a detecte de la pub pour sur le compte " + this.Username, true);
        }

        foreach (var staffUser in staffUsers)
        {
            staffUser.SendPacket(RoomNotificationComposer.SendBubble("mention", "Détection d'un message suspect sur le compte " + this.Username));
            staffUser.SendPacket(new AddChatlogsComposer(this.Id, this.Username, type + message));
        }
        return true;
    }

    public bool EnterRoom(Room room)
    {
        if (this.Client == null)
        {
            return false;
        }

        this.Client.SendPacket(new RoomReadyComposer(room.Id, room.RoomData.ModelName));

        if (room.RoomData.Wallpaper != "0.0")
        {
            this.Client.SendPacket(new RoomPropertyComposer("wallpaper", room.RoomData.Wallpaper));
        }

        if (room.RoomData.Floor != "0.0")
        {
            this.Client.SendPacket(new RoomPropertyComposer("floor", room.RoomData.Floor));
        }

        this.Client.SendPacket(new RoomPropertyComposer("landscape", room.RoomData.Landscape));
        this.Client.SendPacket(new RoomRatingComposer(room.RoomData.Score, !(this.Client.User.RatedRooms.Contains(room.Id) || room.RoomData.OwnerId == this.Client.User.Id)));

        this.Client.SendPacket(room.GameMap.Model.SerializeRelativeHeightmap());
        this.Client.SendPacket(room.GameMap.Model.GetHeightmap());

        return true;
    }

    public bool HasPermission(string fuse)
    {
        if ((fuse == "premium_classic" && this.Premium.IsPremiumClassic) ||
           (fuse == "premium_epic" && this.Premium.IsPremiumEpic) ||
           (fuse == "premium_legend" && this.Premium.IsPremiumLegend))
        {
            return true;
        }

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

        WibboEnvironment.GetGame().GetGameClientManager().UnregisterClient(this.Id, this.Username, this.Client.SSOTicket);

        if (this.HasPermission("mod"))
        {
            WibboEnvironment.GetGame().GetGameClientManager().RemoveUserStaff(this.Id);
        }

        ExceptionLogger.WriteLine(this.Username + " has logged out.");

        this.SaveInfo();

        if (this.InRoom && this.CurrentRoom != null)
        {
            this.CurrentRoom.RoomUserManager.RemoveUserFromRoom(this.Client, false, false);
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

                requester.
                User.GuideOtherUserId = 0;
            }
        }
        if (this.OnDuty)
        {
            WibboEnvironment.GetGame().GetHelpManager().TryRemoveGuide(this.Id);
        }

        if (this.Messenger != null)
        {
            this.Messenger.AppearOffline = true;
            this.Messenger.Dispose();
        }

        this.InventoryComponent?.Dispose();
        this.BadgeComponent?.Dispose();
        this.WardrobeComponent?.Dispose();
        this.AchievementComponent?.Dispose();
        this.Premium?.Dispose();
        this.Banner?.Dispose();
        this.UsersRooms?.Clear();
        this.RoomRightsList?.Clear();
        this.FavoriteRooms?.Clear();
        this.Client = null;

        GC.SuppressFinalize(this);
    }

    public void SaveInfo(IDbConnection dbClient = null)
    {
        if (this.InfoSaved)
        {
            return;
        }

        dbClient ??= WibboEnvironment.GetDatabaseManager().Connection();

        this.InfoSaved = true;
        var timeOnline = DateTime.Now - this.OnlineTime;
        var timeOnlineSec = (int)timeOnline.TotalSeconds;

        UserDao.UpdateOffline(dbClient, this.Id, this.Duckets, this.Credits, this.BannerSelected != null ? this.BannerSelected.Id : -1);
        UserStatsDao.UpdateAll(dbClient, this.Id, this.FavouriteGroupId, timeOnlineSec, this.CurrentQuestId, this.Respect, this.DailyRespectPoints, this.DailyPetRespectPoints);
    }

    public int GetQuestProgress(int p)
    {
        _ = this.Quests.TryGetValue(p, out var num);
        return num;
    }

    public override bool Equals(object obj)
    {
        if (obj is not User)
        {
            return false;
        }

        return ((User)obj).Id == this.Id;
    }

    public override int GetHashCode() => this.Id;
    public bool Equals(User other) => other.Id == this.Id;
}
