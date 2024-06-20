namespace WibboEmulator.Games.Rooms;
using System.Data;
using WibboEmulator.Communication.Interfaces;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Avatar;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Session;
using WibboEmulator.Core;
using WibboEmulator.Core.Settings;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Bot;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.Catalogs.Utilities;
using WibboEmulator.Games.Chats.Logs;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Roleplays;
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

public class Room : IDisposable
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly TimeSpan _maximumRunTimeInSec = TimeSpan.FromSeconds(1);

    private readonly TimeSpan _saveRoomTimer = TimeSpan.FromMinutes(2);
    private DateTime _saveRoomTimerLast = DateTime.Now;

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
    public bool IsGameMode { get; set; }
    public bool CloseFullRoom { get; set; }
    public bool OldFoot { get; set; }
    public bool IngameChat { get; set; }

    //Question
    public int VotedYesCount { get; set; }
    public int VotedNoCount { get; set; }

    public int UserCount => this.RoomUserManager.RoomUserCount;

    public event EventHandler<UserSaysEventArgs> OnUserSays;
    public event EventHandler<UserSaysEventArgs> OnCommandTarget;
    public event EventHandler<UserSaysEventArgs> OnCommandSelf;
    public event EventHandler<UserTargetEventArgs> OnUserClick;
    public event EventHandler<UserTargetEventArgs> OnUserClickSelf;
    public event EventHandler OnUserCls;
    public event EventHandler OnUserClsSelf;

    public Room(RoomData data)
    {
        var rpManager = RoleplayManager.GetRolePlay(data.OwnerId);
        if (rpManager != null)
        {
            this.RoomRoleplay = new RoomRoleplay(this);
        }

        this.Disposed = false;
        this.Bans = [];
        this.Mutes = [];
        this.ActiveTrades = [];
        this.RoomData = data;
        this.IdleTime = 0;
        this.RoomMuted = false;
        this.PushPullAllowed = true;
        this.IngameChat = false;

        this.GameMap = new GameMap(this);
        this.RoomItemHandling = new RoomItemHandling(this);
        this.RoomUserManager = new RoomUserManager(this);
        this.WiredHandler = new WiredHandler(this);
        this.ProjectileManager = new ProjectileManager(this);
        this.ChatlogManager = new ChatlogManager();
        this.GameItemHandler = new GameItemHandler(this);
        this.GameManager = new GameManager(this);
        this.JankenManager = new JankenManager(this);
        this.Freeze = new Freeze(this);
        this.BattleBanzai = new BattleBanzai(this);
        this.TeamManager = new TeamManager();
        this.Soccer = new Soccer(this);

        using var dbClient = DatabaseManager.Connection;

        this.ChatlogManager.LoadRoomChatlogs(this.Id, dbClient);

        this.RoomItemHandling.LoadFurniture(dbClient);
        if (this.RoomData.OwnerName == SettingsManager.GetData<string>("autogame.owner"))
        {
            this.RoomItemHandling.LoadFurniture(dbClient, SettingsManager.GetData<int>("autogame.deco.room.id"));
        }

        this.GameMap.GenerateMaps(true);
        this.LoadRights(dbClient);
        this.LoadBots(dbClient);
        this.LoadPets(dbClient);

        this.WiredHandler.SecurityEnabled = data.WiredSecurity;

        this.LastTimerReset = DateTime.Now;
    }

    public void UserClick(RoomUser user, RoomUser userTarget)
    {
        var args = new UserTargetEventArgs(userTarget);
        this.OnUserClick?.Invoke(user, args);
        this.OnUserClickSelf?.Invoke(user, args);
    }

    public bool AllowsShous(RoomUser user, string message)
    {
        var args = new UserSaysEventArgs(user, message);
        this.OnUserSays?.Invoke(null, args);

        return args.Result;
    }

    public void CollisionUser(RoomUser user)
    {
        if (this.OnUserCls == null && this.OnUserClsSelf == null)
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

        this.OnUserCls?.Invoke(userGoal, new());
        this.OnUserClsSelf?.Invoke(user, new());
    }

    public bool OnCommand(RoomUser user, string message)
    {
        var args = new UserSaysEventArgs(user, message);

        this.OnCommandTarget?.Invoke(null, args);
        this.OnCommandSelf?.Invoke(null, args);

        return args.Result;
    }

    public void ClearTags() => this.RoomData.Tags.Clear();

    public void AddTagRange(List<string> tags) => this.RoomData.Tags.AddRange(tags);

    private void LoadBots(IDbConnection dbClient)
    {
        var botUserList = BotUserDao.GetAllByRoomId(dbClient, this.Id);
        if (botUserList.Count == 0)
        {
            return;
        }

        foreach (var botUser in botUserList)
        {
            var roomBot = new RoomBot(botUser.Id, botUser.UserId, botUser.RoomId, BotUtility.GetAIFromString(botUser.AiType), botUser.WalkEnabled,
            botUser.Name, botUser.Motto, botUser.Gender, botUser.Look, botUser.X, botUser.Y, botUser.Z, botUser.Rotation, botUser.ChatEnabled,
            botUser.ChatText, botUser.ChatSeconds, botUser.IsDancing, botUser.Enable, botUser.HandItem, botUser.Status);
            _ = this.RoomUserManager.DeployBot(roomBot, null);
        }
    }

    public void LoadPets(IDbConnection dbClient)
    {
        var botPetList = BotPetDao.GetAllByRoomId(dbClient, this.Id);
        if (botPetList.Count == 0)
        {
            return;
        }

        foreach (var botPet in botPetList)
        {
            var petData = new Pet(botPet.Id, botPet.UserId, botPet.RoomId, botPet.Name, botPet.Type, botPet.Race, botPet.Color, botPet.Experience, botPet.Energy, botPet.Nutrition, botPet.Respect, botPet.CreateStamp, botPet.X, botPet.Y, botPet.Z, botPet.HaveSaddle, botPet.HairDye, botPet.PetHair, botPet.AnyoneRide);
            _ = this.RoomUserManager.DeployBot(new RoomBot(petData.PetId, petData.OwnerId, this.Id, BotAIType.Pet, true, petData.Name, "", "", petData.Look, petData.X, petData.Y, petData.Z, 0, false, "", 0, false, 0, 0, 0), petData);
        }
    }

    public void OnUserSay(RoomUser user, string message, bool shout)
    {
        foreach (var roomUser in this.RoomUserManager.Pets.ToList())
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

        foreach (var roomUser in this.RoomUserManager.Bots.ToList())
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

    public void LoadRights(IDbConnection dbClient)
    {
        this.UsersWithRights = [];

        var roomRightIdList = RoomRightDao.GetAllByRoomId(dbClient, this.RoomData.Id);

        if (roomRightIdList.Count == 0)
        {
            return;
        }

        foreach (var roomRightId in roomRightIdList)
        {
            this.UsersWithRights.Add(roomRightId);
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

            if (!this.RoomData.Group.AdminOnlyDeco)
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

        foreach (var roomUser in this.RoomUserManager.UserList.ToList())
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

        packetList.Add(new UserUpdateComposer([.. this.RoomUserManager.UserList]));
        packetList.Add(new ObjectsComposer(this.RoomItemHandling.FloorItems.ToArray(), this));
        packetList.Add(new ObjectsComposer(this.RoomItemHandling.TempItems.ToArray(), this));
        packetList.Add(new ItemWallComposer([.. this.RoomItemHandling.WallItems], this));

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
                RoomManager.UnloadRoom(this);

                return Task.CompletedTask;
            }
            else
            {
                this.RoomUserManager.SerializeStatusUpdates();
            }

            if (timeStarted > this._saveRoomTimerLast + this._saveRoomTimer)
            {
                this._saveRoomTimerLast = timeStarted;

                using var dbClient = DatabaseManager.Connection;
                RoomDao.UpdateUsersNow(dbClient, this.Id, this.UserCount);

                this.RoomItemHandling.SaveFurniture(dbClient);
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
            RoomManager.UnloadRoom(this);
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

        var users = this.RoomUserManager.UserList.ToList();
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

            if (user.Client == null || user.Client.User == null)
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

        var users = this.RoomUserManager.UserList.ToList();
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

            if (user.Client == null)
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

        this.BroadcastPacket(messages.Bytes);
    }

    public void BroadcastPacket(byte[] packet)
    {
        foreach (var user in this.RoomUserManager.UserList.ToList())
        {
            if (user == null || user.IsBot)
            {
                continue;
            }

            if (user.Client == null)
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

        using (var dbClient = DatabaseManager.Connection)
        {
            RoomDao.UpdateUsersNow(dbClient, this.Id, 0);

            this.RoomItemHandling.SaveFurniture(dbClient);
        }

        this.RoomData.Tags.Clear();
        this.UsersWithRights.Clear();
        this.Bans.Clear();
        this.ActiveTrades.Clear();

        foreach (var roomItem in this.RoomItemHandling.WallAndFloorItems)
        {
            roomItem.Destroy();
        }

        this.WiredHandler.Destroy();
        this.RoomItemHandling.Destroy();
        this.RoomUserManager.UpdateUserCount(0);
        this.RoomUserManager.Destroy();
        this.GameMap.Destroy();

        GC.SuppressFinalize(this);
    }

    public Dictionary<int, double> Bans { get; }

    public bool UserIsBanned(int id) => this.Bans.ContainsKey(id);

    public void RemoveBan(int id) => this.Bans.Remove(id);

    public void AddBan(int id, int time)
    {
        if (this.Bans.ContainsKey(id))
        {
            return;
        }

        this.Bans.Add(id, WibboEnvironment.GetUnixTimestamp() + time);
    }

    public bool HasBanExpired(int id) => !this.UserIsBanned(id) || this.Bans[id] - WibboEnvironment.GetUnixTimestamp() <= 0.0;

    public Dictionary<int, double> Mutes { get; }

    public bool UserIsMuted(int id) => this.Mutes.ContainsKey(id);

    public void RemoveMute(int id) => this.Mutes.Remove(id);

    public void AddMute(int id, int time)
    {
        if (this.Mutes.ContainsKey(id))
        {
            this.RemoveMute(id);
        }

        this.Mutes.Add(id, WibboEnvironment.GetUnixTimestamp() + time);
    }

    public bool HasMuteExpired(int id) => !this.UserIsMuted(id) || this.Mutes[id] - WibboEnvironment.GetUnixTimestamp() <= 0.0;

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
