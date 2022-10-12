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
    public int Id => this.Data.Id;
    public int IsLagging { get; set; }
    public int IdleTime { get; set; }
    public bool Disposed { get; set; }
    public Task ProcessTask { get; set; }
    public RoomRoleplay Roleplay { get; set; }
    public bool IsRoleplay => this.Roleplay != null;
    public List<int> UsersWithRights { get; set; }
    public DateTime LastTimerReset { get; set; }
    public MoodlightData MoodlightData { get; set; }
    public List<Trade> ActiveTrades { get; set; }
    public RoomData Data { get; set; }
    public bool RoomMuted { get; set; }
    public bool RoomMutePets { get; set; }
    public bool FreezeRoom { get; set; }
    public bool PushPullAllowed { get; set; }
    public bool CloseFullRoom { get; set; }
    public bool OldFoot { get; set; }
    public bool IngameChat { get; set; }

    //Question
    public int VotedYesCount { get; set; }
    public int VotedNoCount { get; set; }
    public int UserCount => this._roomUserManager.GetRoomUserCount();

    public event RoomUserSaysEvent OnUserSays;
    public event EventHandler OnTrigger;
    public event EventHandler OnTriggerSelf;
    public event EventHandler OnUserCls;

    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly TimeSpan _maximumRunTimeInSec = TimeSpan.FromSeconds(1);

    private readonly TeamManager _teamManager;
    private readonly GameManager _gameManager;
    private readonly Gamemap _gameMap;
    private readonly RoomItemHandling _roomItemHandling;
    private readonly RoomUserManager _roomUserManager;
    private readonly Soccer _soccer;
    private readonly BattleBanzai _banzai;
    private readonly Freeze _freeze;
    private readonly JankenManager _jankan;
    private readonly GameItemHandler _gameItemHandler;
    private readonly WiredHandler _wiredHandler;
    private readonly ProjectileManager _projectileManager;
    private readonly ChatlogManager _chatMessageManager;

    private readonly Dictionary<int, double> _bans;
    private readonly Dictionary<int, double> _mutes;

    private int _saveFurnitureTimer;

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
        this.Data = data;
        this.IdleTime = 0;
        this.RoomMuted = false;
        this.PushPullAllowed = true;
        this.IngameChat = false;

        this._gameMap = new Gamemap(this);
        this._roomItemHandling = new RoomItemHandling(this);
        this._roomUserManager = new RoomUserManager(this);
        this._wiredHandler = new WiredHandler(this);
        this._projectileManager = new ProjectileManager(this);
        this._chatMessageManager = new ChatlogManager();
        this._gameItemHandler = new GameItemHandler(this);
        this._gameManager = new GameManager(this);
        this._jankan = new JankenManager(this);
        this._freeze = new Freeze(this);
        this._banzai = new BattleBanzai(this);
        this._teamManager = new TeamManager();
        this._soccer = new Soccer(this);

        this._chatMessageManager.LoadRoomChatlogs(this.Id);

        this.GetRoomItemHandler().LoadFurniture();
        if (this.Data.OwnerName == WibboEnvironment.GetSettings().GetData<string>("autogame.owner"))
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

    public Soccer GetSoccer() => this._soccer;

    public TeamManager GetTeamManager() => this._teamManager;

    public BattleBanzai GetBanzai() => this._banzai;

    public Freeze GetFreeze() => this._freeze;

    public JankenManager GetJanken() => this._jankan;

    public GameManager GetGameManager() => this._gameManager;

    public GameItemHandler GetGameItemHandler() => this._gameItemHandler;

    public WiredHandler GetWiredHandler() => this._wiredHandler;

    public ProjectileManager GetProjectileManager() => this._projectileManager;

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

        if (userGoal.Team == user.Team && user.Team != TeamType.None)
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

    public void ClearTags() => this.Data.Tags.Clear();

    public void AddTagRange(List<string> tags) => this.Data.Tags.AddRange(tags);

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
            if (!roomUser.IsBot && !roomUser.Client.GetUser().HasPermission("perm_no_kick"))
            {
                this.GetRoomUserManager().RemoveUserFromRoom(roomUser.Client, true, true);
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
            dataTable = RoomRightDao.GetAllByRoomId(dbClient, this.Data.Id);
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

        if (session.GetUser().Username == this.Data.OwnerName || session.GetUser().HasPermission("perm_owner_all_rooms"))
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

        if (this.Data.AllowRightsOverride)
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

        if (session.GetUser().Username == this.Data.OwnerName || session.GetUser().HasPermission("perm_owner_all_rooms"))
        {
            return true;
        }

        if (!requireOwnership)
        {
            if (session.GetUser().HasPermission("perm_room_rights") || this.UsersWithRights.Contains(session.GetUser().Id))
            {
                return true;
            }

            if (this.Data.AllowRightsOverride)
            {
                return true;
            }

            if (this.Data.Group == null)
            {
                return false;
            }

            if (this.Data.Group.IsAdmin(session.GetUser().Id))
            {
                return true;
            }

            if (this.Data.Group.AdminOnlyDeco == 0)
            {
                if (this.Data.Group.IsMember(session.GetUser().Id))
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

            if (!roomUser.IsBot && roomUser.Client == null)
            {
                continue;
            }

            if (!roomUser.IsBot && roomUser.Client != null && roomUser.Client.GetUser() == null)
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
        if (this.Disposed)
        {
            return Task.CompletedTask;
        }

        try
        {
            var timeStarted = DateTime.Now;

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
        return Task.CompletedTask;
    }

    private void RpCycleHour()
    {
        if (!this.IsRoleplay)
        {
            return;
        }

        if (this.Data.OwnerName == "WibboParty")
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

                if (user.Client == null || user.Client.GetConnection() == null || user.Client.GetUser() == null)
                {
                    continue;
                }

                if (userMutedOnly && thisUser != null && user.Client.GetUser().MutedUsers.Contains(thisUser.UserId))
                {
                    continue;
                }

                if (thisUser != null && thisUser.Client != null && thisUser.Client.GetUser() != null && thisUser.Client.GetUser().IgnoreAll && thisUser != user)
                {
                    continue;
                }

                if (!userMutedOnly && thisUser == user)
                {
                    continue;
                }

                if (this.IngameChat && userNotIngameOnly && user.Team != TeamType.None)
                {
                    continue;
                }

                if (thisUser != null && this.Data.ChatMaxDistance > 0 && (Math.Abs(thisUser.X - user.X) > this.Data.ChatMaxDistance || Math.Abs(thisUser.Y - user.Y) > this.Data.ChatMaxDistance))
                {
                    continue;
                }

                user.Client.SendPacket(message);
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

                if (user.Client == null || user.Client.GetConnection() == null)
                {
                    continue;
                }

                if (usersWithRightsOnly && !this.CheckRights(user.Client))
                {
                    continue;
                }

                user.Client.SendPacket(message);
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

            if (user.Client == null || user.Client.GetConnection() == null)
            {
                continue;
            }

            user.Client.GetConnection().SendData(packet);
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

        this.Data.Tags.Clear();
        this.UsersWithRights.Clear();
        this._bans.Clear();
        this.ActiveTrades.Clear();

        foreach (var roomItem in this.GetRoomItemHandler().GetWallAndFloor)
        {
            roomItem.Destroy();
        }

        this.GetWiredHandler().Destroy();
        this.GetRoomItemHandler().Destroy();
        this.GetRoomUserManager().UpdateUserCount(0);
        this.GetRoomUserManager().Destroy();
        this.GetGameMap().Destroy();
    }

    public Dictionary<int, double> GetBans() => this._bans;

    public bool UserIsBanned(int id) => this._bans.ContainsKey(id);

    public void RemoveBan(int id) => this._bans.Remove(id);

    public void AddBan(int id, int time)
    {
        if (this._bans.ContainsKey(id))
        {
            return;
        }

        this._bans.Add(id, WibboEnvironment.GetUnixTimestamp() + time);
    }

    public bool HasBanExpired(int id) => !this.UserIsBanned(id) || this._bans[id] - WibboEnvironment.GetUnixTimestamp() <= 0.0;

    public Dictionary<int, double> GetMute() => this._mutes;

    public bool UserIsMuted(int id) => this._mutes.ContainsKey(id);

    public void RemoveMute(int id) => this._mutes.Remove(id);

    public void AddMute(int id, int time)
    {
        if (this._mutes.ContainsKey(id))
        {
            this.RemoveMute(id);
        }

        this._mutes.Add(id, WibboEnvironment.GetUnixTimestamp() + time);
    }

    public bool HasMuteExpired(int id) => !this.UserIsMuted(id) || this._mutes[id] - WibboEnvironment.GetUnixTimestamp() <= 0.0;

    public bool HasActiveTrade(RoomUser user)
    {
        if (user.IsBot)
        {
            return false;
        }
        else
        {
            return this.HasActiveTrade(user.Client.GetUser().Id);
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

        this.ActiveTrades.Add(new Trade(userOne.Client.GetUser().Id, userTwo.Client.GetUser().Id, this.Id));
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
