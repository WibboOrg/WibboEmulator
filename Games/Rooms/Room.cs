namespace WibboEmulator.Games.Rooms;
using System.Data;
using WibboEmulator.Communication.Interfaces;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Avatar;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Session;
using WibboEmulator.Core;
using WibboEmulator.Database.Daos.Bot;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.Chat.Logs;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms.AI;
using WibboEmulator.Games.Rooms.Games;
using WibboEmulator.Games.Rooms.Games.Banzai;
using WibboEmulator.Games.Rooms.Games.Football;
using WibboEmulator.Games.Rooms.Games.Freeze;
using WibboEmulator.Games.Rooms.Games.Teams;
using WibboEmulator.Games.Rooms.Jankens;
using WibboEmulator.Games.Rooms.Map;
using WibboEmulator.Games.Rooms.Moodlight;
using WibboEmulator.Games.Rooms.Projectile;
using WibboEmulator.Games.Rooms.Trading;
using WibboEmulator.Games.Rooms.Wired;
using WibboEmulator.Utilities;
using WibboEmulator.Utilities.Events;

public delegate void RoomUserSaysEvent(object sender, UserSaysEventArgs e, ref bool messageHandled);

public class Room
{
    public int IsLagging { get; set; }
    public int IdleTime { get; set; }
    public bool Disposed { get; set; }

    public Task ProcessTask { get; set; }

    public RoomRoleplay Roleplay { get; set; }
    public bool IsRoleplay => this.Roleplay != null;
    public List<int> UsersWithRights { get; set; }
    public bool EveryoneGotRights { get; set; }
    public bool HeightMapLoaded { get; set; }
    public DateTime LastTimerReset { get; set; }
    public MoodlightData MoodlightData { get; set; }
    public List<Trade> ActiveTrades { get; set; }
    public RoomData RoomData { get; set; }

    private readonly TimeSpan _maximumRunTimeInSec = TimeSpan.FromSeconds(1);

    private TeamManager _teamManager;
    private GameManager _gameManager;
    private readonly Gamemap _gameMap;
    private readonly RoomItemHandling _roomItemHandling;
    private readonly RoomUserManager _roomUserManager;
    private Soccer _soccer;
    private BattleBanzai _banzai;
    private Freeze _freeze;
    private JankenManager _jankan;
    private GameItemHandler _gameItemHandler;
    private readonly WiredHandler _wiredHandler;
    private readonly ProjectileManager _projectileManager;
    private readonly ChatlogManager _chatMessageManager;

    private readonly Dictionary<int, double> _bans;
    private readonly Dictionary<int, double> _mutes;

    public bool RoomMuted { get; set; }
    public bool RoomMutePets { get; set; }
    public bool FreezeRoom { get; set; }
    public bool PushPullAllowed { get; set; }
    public bool CloseFullRoom { get; set; }
    public bool OldFoot { get; set; }
    public bool RoomIngameChat { get; set; }

    private int _saveFurnitureTimer;

    //Question
    public int VotedYesCount { get; set; }
    public int VotedNoCount { get; set; }

    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public int UserCount => this._roomUserManager.GetRoomUserCount();

    public int Id => this.RoomData.Id;

    public event RoomUserSaysEvent OnUserSays;
    public event EventHandler OnTrigger;
    public event EventHandler OnTriggerSelf;
    public event EventHandler OnUserCls;

