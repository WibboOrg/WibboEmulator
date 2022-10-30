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
using WibboEmulator.Games.Chats.Logs;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms.AI;
using WibboEmulator.Games.Rooms.Events;
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

public class Room
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly TimeSpan _maximumRunTimeInSec = TimeSpan.FromSeconds(1);

    private readonly TimeSpan _saveFurnitureTimer = TimeSpan.FromMinutes(2);
    private DateTime _saveFurnitureTimerLast = DateTime.Now;

    private readonly Dictionary<int, double> _bans;
    private readonly Dictionary<int, double> _mutes;

    public MoodlightData MoodlightData { get; set; }
    public RoomData RoomData { get; set; }
    public RoomRoleplay RoomRoleplay { get; }
    public GameMap GameMap { get; }
    public RoomItemHandling RoomItemHandling { get; }
    public RoomUserManager RoomUserManager { get; }
    public Soccer Soccer { get; }
    public TeamManager TeamManager { get; }
    public BattleBanzai BattleBanzai { get; }
    public Freeze Freeze { get; }
    public JankenManager JankenManager { get; }
    public GameManager GameManager { get; }
    public GameItemHandler GameItemHandler { get; }
    public WiredHandler WiredHandler { get; }
    public ProjectileManager ProjectileManager { get; }
    public ChatlogManager ChatlogManager { get; }

    public int Id => this.RoomData.Id;
    public int IsLagging { get; set; }
    public int IdleTime { get; set; }
    public bool Disposed { get; set; }
    public Task ProcessTask { get; set; }
    public bool IsRoleplay => this.RoomRoleplay != null;
    public List<int> UsersWithRights { get; set; }
    public DateTime LastTimerReset { get; set; }
    public List<Trade> ActiveTrades { get; set; }
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

    public int UserCount => this.RoomUserManager.GetRoomUserCount();

    public event EventHandler<UserSaysEventArgs> OnUserSays;
    public event EventHandler OnTrigger;
    public event EventHandler OnTriggerSelf;
    public event EventHandler OnUserCls;

    public Room(RoomData data)
    {
        var rpManager = WibboEnvironment.GetGame().GetRoleplayManager().GetRolePlay(data.OwnerId);
        if (rpManager != null)
        {
            this.RoomRoleplay = new RoomRoleplay(this);
        }

        this.Disposed = false;
        this._bans = new Dictionary<int, double>();
        this._mutes = new Dictionary<int, double>();
        this.ActiveTrades = new List<Trade>();
        this.RoomData = data;
        this.IdleTime = 0;
        this.RoomMuted = false;
        this.PushPullAllowed = true;
        this.IngameChat = false;

        this.GameMap = new GameMap(this);
        this.RoomItemHandling = new RoomItemHandling(this);
        this.RoomUserManager = new RoomUserManager(this);
        this.WiredHandler = new WiredHandler();
        this.ProjectileManager = new ProjectileManager(this);
        this.ChatlogManager = new ChatlogManager();
        this.GameItemHandler = new GameItemHandler(this);
        this.GameManager = new GameManager(this);
        this.JankenManager = new JankenManager(this);
        this.Freeze = new Freeze(this);
        this.BattleBanzai = new BattleBanzai(this);
        this.TeamManager = new TeamManager();
        this.Soccer = new Soccer(this);

        this.ChatlogManager.LoadRoomChatlogs(this.Id);

        this.RoomItemHandling.LoadFurniture();
        if (this.RoomData.OwnerName == WibboEnvironment.GetSettings().GetData<string>("autogame.owner"))
        {
            this.RoomItemHandling.LoadFurniture(WibboEnvironment.GetSettings().GetData<int>("autogame.deco.room.id"));
        }

        this.GameMap.GenerateMaps(true);
        this.LoadRights();
        this.LoadBots();
        this.LoadPets();
        this.LastTimerReset = DateTime.Now;
    }

    public bool AllowsShous(RoomUser user, string message)
    {
        var args = new UserSaysEventArgs(user, message);
        this.OnUserSays?.Invoke(null, args);

        return args.Result;
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

        var userGoal = this.RoomUserManager.GetUserForSquare(goalX, goalY);
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

    public void ClearTags() => this.RoomData.Tags.Clear();

    public void AddTagRange(List<string> tags) => this.RoomData.Tags.AddRange(tags);

    private void LoadBots()
    {
        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        var table = BotUserDao.GetOneByRoomId(dbClient, this.Id);
        if (table == null)
        {
            return;
        }

        foreach (DataRow row in table.Rows)
        {
            var roomBot = new RoomBot(Convert.ToInt32(row["id"]), Convert.ToInt32(row["user_id"]), Convert.ToInt32(row["room_id"]), this.IsRoleplay ? BotAIType.RoleplayBot : BotAIType.Generic, (string)row["walk_enabled"] == "1", (string)row["name"], (string)row["motto"], (string)row["gender"], (string)row["look"], Convert.ToInt32(row["x"]), Convert.ToInt32(row["y"]), Convert.ToInt32(row["z"]), Convert.ToInt32(row["rotation"]), (string)row["chat_enabled"] == "1", (string)row["chat_text"], Convert.ToInt32(row["chat_seconds"]), (string)row["is_dancing"] == "1", Convert.ToInt32(row["enable"]), Convert.ToInt32(row["handitem"]), Convert.ToInt32((string)row["status"]));
            var roomUser = this.RoomUserManager.DeployBot(roomBot, null);
            if (roomBot.IsDancing)
            {
                roomUser.DanceId = 3;
            }
        }
    }

    public void LoadPets()
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
            _ = this.RoomUserManager.DeployBot(new RoomBot(petData.PetId, petData.OwnerId, this.Id, BotAIType.Pet, true, petData.Name, "", "", petData.Look, petData.X, petData.Y, petData.Z, 0, false, "", 0, false, 0, 0, 0), petData);
        }
    }

    public void OnUserSay(RoomUser user, string message, bool shout)
    {
        foreach (var roomUser in this.RoomUserManager.GetPets().ToList())
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

    public bool CheckRights(GameClient session, bool requireOwnership = false)
    {
        if (session == null || session.User == null)
        {
            return false;
        }

        if (session.User.Username == this.RoomData.OwnerName || session.User.HasPermission("owner_all_rooms"))
        {
            return true;
        }

        if (!requireOwnership)
        {
            if (session.User.HasPermission("room_rights") || this.UsersWithRights.Contains(session.User.Id))
            {
                return true;
            }

            if (this.RoomData.AllowRightsOverride)
            {
                return true;
            }

            if (this.RoomData.Group == null)
            {
                return false;
            }

            if (this.RoomData.Group.IsAdmin(session.User.Id))
            {
                return true;
            }

            if (this.RoomData.Group.AdminOnlyDeco == 0)
            {
                if (this.RoomData.Group.IsMember(session.User.Id))
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

        foreach (var roomUser in this.RoomUserManager.GetUserList().ToList())
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

            if (!roomUser.IsBot && roomUser.Client != null && roomUser.Client.User == null)
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

        packetList.Add(new UserUpdateComposer(this.RoomUserManager.GetUserList().ToList()));
        packetList.Add(new ObjectsComposer(this.RoomItemHandling.GetFloor.ToArray(), this));
        packetList.Add(new ObjectsComposer(this.RoomItemHandling.GetTempItems.ToArray(), this));
        packetList.Add(new ItemWallComposer(this.RoomItemHandling.GetWall.ToArray(), this));

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

            this.RoomUserManager.OnCycle(ref idleCount);
            this.RoomItemHandling.OnCycle();
            this.RoomRoleplay?.OnCycle();
            this.ProjectileManager.OnCycle();
            this.GameItemHandler.OnCycle();
            this.WiredHandler.OnCycle();
            this.JankenManager.OnCycle();

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
                this.RoomUserManager.SerializeStatusUpdates();
            }

            if (timeStarted > this._saveFurnitureTimerLast + this._saveFurnitureTimer)
            {
                this._saveFurnitureTimerLast = timeStarted;

                this.RoomItemHandling.SaveFurniture();
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
            ExceptionLogger.LogThreadException(ex.ToString(), "Room cycle task for room " + this.Id);
            WibboEnvironment.GetGame().GetRoomManager().UnloadRoom(this);
        }

        return Task.CompletedTask;
    }

    public void SendPacketOnChat(IServerPacket message, RoomUser thisUser = null, bool userMutedOnly = false, bool userNotIngameOnly = false)
    {
        if (message == null)
        {
            return;
        }

        if (this == null || this.RoomUserManager == null)
        {
            return;
        }

        var users = this.RoomUserManager.GetUserList().ToList();
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

            if (user.Client == null || user.Client.Connection == null || user.Client.User == null)
            {
                continue;
            }

            if (userMutedOnly && thisUser != null && user.Client.User.MutedUsers.Contains(thisUser.UserId))
            {
                continue;
            }

            if (thisUser != null && thisUser.Client != null && thisUser.Client.User != null && thisUser.Client.User.IgnoreAll && thisUser != user)
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

            if (thisUser != null && this.RoomData.ChatMaxDistance > 0 && (Math.Abs(thisUser.X - user.X) > this.RoomData.ChatMaxDistance || Math.Abs(thisUser.Y - user.Y) > this.RoomData.ChatMaxDistance))
            {
                continue;
            }

            user.Client.SendPacket(message);
        }
    }

    public void SendPacket(IServerPacket message, bool usersWithRightsOnly = false)
    {
        if (message == null)
        {
            return;
        }

        var users = this.RoomUserManager.GetUserList().ToList();
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

            if (user.Client == null || user.Client.Connection == null)
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
        foreach (var user in this.RoomUserManager.GetUserList().ToList())
        {
            if (user == null || user.IsBot)
            {
                continue;
            }

            if (user.Client == null || user.Client.Connection == null)
            {
                continue;
            }

            user.Client.Connection.SendData(packet);
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

        this.RoomItemHandling.SaveFurniture();

        this.RoomData.Tags.Clear();
        this.UsersWithRights.Clear();
        this._bans.Clear();
        this.ActiveTrades.Clear();

        foreach (var roomItem in this.RoomItemHandling.GetWallAndFloor)
        {
            roomItem.Destroy();
        }

        this.WiredHandler.Destroy();
        this.RoomItemHandling.Destroy();
        this.RoomUserManager.UpdateUserCount(0);
        this.RoomUserManager.Destroy();
        this.GameMap.Destroy();
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
        if (user.Client == null)
        {
            return false;
        }
        else
        {
            return this.HasActiveTrade(user.Client.User.Id);
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

        if (userOne.Client == null || userTwo.Client == null || userOne.IsTrading || userTwo.IsTrading || this.HasActiveTrade(userOne) || this.HasActiveTrade(userTwo))
        {
            return;
        }

        this.ActiveTrades.Add(new Trade(userOne.Client.User.Id, userTwo.Client.User.Id, this.Id));
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