    public Room(RoomData data)
    {
        var rpManager = WibboEnvironment.GetGame().GetRoleplayManager().GetRolePlay(data.OwnerId);
        if (rpManager != null)
        {
            this.Roleplay = new RoomRoleplay();
        }

        this._saveFurnitureTimer = 0;
        this.Disposed = false;
        this._bans = new Dictionary<int, double>();
        this._mutes = new Dictionary<int, double>();
        this.ActiveTrades = new List<Trade>();
        this.HeightMapLoaded = false;
        this.RoomData = data;
        this.EveryoneGotRights = data.AllowRightsOverride;
        this.IdleTime = 0;
        this.RoomMuted = false;
        this.PushPullAllowed = true;
        this.RoomIngameChat = false;

        this._gameMap = new Gamemap(this);
        this._roomItemHandling = new RoomItemHandling(this);
        this._roomUserManager = new RoomUserManager(this);
        this._wiredHandler = new WiredHandler(this);
        this._projectileManager = new ProjectileManager(this);
        this._chatMessageManager = new ChatlogManager();

        this._chatMessageManager.LoadRoomChatlogs(this.Id);

        this.GetRoomItemHandler().LoadFurniture();
        if (this.RoomData.OwnerName == WibboEnvironment.GetSettings().GetData<string>("autogame.owner"))
        {
            this.GetRoomItemHandler().LoadFurniture(WibboEnvironment.GetSettings().GetData<int>("autogame.deco.room.id"));
        }

        this.GetGameMap().GenerateMaps(true);
        this.LoadRights();
        this.LoadBots();
        this.InitPets();
        this.LastTimerReset = DateTime.Now;
    }

    public Gamemap GetGameMap() => this._gameMap;

    public RoomItemHandling GetRoomItemHandler() => this._roomItemHandling;

    public RoomUserManager GetRoomUserManager() => this._roomUserManager;

    public Soccer GetSoccer()
    {
        this._soccer ??= new Soccer(this);

        return this._soccer;
    }

    public TeamManager GetTeamManager()
    {
        this._teamManager ??= new TeamManager();

        return this._teamManager;
    }

    public BattleBanzai GetBanzai()
    {
        this._banzai ??= new BattleBanzai(this);

        return this._banzai;
    }

    public Freeze GetFreeze()
    {
        this._freeze ??= new Freeze(this);

        return this._freeze;
    }

    public JankenManager GetJanken()
    {
        this._jankan ??= new JankenManager(this);

        return this._jankan;
    }

    public GameManager GetGameManager()
    {
        this._gameManager ??= new GameManager(this);

        return this._gameManager;
    }

    public GameItemHandler GetGameItemHandler()
    {
        this._gameItemHandler ??= new GameItemHandler(this);

        return this._gameItemHandler;
    }

    public WiredHandler GetWiredHandler() => this._wiredHandler;

    public ProjectileManager GetProjectileManager() => this._projectileManager;

    public bool GotSoccer() => this._soccer != null;

    public bool GotBanzai() => this._banzai != null;

    public bool GotFreeze() => this._freeze != null;

    public bool GotJanken() => this._jankan != null;

    public bool GotWired() => this._wiredHandler != null;

    public ChatlogManager GetChatMessageManager() => this._chatMessageManager;

    public bool AllowsShous(RoomUser user, string message)
    {
        var messageHandled = false;
        this.OnUserSays?.Invoke(null, new UserSaysEventArgs(user, message), ref messageHandled);

        return messageHandled;
    }

    public void CollisionUser(RoomUser user)
    {
        if (this.OnUserCls == null)
        {
            return;
        }

        var lenght = 1;
        var goalX = user.X;
        var goalY = user.Y;

        switch (user.RotBody)
        {
            case 0:
                goalX = user.X;
                goalY = user.Y - lenght;
                break;
            case 1:
                goalX = user.X + lenght;
                goalY = user.Y - lenght;
                break;
            case 2:
                goalX = user.X + lenght;
                goalY = user.Y;
                break;
            case 3:
                goalX = user.X + lenght;
                goalY = user.Y + lenght;
                break;
            case 4:
                goalX = user.X;
                goalY = user.Y + lenght;
                break;
            case 5:
                goalX = user.X - lenght;
                goalY = user.Y + lenght;
                break;
            case 6:
                goalX = user.X - lenght;
                goalY = user.Y;
                break;
            case 7:
                goalX = user.X - lenght;
                goalY = user.Y - lenght;
                break;
            default:
                break;
        }

        var userGoal = this.GetRoomUserManager().GetUserForSquare(goalX, goalY);
        if (userGoal == null)
        {
            return;
        }

        if (userGoal.Team == user.Team && user.Team != TeamType.NONE)
        {
            return;
        }

        this.OnUserCls(userGoal, new());
    }

    public void OnTriggerUser(RoomUser roomUser, bool isTarget)
    {
        if (isTarget)
        {
            this.OnTrigger?.Invoke(roomUser, new());
        }
        else
        {
            this.OnTriggerSelf?.Invoke(roomUser, new());
        }
    }

    public void ClearTags() => this.RoomData.Tags.Clear();

    public void AddTagRange(List<string> tags) => this.RoomData.Tags.AddRange(tags);

    private void LoadBots()
    {
        DataTable table;
        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        table = BotUserDao.GetOneByRoomId(dbClient, this.Id);
        if (table == null)
        {
            return;
        }

        foreach (DataRow row in table.Rows)
        {
            var roomBot = new RoomBot(Convert.ToInt32(row["id"]), Convert.ToInt32(row["user_id"]), Convert.ToInt32(row["room_id"]), this.IsRoleplay ? BotAIType.RoleplayBot : BotAIType.Generic, (string)row["walk_enabled"] == "1", (string)row["name"], (string)row["motto"], (string)row["gender"], (string)row["look"], Convert.ToInt32(row["x"]), Convert.ToInt32(row["y"]), Convert.ToInt32(row["z"]), Convert.ToInt32(row["rotation"]), (string)row["chat_enabled"] == "1", (string)row["chat_text"], Convert.ToInt32(row["chat_seconds"]), (string)row["is_dancing"] == "1", Convert.ToInt32(row["enable"]), Convert.ToInt32(row["handitem"]), Convert.ToInt32((string)row["status"]));
            var roomUser = this.GetRoomUserManager().DeployBot(roomBot, null);
            if (roomBot.IsDancing)
            {
                roomUser.DanceId = 3;
            }
        }
    }

    public void InitPets()
    {
        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        var table = BotPetDao.GetAllByRoomId(dbClient, this.Id);
        if (table == null)
        {
            return;
        }

        foreach (DataRow row in table.Rows)
        {
            var petData = new Pet(Convert.ToInt32(row["id"]), Convert.ToInt32(row["user_id"]), Convert.ToInt32(row["room_id"]), (string)row["name"], Convert.ToInt32(row["type"]), (string)row["race"], (string)row["color"], Convert.ToInt32(row["experience"]), Convert.ToInt32(row["energy"]), Convert.ToInt32(row["nutrition"]), Convert.ToInt32(row["respect"]), (double)row["createstamp"], Convert.ToInt32(row["x"]), Convert.ToInt32(row["y"]), (double)row["z"], Convert.ToInt32(row["have_saddle"]), Convert.ToInt32(row["hairdye"]), Convert.ToInt32(row["pethair"]), (string)row["anyone_ride"] == "1");
            _ = this._roomUserManager.DeployBot(new RoomBot(petData.PetId, petData.OwnerId, this.Id, BotAIType.Pet, true, petData.Name, "", "", petData.Look, petData.X, petData.Y, petData.Z, 0, false, "", 0, false, 0, 0, 0), petData);
        }
    }

    public void OnRoomKick()
    {
        foreach (var roomUser in this._roomUserManager.GetUserList().ToList())
        {
            if (!roomUser.IsBot && !roomUser.GetClient().GetUser().HasPermission("perm_no_kick"))
            {
                this.GetRoomUserManager().RemoveUserFromRoom(roomUser.GetClient(), true, true);
            }
        }
    }

    public void OnUserSay(RoomUser user, string message, bool shout)
    {
        foreach (var roomUser in this._roomUserManager.GetPets().ToList())
        {
            if (shout)
            {
                roomUser.BotAI.OnUserShout(user, message);
            }
            else
            {
                roomUser.BotAI.OnUserSay(user, message);
            }
        }
    }

    public void LoadRights()
    {
        this.UsersWithRights = new List<int>();
        var dataTable = new DataTable();
        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            dataTable = RoomRightDao.GetAllByRoomId(dbClient, this.RoomData.Id);
        }

        if (dataTable == null)
        {
            return;
        }

        foreach (DataRow dataRow in dataTable.Rows)
        {
            this.UsersWithRights.Add(Convert.ToInt32(dataRow["user_id"]));
        }
    }

    public int GetRightsLevel(GameClient session)
    {
        if (session == null || session.GetUser() == null)
        {
            return 0;
        }

        if (session.GetUser().Username == this.RoomData.OwnerName || session.GetUser().HasPermission("perm_owner_all_rooms"))
        {
            return 4;
        }

        if (session.GetUser().HasPermission("perm_room_rights"))
        {
            return 3;
        }

        if (this.UsersWithRights.Contains(session.GetUser().Id))
        {
            return 1;
        }

        if (this.EveryoneGotRights)
        {
            return 1;
        }

        return 0;
    }

    public bool CheckRights(GameClient session, bool requireOwnership = false)
    {
        if (session == null || session.GetUser() == null)
        {
            return false;
        }

        if (session.GetUser().Username == this.RoomData.OwnerName || session.GetUser().HasPermission("perm_owner_all_rooms"))
        {
            return true;
        }

        if (!requireOwnership)
        {
            if (session.GetUser().HasPermission("perm_room_rights") || this.UsersWithRights.Contains(session.GetUser().Id))
            {
                return true;
            }

            if (this.EveryoneGotRights)
            {
                return true;
            }

            if (this.RoomData.Group == null)
            {
                return false;
            }

            if (this.RoomData.Group.IsAdmin(session.GetUser().Id))
            {
                return true;
            }

            if (this.RoomData.Group.AdminOnlyDeco == 0)
            {
                if (this.RoomData.Group.IsMember(session.GetUser().Id))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void SendObjects(GameClient session)
    {
        var packetList = new ServerPacketList();

        foreach (var roomUser in this._roomUserManager.GetUserList().ToList())
        {
            if (roomUser == null)
            {
                continue;
            }

            if (roomUser.IsSpectator)
            {
                continue;
            }

            if (!roomUser.IsBot && roomUser.GetClient() == null)
            {
                continue;
            }

            if (!roomUser.IsBot && roomUser.GetClient().GetUser() == null)
            {
                continue;
            }

            packetList.Add(new UsersComposer(roomUser));

            if (roomUser.IsDancing)
            {
                packetList.Add(new DanceComposer(roomUser.VirtualId, roomUser.DanceId));
            }

            if (roomUser.IsAsleep)
            {
                packetList.Add(new SleepComposer(roomUser.VirtualId, true));
            }

            if (roomUser.CarryItemID > 0 && roomUser.CarryTimer > 0)
            {
                packetList.Add(new CarryObjectComposer(roomUser.VirtualId, roomUser.CarryItemID));
            }

            if (roomUser.CurrentEffect > 0)
            {
                packetList.Add(new AvatarEffectComposer(roomUser.VirtualId, roomUser.CurrentEffect));
            }
        }

        packetList.Add(new UserUpdateComposer(this._roomUserManager.GetUserList().ToList()));
        packetList.Add(new ObjectsComposer(this.GetRoomItemHandler().GetFloor.ToArray(), this));
        packetList.Add(new ObjectsComposer(this.GetRoomItemHandler().GetTempItems.ToArray(), this));
        packetList.Add(new ItemWallComposer(this.GetRoomItemHandler().GetWall.ToArray(), this));

        session.SendPacket(packetList);
        packetList.Clear();
    }

    public Task ProcessRoom()
    {
        try
        {
            var timeStarted = DateTime.Now;

            if (this.Disposed)
            {
                return Task.CompletedTask;
            }

            try
            {
                var idleCount = 0;

                this.GetRoomUserManager().OnCycle(ref idleCount);

                this.GetRoomItemHandler().OnCycle();

                this.RpCycleHour();

                this.GetProjectileManager().OnCycle();

                if (idleCount > 0)
                {
                    this.IdleTime++;
                }
                else
                {
                    this.IdleTime = 0;
                }

                if (this.IdleTime >= 60)
                {
                    WibboEnvironment.GetGame().GetRoomManager().UnloadRoom(this);

                    return Task.CompletedTask;
                }
                else
                {
                    this.GetRoomUserManager().SerializeStatusUpdates();
                }

                if (this.GetGameItemHandler() != null)
                {
                    this.GetGameItemHandler().OnCycle();
                }

                if (this.GetWiredHandler() != null)
                {
                    this.GetWiredHandler().OnCycle();
                }

                if (this.GotJanken())
                {
                    this.GetJanken().OnCycle();
                }

                if (this._saveFurnitureTimer < 240)
                {
                    this._saveFurnitureTimer++;
                }
                else
                {
                    this._saveFurnitureTimer = 0;
                    this.GetRoomItemHandler().SaveFurniture();
                }

                var timeEnded = DateTime.Now;

                var timeExecution = timeEnded - timeStarted;
                if (timeExecution > this._maximumRunTimeInSec)
                {
                    ExceptionLogger.LogThreadException(string.Format("High latency in {0}: {1}ms", this.Id, timeExecution.TotalMilliseconds), "ProcessRoom");
                }
            }
            catch (Exception ex)
            {
                this.OnRoomCrash(ex);
            }
        }
        catch (Exception ex)
        {
            ExceptionLogger.LogCriticalException("Sub crash in room cycle: " + ex.ToString());
        }
        return Task.CompletedTask;
    }

    private void RpCycleHour()
    {
        if (!this.IsRoleplay)
        {
            return;
        }

        if (this.RoomData.OwnerName == "WibboParty")
        {
            return;
        }

        if (!this.Roleplay.Cycle())
        {
            return;
        }

        this.UpdateRpMoodLight();
        this.UpdateRpToner();
        this.UpdateRpBlock();
    }

    private void UpdateRpBlock()
    {
        var roomItems = this.GetRoomItemHandler().GetFloor.Where(i => i.GetBaseItem().Id == 99138022).ToList();
        if (roomItems == null)
        {
            return;
        }

        var useNum = 0;
        if (this.Roleplay.Intensity == 50)
        {
            useNum = 0;
        }
        else if (this.Roleplay.Intensity == 75)
        {
            useNum = 1;
        }
        else if (this.Roleplay.Intensity == 100)
        {
            useNum = 2;
        }
        else if (this.Roleplay.Intensity == 150)
        {
            useNum = 3;
        }
        else if (this.Roleplay.Intensity == 200)
        {
            useNum = 4;
        }
        else if (this.Roleplay.Intensity == 255)
        {
            useNum = 5;
        }

        foreach (var roomItem in roomItems)
        {
            roomItem.ExtraData = useNum.ToString();
            roomItem.UpdateState();
        }
    }

    private void UpdateRpMoodLight()
    {
        if (this.MoodlightData == null)
        {
            return;
        }

        var roomItem = this.GetRoomItemHandler().GetItem(this.MoodlightData.ItemId);
        if (roomItem == null || roomItem.GetBaseItem().InteractionType != InteractionType.MOODLIGHT)
        {
            return;
        }

        this.MoodlightData.Enabled = true;
        this.MoodlightData.CurrentPreset = 1;
        this.MoodlightData.UpdatePreset(1, "#000000", this.Roleplay.Intensity, false);
        roomItem.ExtraData = this.MoodlightData.GenerateExtraData();
        roomItem.UpdateState();
    }

    private void UpdateRpToner()
    {
        var roomItem = this.GetRoomItemHandler().GetFloor.FirstOrDefault(i => i.GetBaseItem().InteractionType == InteractionType.TONER);
        if (roomItem == null)
        {
            return;
        }

        var teinte = 135;
        var saturation = 180;
        var luminosite = (int)Math.Floor((double)this.Roleplay.Intensity / 2);
        roomItem.ExtraData = "on," + teinte + "," + saturation + "," + luminosite;
        roomItem.UpdateState(true, true);
    }

    public void OnRoomCrash(Exception e)
    {
        ExceptionLogger.LogThreadException(e.ToString(), "Room cycle task for room " + this.Id);
        WibboEnvironment.GetGame().GetRoomManager().UnloadRoom(this);
    }

    public void SendPacketOnChat(IServerPacket message, RoomUser thisUser = null, bool userMutedOnly = false, bool userNotIngameOnly = false)
    {
        try
        {
            if (message == null)
            {
                return;
            }

            if (this == null || this._roomUserManager == null)
            {
                return;
            }

            var users = this._roomUserManager.GetUserList().ToList();
            if (users == null)
            {
                return;
            }

            foreach (var user in users)
            {
                if (user == null || user.IsBot)
                {
                    continue;
                }

                if (user.GetClient() == null || user.GetClient().GetConnection() == null || user.GetClient().GetUser() == null)
                {
                    continue;
                }

                if (userMutedOnly && thisUser != null && user.GetClient().GetUser().MutedUsers.Contains(thisUser.UserId))
                {
                    continue;
                }

                if (thisUser != null && thisUser.GetClient() != null && thisUser.GetClient().GetUser() != null && thisUser.GetClient().GetUser().IgnoreAll && thisUser != user)
                {
                    continue;
                }

                if (!userMutedOnly && thisUser == user)
                {
                    continue;
                }

                if (this.RoomIngameChat && userNotIngameOnly && user.Team != TeamType.NONE)
                {
                    continue;
                }

                if (thisUser != null && this.RoomData.ChatMaxDistance > 0 && (Math.Abs(thisUser.X - user.X) > this.RoomData.ChatMaxDistance || Math.Abs(thisUser.Y - user.Y) > this.RoomData.ChatMaxDistance))
                {
                    continue;
                }

                user.GetClient().SendPacket(message);
            }
        }
        catch (Exception ex)
        {
            ExceptionLogger.HandleException(ex, "Room.SendMessage (" + this.Id + ")");
        }
    }

    public void SendPacket(IServerPacket message, bool usersWithRightsOnly = false)
    {
        try
        {
            if (message == null)
            {
                return;
            }

            if (this == null || this._roomUserManager == null)
            {
                return;
            }

            var users = this._roomUserManager.GetUserList().ToList();
            if (users == null)
            {
                return;
            }

            foreach (var user in users)
            {
                if (user == null || user.IsBot)
                {
                    continue;
                }

                if (user.GetClient() == null || user.GetClient().GetConnection() == null)
                {
                    continue;
                }

                if (usersWithRightsOnly && !this.CheckRights(user.GetClient()))
                {
                    continue;
                }

                user.GetClient().SendPacket(message);
            }
        }
        catch (Exception ex)
        {
            ExceptionLogger.HandleException(ex, "Room.SendMessage (" + this.Id + ")");
        }
    }

    public void SendMessage(ServerPacketList messages)
    {
        if (messages.Count == 0)
        {
            return;
        }

        this.BroadcastPacket(messages.GetBytes);
    }

    public void BroadcastPacket(byte[] packet)
    {
        foreach (var user in this._roomUserManager.GetUserList().ToList())
        {
            if (user == null || user.IsBot)
            {
                continue;
            }

            if (user.GetClient() == null || user.GetClient().GetConnection() == null)
            {
                continue;
            }

            user.GetClient().GetConnection().SendData(packet);
        }
    }

    public void Dispose()
    {
        if (this.Disposed)
        {
            return;
        }

        this.Disposed = true;

        this.SendPacket(new CloseConnectionComposer());

        try
        {
            if (this.ProcessTask != null && this.ProcessTask.IsCompleted)
            {
                this.ProcessTask.Dispose();
            }
        }
        catch { }

        this._cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(1));

        this.GetRoomItemHandler().SaveFurniture();

        this.RoomData.Tags.Clear();
        this.UsersWithRights.Clear();
        this._bans.Clear();

        foreach (var roomItem in this.GetRoomItemHandler().GetWallAndFloor)
        {
            roomItem.Destroy();
        }

        this.GetRoomItemHandler().Destroy();
        this.ActiveTrades.Clear();
        this.GetRoomUserManager().UpdateUserCount(0);
        this.GetRoomUserManager().Destroy();
        this._gameMap.Destroy();
    }

    public Dictionary<int, double> GetBans() => this._bans;

    public bool UserIsBanned(int pId) => this._bans.ContainsKey(pId);

    public void RemoveBan(int pId) => this._bans.Remove(pId);

    public void AddBan(int pId, int time)
    {
        if (this._bans.ContainsKey(pId))
        {
            return;
        }

        this._bans.Add(pId, WibboEnvironment.GetUnixTimestamp() + time);
    }

    public bool HasBanExpired(int pId) => !this.UserIsBanned(pId) || this._bans[pId] - WibboEnvironment.GetUnixTimestamp() <= 0.0;

    public Dictionary<int, double> GetMute() => this._mutes;

    public bool UserIsMuted(int pId) => this._mutes.ContainsKey(pId);

    public void RemoveMute(int pId) => this._mutes.Remove(pId);

    public void AddMute(int pId, int time)
    {
        if (this._mutes.ContainsKey(pId))
        {
            return;
        }

        this._mutes.Add(pId, WibboEnvironment.GetUnixTimestamp() + time);
    }

    public bool HasMuteExpired(int pId) => !this.UserIsMuted(pId) || this._mutes[pId] - WibboEnvironment.GetUnixTimestamp() <= 0.0;

    public bool HasActiveTrade(RoomUser user)
    {
        if (user.IsBot)
        {
            return false;
        }
        else
        {
            return this.HasActiveTrade(user.GetClient().GetUser().Id);
        }
    }

    public bool HasActiveTrade(int userId)
    {
        foreach (var trade in this.ActiveTrades)
        {
            if (trade.ContainsUser(userId))
            {
                return true;
            }
        }
        return false;
    }

    public Trade GetUserTrade(int userId)
    {
        foreach (var trade in this.ActiveTrades)
        {
            if (trade.ContainsUser(userId))
            {
                return trade;
            }
        }
        return null;
    }

    public void TryStartTrade(RoomUser userOne, RoomUser userTwo)
    {
        if (userOne == null || userTwo == null)
        {
            return;
        }

        if (userOne.IsBot || userTwo.IsBot || userOne.IsTrading || userTwo.IsTrading || this.HasActiveTrade(userOne) || this.HasActiveTrade(userTwo))
        {
            return;
        }

        this.ActiveTrades.Add(new Trade(userOne.GetClient().GetUser().Id, userTwo.GetClient().GetUser().Id, this.Id));
    }

    public void TryStopTrade(int userId)
    {
        var userTrade = this.GetUserTrade(userId);
        if (userTrade == null)
        {
            return;
        }

        userTrade.CloseTrade(userId);
        _ = this.ActiveTrades.Remove(userTrade);
    }

    public void SetMaxUsers(int maxUsers)
    {
        this.RoomData.UsersMax = maxUsers;

        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        RoomDao.UpdateUsersMax(dbClient, this.Id, maxUsers);
    }

    public Task RunTask(Func<Task> callBack)
    {
        var task = Task.Run(async () =>
        {
            if (this.Disposed)
            {
                return;
            }

            await callBack();

        }, this._cancellationTokenSource.Token);

        return task;
    }
}
