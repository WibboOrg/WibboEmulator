namespace WibboEmulator.Games.Rooms;
using System.Collections.Concurrent;
using System.Drawing;
using WibboEmulator.Communication.Packets.Outgoing.GameCenter;
using WibboEmulator.Communication.Packets.Outgoing.Handshake;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Avatar;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Permissions;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Session;
using WibboEmulator.Core;
using WibboEmulator.Database.Daos.Bot;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Quests;
using WibboEmulator.Games.Roleplay.Enemy;
using WibboEmulator.Games.Rooms.AI;
using WibboEmulator.Games.Rooms.Games.Teams;
using WibboEmulator.Games.Rooms.Map;
using WibboEmulator.Games.Rooms.Map.Movement;
using WibboEmulator.Games.Rooms.PathFinding;

public class RoomUserManager
{
    private readonly Room _room;
    private readonly ConcurrentDictionary<string, RoomUser> _usersByUsername;
    private readonly ConcurrentDictionary<int, RoomUser> _usersByUserID;

    private readonly ConcurrentDictionary<int, RoomUser> _users;
    private readonly ConcurrentDictionary<int, RoomUser> _pets;
    private readonly ConcurrentDictionary<int, RoomUser> _bots;

    private readonly List<int> _usersRank;

    private int _primaryPrivateUserID;
    public int BotPetCount => this._pets.Count + this._bots.Count;

    public event EventHandler OnUserEnter;

    public RoomUserManager(Room room)
    {
        this._room = room;
        this._users = new ConcurrentDictionary<int, RoomUser>();
        this._pets = new ConcurrentDictionary<int, RoomUser>();
        this._bots = new ConcurrentDictionary<int, RoomUser>();
        this._usersByUsername = new ConcurrentDictionary<string, RoomUser>();
        this._usersByUserID = new ConcurrentDictionary<int, RoomUser>();
        this._usersRank = new List<int>();
        this._primaryPrivateUserID = 1;
    }

    public void UserEnter(RoomUser thisUser) => this.OnUserEnter?.Invoke(thisUser, new());

    public int GetRoomUserCount() => this._room.RoomData.UsersNow;

    public RoomUser DeploySuperBot(RoomBot bot)
    {
        var key = this._primaryPrivateUserID++;
        var roomUser = new RoomUser(0, this._room.Id, key, this._room);

        bot.Id = -key;

        _ = this._users.TryAdd(key, roomUser);

        roomUser.SetPos(bot.X, bot.Y, bot.Z);
        roomUser.SetRot(bot.Rot, false);

        roomUser.BotData = bot;
        roomUser.BotAI = bot.GenerateBotAI(roomUser.VirtualId);

        roomUser.BotAI.Init(bot.Id, roomUser, this._room);

        roomUser.SetStatus("flatctrl", "4");
        this.UpdateUserStatus(roomUser, false);
        roomUser.UpdateNeeded = true;

        this._room.SendPacket(new UsersComposer(roomUser));

        roomUser.BotAI.OnSelfEnterRoom();

        if (this._bots.ContainsKey(roomUser.BotData.Id))
        {
            this._bots[roomUser.BotData.Id] = roomUser;
        }
        else
        {
            _ = this._bots.TryAdd(roomUser.BotData.Id, roomUser);
        }

        return roomUser;
    }

    public bool UpdateClientUsername(RoomUser user, string oldUsername, string newUsername)
    {
        if (!this._usersByUsername.ContainsKey(oldUsername.ToLower()))
        {
            return false;
        }

        _ = this._usersByUsername.TryRemove(oldUsername.ToLower(), out _);
        _ = this._usersByUsername.TryAdd(newUsername.ToLower(), user);
        return true;
    }

    public RoomUser DeployBot(RoomBot bot, Pet petData)
    {
        var key = this._primaryPrivateUserID++;
        var roomUser = new RoomUser(0, this._room.Id, key, this._room);

        _ = this._users.TryAdd(key, roomUser);

        roomUser.SetPos(bot.X, bot.Y, bot.Z);
        roomUser.SetRot(bot.Rot, false);

        roomUser.BotData = bot;

        if (this._room.IsRoleplay)
        {
            RPEnemy enemy;
            if (bot.IsPet)
            {
                enemy = WibboEnvironment.GetGame().GetRoleplayManager().GetEnemyManager().GetEnemyPet(bot.Id);
            }
            else
            {
                enemy = WibboEnvironment.GetGame().GetRoleplayManager().GetEnemyManager().GetEnemyBot(bot.Id);
            }

            if (enemy != null)
            {
                roomUser.BotData.RoleBot = new RoleBot(enemy);
                if (bot.IsPet)
                {
                    roomUser.BotData.AiType = BotAIType.RoleplayPet;
                }
                else
                {
                    roomUser.BotData.AiType = BotAIType.RoleplayBot;
                }
            }
        }

        roomUser.BotAI = bot.GenerateBotAI(roomUser.VirtualId);

        if (roomUser.IsPet)
        {
            roomUser.BotAI.Init(bot.Id, roomUser, this._room);
            roomUser.PetData = petData;
            roomUser.PetData.VirtualId = roomUser.VirtualId;
        }
        else
        {
            roomUser.BotAI.Init(bot.Id, roomUser, this._room);
        }

        roomUser.SetStatus("flatctrl", "4");

        if (bot.Status == 1)
        {
            roomUser.SetStatus("sit", "0.5");
            roomUser.IsSit = true;
        }

        if (bot.Status == 2)
        {
            roomUser.SetStatus("lay", "0.7");
            roomUser.IsLay = true;
        }

        this.UpdateUserStatus(roomUser, false);
        roomUser.UpdateNeeded = true;

        if (bot.IsDancing)
        {
            roomUser.DanceId = 3;
            this._room.SendPacket(new DanceComposer(roomUser.VirtualId, 3));
        }

        if (bot.Enable > 0)
        {
            roomUser.ApplyEffect(bot.Enable);
        }

        if (bot.Handitem > 0)
        {
            roomUser.CarryItem(bot.Handitem, true);
        }

        this._room.SendPacket(new UsersComposer(roomUser));

        roomUser.BotAI.OnSelfEnterRoom();
        if (roomUser.IsPet)
        {
            if (this._pets.ContainsKey(roomUser.PetData.PetId))
            {
                this._pets[roomUser.PetData.PetId] = roomUser;
            }
            else
            {
                _ = this._pets.TryAdd(roomUser.PetData.PetId, roomUser);
            }
        }
        else if (this._bots.ContainsKey(roomUser.BotData.Id))
        {
            this._bots[roomUser.BotData.Id] = roomUser;
        }
        else
        {
            _ = this._bots.TryAdd(roomUser.BotData.Id, roomUser);
        }

        return roomUser;
    }

    public void RemoveBot(int virtualId, bool kicked)
    {
        var roomUserByVirtualId = this.GetRoomUserByVirtualId(virtualId);
        if (roomUserByVirtualId == null || !roomUserByVirtualId.IsBot)
        {
            return;
        }

        if (roomUserByVirtualId.IsPet)
        {
            _ = this._pets.TryRemove(roomUserByVirtualId.PetData.PetId, out _);
        }
        else
        {
            _ = this._bots.TryRemove(roomUserByVirtualId.BotData.Id, out _);
        }

        roomUserByVirtualId.BotAI.OnSelfLeaveRoom(kicked);

        this._room.SendPacket(new UserRemoveComposer(roomUserByVirtualId.VirtualId));

        this._room.GetGameMap().RemoveTakingSquare(roomUserByVirtualId.SetX, roomUserByVirtualId.SetY);
        this._room.GetGameMap().RemoveUserFromMap(roomUserByVirtualId, new Point(roomUserByVirtualId.X, roomUserByVirtualId.Y));

        _ = this._users.TryRemove(roomUserByVirtualId.VirtualId, out _);

    }

    private void UpdateUserEffect(RoomUser user, int x, int y)
    {
        if (user == null)
        {
            return;
        }

        if (user.IsPet)
        {
            return;
        }

        if (!this._room.GetGameMap().ValidTile(x, y))
        {
            return;
        }

        var pByte = this._room.GetGameMap().EffectMap[x, y];
        if (pByte > 0)
        {
            var itemEffectType = ByteToItemEffectType.Parse(pByte);
            if (itemEffectType == user.CurrentItemEffect)
            {
                return;
            }

            switch (itemEffectType)
            {
                case ItemEffectType.NONE:
                    user.ApplyEffect(0);
                    user.CurrentItemEffect = itemEffectType;
                    break;
                case ItemEffectType.SWIM:
                    user.ApplyEffect(29);
                    if (user.GetClient() != null)
                    {
                        WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(user.GetClient(), QuestType.EXPLORE_FIND_ITEM, 1948);
                    }

                    user.CurrentItemEffect = itemEffectType;
                    break;
                case ItemEffectType.SWIMLOW:
                    user.ApplyEffect(30);
                    user.CurrentItemEffect = itemEffectType;
                    break;
                case ItemEffectType.SWIMHALLOWEEN:
                    user.ApplyEffect(37);
                    user.CurrentItemEffect = itemEffectType;
                    break;
                case ItemEffectType.ICESKATES:
                    if (user.GetClient() != null)
                    {
                        if (user.GetClient().GetUser().Gender == "M")
                        {
                            user.ApplyEffect(38);
                        }
                        else
                        {
                            user.ApplyEffect(39);
                        }
                    }
                    else
                    {
                        user.ApplyEffect(38);
                    }

                    user.CurrentItemEffect = ItemEffectType.ICESKATES;
                    if (user.GetClient() != null)
                    {
                        WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(user.GetClient(), QuestType.EXPLORE_FIND_ITEM, 1413);
                    }

                    break;
                case ItemEffectType.NORMALSKATES:
                    if (user.GetClient() != null)
                    {
                        if (user.GetClient().GetUser().Gender == "M")
                        {
                            user.ApplyEffect(55);
                        }
                        else
                        {
                            user.ApplyEffect(56);
                        }
                    }
                    else
                    {
                        user.ApplyEffect(55);
                    }

                    user.CurrentItemEffect = itemEffectType;
                    if (user.GetClient() != null)
                    {
                        WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(user.GetClient(), QuestType.EXPLORE_FIND_ITEM, 2199);
                    }

                    break;
                case ItemEffectType.TRAMPOLINE:
                    user.ApplyEffect(193);
                    user.CurrentItemEffect = itemEffectType;
                    break;
                case ItemEffectType.TREADMILL:
                    user.ApplyEffect(194);
                    user.CurrentItemEffect = itemEffectType;
                    break;
                case ItemEffectType.CROSSTRAINER:
                    user.ApplyEffect(195);
                    user.CurrentItemEffect = itemEffectType;
                    break;

            }
        }
        else
        {
            if (user.CurrentItemEffect == ItemEffectType.NONE || pByte != 0)
            {
                return;
            }

            user.ApplyEffect(0);
            user.CurrentItemEffect = ItemEffectType.NONE;
        }
    }

    public List<RoomUser> GetUsersForSquare(int x, int y) => this._room.GetGameMap().GetRoomUsers(new Point(x, y)).OrderBy(u => u.IsBot).ToList();

    public RoomUser GetUserForSquare(int x, int y) => Enumerable.FirstOrDefault(this._room.GetGameMap().GetRoomUsers(new Point(x, y)).OrderBy(u => u.IsBot));

    public RoomUser GetUserForSquareNotBot(int x, int y) => this._room.GetGameMap().GetRoomUsers(new Point(x, y)).FirstOrDefault(u => !u.IsBot);

    public bool AddAvatarToRoom(GameClient session)
    {
        if (this._room == null)
        {
            return false;
        }

        if (session == null || session.GetUser() == null)
        {
            return false;
        }

        var personalID = this._primaryPrivateUserID++;

        var user = new RoomUser(session.GetUser().Id, this._room.Id, personalID, this._room)
        {
            IsSpectator = session.GetUser().SpectatorMode
        };

        if (!this._users.TryAdd(personalID, user))
        {
            return false;
        }

        if (session.GetUser().Rank > 5 && !this._usersRank.Contains(user.UserId))
        {
            this._usersRank.Add(user.UserId);
        }

        session.GetUser().CurrentRoomId = this._room.Id;
        session.GetUser().LoadingRoomId = 0;

        var username = session.GetUser().Username;
        var userId = session.GetUser().Id;

        if (this._usersByUsername.ContainsKey(username.ToLower()))
        {
            _ = this._usersByUsername.TryRemove(username.ToLower(), out _);
        }

        if (this._usersByUserID.ContainsKey(userId))
        {
            _ = this._usersByUserID.TryRemove(userId, out _);
        }

        _ = this._usersByUsername.TryAdd(username.ToLower(), user);
        _ = this._usersByUserID.TryAdd(userId, user);

        var roomModel = this._room.GetGameMap().Model;
        if (roomModel == null)
        {
            return false;
        }

        user.SetPos(roomModel.DoorX, roomModel.DoorY, roomModel.DoorZ);
        user.SetRot(roomModel.DoorOrientation, false);

        if (session.GetUser().IsTeleporting)
        {
            var roomItem = this._room.GetRoomItemHandler().GetItem(user.GetClient().GetUser().TeleporterId);
            if (roomItem != null)
            {
                Gamemap.TeleportToItem(user, roomItem);

                roomItem.InteractingUser2 = session.GetUser().Id;
                roomItem.ReqUpdate(1);
            }
        }

        if (user.GetClient() != null && user.GetClient().GetUser() != null)
        {
            user.GetClient().GetUser().IsTeleporting = false;
            user.GetClient().GetUser().TeleporterId = 0;
            user.GetClient().GetUser().TeleportingRoomID = 0;
        }

        if (!user.IsSpectator)
        {
            this._room.SendPacket(new UsersComposer(user));
        }

        if (user.IsSpectator)
        {
            var roomUserByRank = this._room.GetRoomUserManager().GetStaffRoomUser();
            if (roomUserByRank.Count > 0)
            {
                foreach (var staffUser in roomUserByRank)
                {
                    if (staffUser != null && staffUser.GetClient() != null && staffUser.GetClient().GetUser() != null && staffUser.GetClient().GetUser().HasPermission("perm_show_invisible"))
                    {
                        staffUser.SendWhisperChat(user.GetUsername() + " est entré dans l'appart en mode invisible !", true);
                    }
                }
            }
        }

        if (session.GetUser().HasPermission("perm_owner_all_rooms"))
        {
            user.SetStatus("flatctrl", "5");
            session.SendPacket(new YouAreOwnerComposer());
            session.SendPacket(new YouAreControllerComposer(5));
        }
        else if (this._room.CheckRights(session, true))
        {
            user.SetStatus("flatctrl", "4");
            session.SendPacket(new YouAreOwnerComposer());
            session.SendPacket(new YouAreControllerComposer(4));
        }
        else if (this._room.CheckRights(session))
        {
            user.SetStatus("flatctrl", "1");
            session.SendPacket(new YouAreControllerComposer(1));
        }
        else
        {
            user.RemoveStatus("flatctrl");
            session.SendPacket(new YouAreNotControllerComposer());
        }

        if (!user.IsBot)
        {
            if (session.GetUser().GetBadgeComponent().HasBadgeSlot("ADM")) // STAFF
            {
                user.CurrentEffect = 540;
            }
            else if (session.GetUser().GetBadgeComponent().HasBadgeSlot("PRWRD1")) // PROWIRED
            {
                user.CurrentEffect = 580;
            }
            else if (session.GetUser().GetBadgeComponent().HasBadgeSlot("GPHWIB")) // GRAPHISTE
            {
                user.CurrentEffect = 557;
            }
            else if (session.GetUser().GetBadgeComponent().HasBadgeSlot("wibbo.helpeur")) // HELPEUR
            {
                user.CurrentEffect = 544;
            }
            else if (session.GetUser().GetBadgeComponent().HasBadgeSlot("WIBARC")) // ARCHI
            {
                user.CurrentEffect = 546;
            }
            else if (session.GetUser().GetBadgeComponent().HasBadgeSlot("CRPOFFI")) // CROUPIER
            {
                user.CurrentEffect = 570;
            }
            else if (session.GetUser().GetBadgeComponent().HasBadgeSlot("ZEERSWS")) // WIBBOSTATIONORIGINERADIO
            {
                user.CurrentEffect = 552;
            }
            else if (session.GetUser().GetBadgeComponent().HasBadgeSlot("WBASSO")) // ASSOCIER
            {
                user.CurrentEffect = 576;
            }
            else if (session.GetUser().GetBadgeComponent().HasBadgeSlot("WIBBOCOM")) // AGENT DE COMMUNICATION
            {
                user.CurrentEffect = 581;
            }

            if (user.CurrentEffect > 0)
            {
                this._room.SendPacket(new AvatarEffectComposer(user.VirtualId, user.CurrentEffect));
            }
        }

        user.UpdateNeeded = true;

        foreach (var bot in this._bots.Values.ToList())
        {
            if (bot == null || bot.BotAI == null)
            {
                continue;
            }

            bot.BotAI.OnUserEnterRoom(user);
        }

        if (!user.IsBot && this._room.RoomData.OwnerName != user.GetClient().GetUser().Username)
        {
            WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(user.GetClient(), QuestType.SOCIAL_VISIT, 0);
        }

        if (!user.IsBot)
        {
            if (session.GetUser().RolePlayId > 0 && this._room.RoomData.OwnerId != session.GetUser().RolePlayId)
            {
                var rpManager = WibboEnvironment.GetGame().GetRoleplayManager().GetRolePlay(session.GetUser().RolePlayId);
                if (rpManager != null)
                {
                    var rp = rpManager.GetPlayer(session.GetUser().Id);
                    if (rp != null)
                    {
                        rpManager.RemovePlayer(session.GetUser().Id);
                    }
                }
                session.GetUser().RolePlayId = 0;
            }

            if (this._room.IsRoleplay && this._room.RoomData.OwnerId != session.GetUser().RolePlayId)
            {
                var rpManager = WibboEnvironment.GetGame().GetRoleplayManager().GetRolePlay(this._room.RoomData.OwnerId);
                if (rpManager != null)
                {
                    var rp = rpManager.GetPlayer(session.GetUser().Id);
                    if (rp == null)
                    {
                        rpManager.AddPlayer(session.GetUser().Id);
                    }
                }

                session.GetUser().RolePlayId = this._room.RoomData.OwnerId;
            }
        }


        user.InGame = this._room.IsRoleplay;

        return true;
    }

    public void RemoveUserFromRoom(GameClient session, bool notifyClient, bool notifyKick)
    {
        try
        {
            if (session == null)
            {
                return;
            }

            if (session.GetUser() == null)
            {
                return;
            }

            if (notifyClient)
            {
                if (notifyKick)
                {
                    session.SendPacket(new GenericErrorComposer(4008));
                }
                session.SendPacket(new CloseConnectionComposer());
            }

            var user = this.GetRoomUserByUserId(session.GetUser().Id);
            if (user == null)
            {
                return;
            }

            if (this._usersRank.Contains(user.UserId))
            {
                _ = this._usersRank.Remove(user.UserId);
            }

            if (user.Team != TeamType.NONE)
            {
                this._room.GetTeamManager().OnUserLeave(user);
                this._room.GetGameManager().UpdateGatesTeamCounts();

                session.SendPacket(new IsPlayingComposer(false));
            }

            if (this._room.GotJanken())
            {
                this._room.GetJanken().RemovePlayer(user);
            }

            if (user.RidingHorse)
            {
                user.RidingHorse = false;
                var roomUserByVirtualId = this.GetRoomUserByVirtualId(user.HorseID);
                if (roomUserByVirtualId != null)
                {
                    roomUserByVirtualId.RidingHorse = false;
                    roomUserByVirtualId.HorseID = 0;
                }
            }

            if (user.IsSit || user.IsLay)
            {
                user.IsSit = false;
                user.IsLay = false;
            }

            if (this._room.HasActiveTrade(session.GetUser().Id))
            {
                this._room.TryStopTrade(session.GetUser().Id);
            }

            if (user.Roleplayer != null)
            {
                WibboEnvironment.GetGame().GetRoleplayManager().GetTrocManager().RemoveTrade(user.Roleplayer.TradeId);
            }

            if (user.IsSpectator)
            {
                var roomUserByRank = this._room.GetRoomUserManager().GetStaffRoomUser();
                if (roomUserByRank.Count > 0)
                {
                    foreach (var staffUser in roomUserByRank)
                    {
                        if (staffUser != null && staffUser.GetClient() != null && staffUser.GetClient().GetUser() != null && staffUser.GetClient().GetUser().HasPermission("perm_show_invisible"))
                        {
                            staffUser.SendWhisperChat(user.GetUsername() + " était en mode invisible. Il vient de partir de l'appartement.", true);
                        }
                    }
                }
            }

            session.GetUser().CurrentRoomId = 0;
            session.GetUser().LoadingRoomId = 0;

            session.GetUser().ForceUse = -1;

            this.RemoveRoomUser(user);

            user.Freeze = true;
            user.FreezeEndCounter = 0;
            user.Dispose();

            _ = this._usersByUserID.TryRemove(user.UserId, out user);
            _ = this._usersByUsername.TryRemove(session.GetUser().Username.ToLower(), out user);
        }
        catch (Exception ex)
        {
            ExceptionLogger.LogCriticalException("Error during removing user (" + session.ConnectionID + ") from room:" + ex.ToString());
        }
    }

    private void RemoveRoomUser(RoomUser user)
    {
        this._room.GetGameMap().RemoveTakingSquare(user.SetX, user.SetY);
        this._room.GetGameMap().RemoveUserFromMap(user, new Point(user.X, user.Y));

        this._room.SendPacket(new UserRemoveComposer(user.VirtualId));

        _ = this._users.TryRemove(user.VirtualId, out _);
    }

    public void UpdateUserCount(int count)
    {
        if (this._room.RoomData.UsersNow == count)
        {
            return;
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            RoomDao.UpdateUsersNow(dbClient, this._room.Id, count);
        }

        this._room.RoomData.UsersNow = count;
    }

    public RoomUser GetRoomUserByVirtualId(int virtualId)
    {
        if (!this._users.TryGetValue(virtualId, out var user))
        {
            return null;
        }

        return user;
    }

    public RoomUser GetRoomUserByUserId(int pId)
    {
        if (this._usersByUserID.ContainsKey(pId))
        {
            return this._usersByUserID[pId];
        }
        else
        {
            return null;
        }
    }

    public RoomUser GetUserByTracker(string webIP, string machineId)
    {
        foreach (var user in this.GetUserList())
        {
            if (user == null)
            {
                continue;
            }

            if (user.GetClient() == null)
            {
                continue;
            }

            if (user.GetClient().GetUser() == null)
            {
                continue;
            }

            if (user.GetClient().GetConnection() == null)
            {
                continue;
            }

            if (user.GetClient().MachineId != machineId)
            {
                continue;
            }

            if (user.GetClient().GetUser().IP != webIP)
            {
                continue;
            }

            return user;
        }

        return null;
    }

    public List<RoomUser> GetRoomUsers()
    {
        var list = new List<RoomUser>();

        list = this.GetUserList().Where(x => !x.IsBot).ToList();

        return list;
    }

    public ICollection<RoomUser> GetUserList() => this._users.Values;

    public RoomUser GetBotByName(string name) => this._bots.Values.FirstOrDefault(b => b.IsBot && b.BotData.Name == name);

    public RoomUser GetBotOrPetByName(string name) => this._bots.Values.Concat(this._pets.Values).FirstOrDefault(b => (b.IsBot && b.BotData.Name == name) || (b.IsPet && b.BotData.Name == name));

    public List<RoomUser> GetStaffRoomUser()
    {
        var list = new List<RoomUser>();
        foreach (var userId in this._usersRank)
        {
            var roomUser = this.GetRoomUserByUserId(userId);
            if (roomUser != null)
            {
                list.Add(roomUser);
            }
        }
        return list;
    }

    public RoomUser GetRoomUserByName(string pName)
    {
        if (this._usersByUsername.ContainsKey(pName.ToLower()))
        {
            return this._usersByUsername[pName.ToLower()];
        }
        else
        {
            return null;
        }
    }

    public void SaveBots(IQueryAdapter dbClient)
    {
        var botList = this.GetBots();
        if (botList.Count <= 0)
        {
            return;
        }

        BotUserDao.SaveBots(dbClient, botList);
    }

    public void SavePets(IQueryAdapter dbClient)
    {
        var petlist = this.GetPets();
        if (petlist.Count <= 0)
        {
            return;
        }

        BotPetDao.SavePet(dbClient, petlist);
    }

    public List<RoomUser> GetBots()
    {
        var bots = new List<RoomUser>();
        foreach (var user in this._bots.Values.ToList())
        {
            if (user == null || !user.IsBot || user.IsPet)
            {
                continue;
            }

            bots.Add(user);
        }

        return bots;
    }

    public List<RoomUser> GetPets()
    {
        var pets = new List<RoomUser>();
        foreach (var user in this._pets.Values.ToList())
        {
            if (user == null || !user.IsPet)
            {
                continue;
            }

            pets.Add(user);
        }

        return pets;
    }

    public void SerializeStatusUpdates()
    {
        var users = new List<RoomUser>();
        var roomUsers = this.GetUserList();

        if (roomUsers == null)
        {
            return;
        }

        foreach (var user in roomUsers.ToList())
        {
            if (user == null || !user.UpdateNeeded)
            {
                continue;
            }

            user.UpdateNeeded = false;
            users.Add(user);
        }

        if (users.Count > 0)
        {
            this._room.SendPacket(new UserUpdateComposer(users));
        }
    }

    public void UpdateUserStatusses()
    {
        foreach (var user in this.GetUserList().ToList())
        {
            this.UpdateUserStatus(user, false);
        }
    }

    private bool IsValid(RoomUser user) => user.IsBot || (user.GetClient() != null && user.GetClient().GetUser() != null && user.GetClient().GetUser().CurrentRoomId == this._room.Id);

    public bool TryGetPet(int petId, out RoomUser pet) => this._pets.TryGetValue(petId, out pet);

    public bool TryGetBot(int botId, out RoomUser bot) => this._bots.TryGetValue(botId, out bot);

    public void UpdateUserStatus(RoomUser user, bool cyclegameitems)
    {
        if (user == null)
        {
            return;
        }

        if (user.ContainStatus("lay") || user.ContainStatus("sit") || user.ContainStatus("sign"))
        {
            if (user.ContainStatus("lay"))
            {
                user.RemoveStatus("lay");
            }

            if (user.ContainStatus("sit"))
            {
                user.RemoveStatus("sit");
            }

            if (user.ContainStatus("sign"))
            {
                user.RemoveStatus("sign");
            }

            user.UpdateNeeded = true;
        }

        var roomItemForSquare = this._room.GetGameMap().GetCoordinatedItems(new Point(user.X, user.Y)).OrderBy(p => p.Z).ToList();

        var newZ = !user.RidingHorse || user.IsPet ? this._room.GetGameMap().SqAbsoluteHeight(user.X, user.Y, roomItemForSquare) : this._room.GetGameMap().SqAbsoluteHeight(user.X, user.Y, roomItemForSquare) + 1.0;
        if (newZ != user.Z)
        {
            user.Z = newZ;
            user.UpdateNeeded = true;
        }

        foreach (var roomItem in roomItemForSquare)
        {
            if (user == null)
            {
                continue;
            }

            if (cyclegameitems)
            {
                if (roomItem.EffectId != 0 && !user.IsBot)
                {
                    user.ApplyEffect(roomItem.EffectId);
                }

                roomItem.UserWalksOnFurni(user, roomItem);
            }

            if (roomItem.GetBaseItem().IsSeat)
            {
                if (!user.ContainStatus("sit"))
                {
                    user.SetStatus("sit", roomItem.Height.ToString());
                    user.IsSit = true;
                }
                user.Z = roomItem.Z;
                user.RotHead = roomItem.Rotation;
                user.RotBody = roomItem.Rotation;
                user.UpdateNeeded = true;
            }

            switch (roomItem.GetBaseItem().InteractionType)
            {
                case InteractionType.BED:
                    if (!user.ContainStatus("lay"))
                    {
                        user.SetStatus("lay", roomItem.Height.ToString() + " null");
                        user.IsLay = true;
                    }
                    user.Z = roomItem.Z;
                    user.RotHead = roomItem.Rotation;
                    user.RotBody = roomItem.Rotation;
                    user.UpdateNeeded = true;
                    break;
                case InteractionType.PRESSUREPAD:
                case InteractionType.TRAMPOLINE:
                case InteractionType.TREADMILL:
                case InteractionType.CROSSTRAINER:
                    roomItem.ExtraData = "1";
                    roomItem.UpdateState(false, true);
                    break;
                case InteractionType.GUILD_GATE:
                    roomItem.ExtraData = "1;" + roomItem.GroupId;
                    roomItem.UpdateState(false, true);
                    break;
                case InteractionType.ARROW:
                    if (!cyclegameitems || user.IsBot)
                    {
                        break;
                    }

                    if (roomItem.InteractingUser != 0)
                    {
                        break;
                    }

                    user.CanWalk = true;
                    roomItem.InteractingUser = user.GetClient().GetUser().Id;
                    roomItem.ReqUpdate(2);
                    break;
                case InteractionType.BANZAIGATEBLUE:
                case InteractionType.BANZAIGATERED:
                case InteractionType.BANZAIGATEYELLOW:
                case InteractionType.BANZAIGATEGREEN:
                    if (cyclegameitems && !user.IsBot)
                    {
                        var effectId = (int)roomItem.Team + 32;
                        var managerForBanzai = this._room.GetTeamManager();
                        if (user.Team != roomItem.Team)
                        {
                            if (user.Team != TeamType.NONE)
                            {
                                managerForBanzai.OnUserLeave(user);
                            }

                            user.Team = roomItem.Team;
                            managerForBanzai.AddUser(user);

                            this._room.GetGameManager().UpdateGatesTeamCounts();
                            if (user.CurrentEffect != effectId)
                            {
                                user.ApplyEffect(effectId);
                            }

                            if (user.GetClient() != null)
                            {
                                user.GetClient().SendPacket(new IsPlayingComposer(true));
                            }
                        }
                        else
                        {
                            managerForBanzai.OnUserLeave(user);
                            this._room.GetGameManager().UpdateGatesTeamCounts();
                            if (user.CurrentEffect == effectId)
                            {
                                user.ApplyEffect(0);
                            }

                            if (user.GetClient() != null)
                            {
                                user.GetClient().SendPacket(new IsPlayingComposer(false));
                            }

                            user.Team = TeamType.NONE;
                            continue;
                        }
                    }
                    break;
                case InteractionType.BANZAIBLO:
                    if (cyclegameitems && user.Team != TeamType.NONE && !user.IsBot)
                    {
                        this._room.GetGameItemHandler().OnWalkableBanzaiBlo(user, roomItem);
                    }
                    break;
                case InteractionType.BANZAIBLOB:
                    if (cyclegameitems && user.Team != TeamType.NONE && !user.IsBot)
                    {
                        this._room.GetGameItemHandler().OnWalkableBanzaiBlob(user, roomItem);
                    }
                    break;
                case InteractionType.BANZAITELE:
                    if (cyclegameitems)
                    {
                        this._room.GetGameItemHandler().OnTeleportRoomUserEnter(user, roomItem);
                    }

                    break;
                case InteractionType.FREEZEYELLOWGATE:
                case InteractionType.FREEZEREDGATE:
                case InteractionType.FREEZEGREENGATE:
                case InteractionType.FREEZEBLUEGATE:
                    if (cyclegameitems && !user.IsBot)
                    {
                        var effectId = (int)roomItem.Team + 39;
                        var managerForFreeze = this._room.GetTeamManager();
                        if (user.Team != roomItem.Team)
                        {
                            if (user.Team != TeamType.NONE)
                            {
                                managerForFreeze.OnUserLeave(user);
                            }

                            user.Team = roomItem.Team;
                            managerForFreeze.AddUser(user);
                            this._room.GetGameManager().UpdateGatesTeamCounts();
                            if (user.CurrentEffect != effectId)
                            {
                                user.ApplyEffect(effectId);
                            }

                            if (user.GetClient() != null)
                            {
                                user.GetClient().SendPacket(new IsPlayingComposer(true));
                            }
                        }
                        else
                        {
                            managerForFreeze.OnUserLeave(user);
                            this._room.GetGameManager().UpdateGatesTeamCounts();
                            if (user.CurrentEffect == effectId)
                            {
                                user.ApplyEffect(0);
                            }

                            if (user.GetClient() != null)
                            {
                                user.GetClient().SendPacket(new IsPlayingComposer(false));
                            }

                            user.Team = TeamType.NONE;
                        }
                    }
                    break;
                case InteractionType.FBGATE:
                    if (cyclegameitems || string.IsNullOrEmpty(roomItem.ExtraData) || !roomItem.ExtraData.Contains(',') || user == null || user.IsBot || user.IsTransf || user.IsSpectator)
                    {
                        break;
                    }

                    if (user.GetClient().GetUser().LastMovFGate && user.GetClient().GetUser().BackupGender == user.GetClient().GetUser().Gender)
                    {
                        user.GetClient().GetUser().LastMovFGate = false;
                        user.GetClient().GetUser().Look = user.GetClient().GetUser().BackupLook;
                    }
                    else
                    {
                        // mini Fix
                        var gateLook = (user.GetClient().GetUser().Gender.ToUpper() == "M") ? roomItem.ExtraData.Split(',')[0] : roomItem.ExtraData.Split(',')[1];
                        if (gateLook == "")
                        {
                            break;
                        }

                        var gateFullLook = "";
                        foreach (var part in gateLook.Split('.'))
                        {
                            if (part.StartsWith("hd"))
                            {
                                continue;
                            }

                            gateFullLook += part + ".";
                        }
                        gateFullLook = gateFullLook[..^1];

                        // Generating New Look.
                        var parts = user.GetClient().GetUser().Look.Split('.');
                        var newLook = "";
                        foreach (var part in parts)
                        {
                            if (/*Part.StartsWith("hd") || */part.StartsWith("sh") || part.StartsWith("cp") || part.StartsWith("cc") || part.StartsWith("ch") || part.StartsWith("lg") || part.StartsWith("ca") || part.StartsWith("wa"))
                            {
                                continue;
                            }

                            newLook += part + ".";
                        }
                        newLook += gateFullLook;

                        user.GetClient().GetUser().BackupLook = user.GetClient().GetUser().Look;
                        user.GetClient().GetUser().BackupGender = user.GetClient().GetUser().Gender;
                        user.GetClient().GetUser().Look = newLook;
                        user.GetClient().GetUser().LastMovFGate = true;
                    }

                    user.GetClient().SendPacket(new UserChangeComposer(user, true));

                    if (user.GetClient().GetUser().InRoom)
                    {
                        this._room.SendPacket(new UserChangeComposer(user, false));
                    }
                    break;
                case InteractionType.FREEZETILEBLOCK:
                    if (!cyclegameitems)
                    {
                        break;
                    }

                    this._room.GetFreeze().OnWalkFreezeBlock(roomItem, user);
                    break;
                default:
                    break;
            }
        }
        if (cyclegameitems)
        {
            this._room.GetBanzai().HandleBanzaiTiles(user.Coordinate, user.Team, user);
        }

        if (user.IsSit || user.IsLay)
        {
            if (user.IsSit)
            {
                if (!user.ContainStatus("sit"))
                {
                    if (user.IsTransf)
                    {
                        user.SetStatus("sit", "0");
                    }
                    else
                    {
                        user.SetStatus("sit", "0.5");
                    }

                    user.UpdateNeeded = true;
                }
            }
            else if (user.IsLay)
            {
                if (!user.ContainStatus("lay"))
                {
                    if (user.IsTransf)
                    {
                        user.SetStatus("lay", "0");
                    }
                    else
                    {
                        user.SetStatus("lay", "0.7");
                    }

                    user.UpdateNeeded = true;
                }

            }
        }
    }

    public void OnCycle(ref int idleCount)
    {
        var userCounter = 0;

        var toRemove = new List<RoomUser>();

        foreach (var user in this.GetUserList().OrderBy(a => Guid.NewGuid()).ToList())
        {
            if (!this.IsValid(user))
            {
                if (user.GetClient() != null && user.GetClient().GetUser() != null)
                {
                    this.RemoveUserFromRoom(user.GetClient(), false, false);
                }
                else
                {
                    this.RemoveRoomUser(user);
                }
            }

            if (user.IsDispose)
            {
                continue;
            }

            if (user.RidingHorse && user.IsPet)
            {
                continue;
            }

            if (this._room.IsRoleplay)
            {
                var rpManager = WibboEnvironment.GetGame().GetRoleplayManager().GetRolePlay(this._room.RoomData.OwnerId);
                if (rpManager != null)
                {
                    if (user.IsBot)
                    {
                        if (user.BotData.RoleBot != null)
                        {
                            user.BotData.RoleBot.OnCycle(user, this._room);
                        }
                    }
                    else
                    {
                        var rp = user.Roleplayer;
                        if (rp != null)
                        {
                            rp.OnCycle(user, rpManager);
                        }
                    }
                }
            }

            user.IdleTime++;

            if (!user.IsAsleep && user.IdleTime >= 600 && !user.IsBot)
            {
                user.IsAsleep = true;
                this._room.SendPacket(new SleepComposer(user.VirtualId, true));
            }

            if (user.CarryItemID > 0 && user.CarryTimer > 0)
            {
                user.CarryTimer--;
                if (user.CarryTimer <= 0)
                {
                    user.CarryItem(0);
                }
            }

            if (user.UserTimer > 0)
            {
                user.UserTimer--;
            }

            if (user.FreezeEndCounter > 0)
            {
                user.FreezeEndCounter--;
                if (user.FreezeEndCounter <= 0)
                {
                    user.Freeze = false;
                }
            }

            if (user.TimerResetEffect > 0)
            {
                user.TimerResetEffect--;
                if (user.TimerResetEffect <= 0)
                {
                    user.ApplyEffect(user.CurrentEffect, true);
                }
            }

            if (this._room.GotFreeze())
            {
                this._room.GetFreeze().CycleUser(user);
            }

            if (user.SetStep)
            {
                if (this.SetStepForUser(user))
                {
                    continue;
                }

                if (user.RidingHorse && !user.IsPet)
                {
                    var roomUserByVirtualId = this.GetRoomUserByVirtualId(Convert.ToInt32(user.HorseID));
                    if (this.SetStepForUser(roomUserByVirtualId))
                    {
                        continue;
                    }
                }
            }
            else
            {
                user.AllowMoveToRoller = true;
                user.AllowBall = true;
                user.MoveWithBall = false;
            }

            if (user.IsWalking && !user.Freezed && !user.Freeze && !(this._room.FreezeRoom && user.GetClient() != null && user.GetClient().GetUser().Rank < 6))
            {
                this.CalculatePath(user);

                user.UpdateNeeded = true;
                if (user.RidingHorse && !user.IsPet)
                {
                    var roomUserByVirtualId = this.GetRoomUserByVirtualId(Convert.ToInt32(user.HorseID));
                    roomUserByVirtualId.UpdateNeeded = true;
                }
            }
            else if (user.ContainStatus("mv"))
            {
                user.RemoveStatus("mv");
                user.IsWalking = false;
                user.UpdateNeeded = true;

                if (user.RidingHorse && !user.IsPet)
                {
                    var roomUserByVirtualId = this.GetRoomUserByVirtualId(Convert.ToInt32(user.HorseID));
                    roomUserByVirtualId.RemoveStatus("mv");
                    roomUserByVirtualId.IsWalking = false;
                    roomUserByVirtualId.UpdateNeeded = true;
                }
            }

            if (user.IsBot && user.BotAI != null)
            {
                user.BotAI.OnTimerTick();
            }
            else if (!user.IsSpectator)
            {
                userCounter++;
            }
        }

        if (userCounter == 0)
        {
            idleCount++;
        }

        foreach (var user in toRemove)
        {
            var clientByUserId = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(user.UserId);
            if (clientByUserId != null)
            {
                this.RemoveUserFromRoom(clientByUserId, true, false);
            }
            else
            {
                this.RemoveRoomUser(user);
            }
        }
        toRemove.Clear();

        this.UpdateUserCount(userCounter);
    }

    private void CalculatePath(RoomUser user)
    {
        var gameMap = this._room.GetGameMap();

        var nextStep = Pathfinder.GetNextStep(user.X, user.Y, user.GoalX, user.GoalY, gameMap.GameMap, gameMap.ItemHeightMap, gameMap.UserOnMap, gameMap.SquareTaking, gameMap.Model.MapSizeX, gameMap.Model.MapSizeY, user.AllowOverride, gameMap.DiagonalEnabled, this._room.RoomData.AllowWalkthrough, gameMap.ObliqueDisable);
        if (user.WalkSpeed)
        {
            nextStep = Pathfinder.GetNextStep(nextStep.X, nextStep.Y, user.GoalX, user.GoalY, gameMap.GameMap, gameMap.ItemHeightMap, gameMap.UserOnMap, gameMap.SquareTaking, gameMap.Model.MapSizeX, gameMap.Model.MapSizeY, user.AllowOverride, gameMap.DiagonalEnabled, this._room.RoomData.AllowWalkthrough, gameMap.ObliqueDisable);
        }

        if (user.BreakWalkEnable && user.StopWalking)
        {
            user.StopWalking = false;
            this.UpdateUserStatus(user, false);
            user.RemoveStatus("mv");

            if (user.RidingHorse && !user.IsPet)
            {
                var roomUserByVirtualId = this.GetRoomUserByVirtualId(Convert.ToInt32(user.HorseID));
                roomUserByVirtualId.IsWalking = false;
                this.UpdateUserStatus(roomUserByVirtualId, false);
                roomUserByVirtualId.RemoveStatus("mv");
            }
        }
        else if ((nextStep.X == user.X && nextStep.Y == user.Y) || this._room.GetGameItemHandler().CheckGroupGate(user, new Point(nextStep.X, nextStep.Y)))
        {
            user.IsWalking = false;
            this.UpdateUserStatus(user, false);
            user.RemoveStatus("mv");

            if (user.RidingHorse && !user.IsPet)
            {
                var roomUserByVirtualId = this.GetRoomUserByVirtualId(Convert.ToInt32(user.HorseID));
                roomUserByVirtualId.IsWalking = false;
                this.UpdateUserStatus(roomUserByVirtualId, false);
                roomUserByVirtualId.RemoveStatus("mv");
            }
        }
        else
        {
            this.HandleSetMovement(nextStep, user);

            if (user.BreakWalkEnable && !user.StopWalking)
            {
                user.StopWalking = true;
            }

            if (user.RidingHorse && !user.IsPet)
            {
                var roomUserByVirtualId = this.GetRoomUserByVirtualId(Convert.ToInt32(user.HorseID));
                this.HandleSetMovement(nextStep, roomUserByVirtualId);
                roomUserByVirtualId.UpdateNeeded = true;
            }

            if (user.IsSit || user.IsLay)
            {
                user.IsSit = false;
                user.IsLay = false;
            }

            this._room.GetSoccer().OnUserWalk(user, nextStep.X == user.GoalX && nextStep.Y == user.GoalY);
            this._room.GetBanzai().OnUserWalk(user);
        }
    }

    private void HandleSetMovement(SquarePoint nextStep, RoomUser user)
    {
        var nextX = nextStep.X;
        var nextY = nextStep.Y;

        var nextZ = this._room.GetGameMap().SqAbsoluteHeight(nextX, nextY);
        if (user.RidingHorse && !user.IsPet)
        {
            nextZ += 1;
        }

        user.RemoveStatus("mv");
        user.RemoveStatus("lay");
        user.RemoveStatus("sit");

        user.SetStatus("mv", nextX + "," + nextY + "," + nextZ);

        int newRot;
        if (user.FacewalkEnabled)
        {
            newRot = user.RotBody;
        }
        else
        {
            newRot = Rotation.Calculate(user.X, user.Y, nextX, nextY, user.MoonwalkEnabled);
        }

        user.RotBody = newRot;
        user.RotHead = newRot;

        user.SetStep = true;
        user.SetX = nextX;
        user.SetY = nextY;
        user.SetZ = nextZ;

        this._room.GetGameMap().AddTakingSquare(nextX, nextY);

        this.UpdateUserEffect(user, user.SetX, user.SetY);
    }

    private bool SetStepForUser(RoomUser user)
    {
        this._room.GetGameMap().UpdateUserMovement(user.Coordinate, new Point(user.SetX, user.SetY), user);

        var coordinatedItems = this._room.GetGameMap().GetCoordinatedItems(new Point(user.X, user.Y)).ToList();

        user.X = user.SetX;
        user.Y = user.SetY;
        user.Z = user.SetZ;

        this._room.CollisionUser(user);

        if (user.IsBot)
        {
            var botCollisionUser = this._room.GetGameMap().LookHasUserNearNotBot(user.X, user.Y);
            if (botCollisionUser != null)
            {
                this._room.GetWiredHandler().TriggerBotCollision(botCollisionUser, user.BotData.Name);
            }
        }

        if (this._room.IsRoleplay)
        {
            var rp = user.Roleplayer;
            if (rp != null && !rp.Dead)
            {
                var itemTmp = this._room.GetRoomItemHandler().GetFirstTempDrop(user.X, user.Y);
                if (itemTmp != null && itemTmp.InteractionType == InteractionTypeTemp.MONEY)
                {
                    rp.Money += itemTmp.Value;
                    rp.SendUpdate();
                    if (user.GetClient() != null)
                    {
                        user.SendWhisperChat(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("rp.pickdollard", user.GetClient().Langue), itemTmp.Value));
                    }

                    user.OnChat("*Récupère un objet au sol*");
                    this._room.GetRoomItemHandler().RemoveTempItem(itemTmp.Id);
                }
                else if (itemTmp != null && itemTmp.InteractionType == InteractionTypeTemp.RPITEM)
                {
                    var rpItem = WibboEnvironment.GetGame().GetRoleplayManager().GetItemManager().GetItem(itemTmp.Value);
                    if (rpItem != null)
                    {
                        if (!rpItem.AllowStack && rp.GetInventoryItem(rpItem.Id) != null)
                        {
                            if (user.GetClient() != null)
                            {
                                user.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("rp.itemown", user.GetClient().Langue));
                            }
                        }
                        else
                        {
                            rp.AddInventoryItem(rpItem.Id);

                            if (user.GetClient() != null)
                            {
                                user.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("rp.itempick", user.GetClient().Langue));
                            }
                        }
                    }

                    user.OnChat("*Récupère un objet au sol*");
                    this._room.GetRoomItemHandler().RemoveTempItem(itemTmp.Id);
                }
            }
        }

        foreach (var roomItem in coordinatedItems)
        {
            roomItem.UserWalksOffFurni(user, roomItem);

            if (roomItem.GetBaseItem().InteractionType == InteractionType.GUILD_GATE)
            {
                roomItem.ExtraData = "0;" + roomItem.GroupId;
                roomItem.UpdateState(false, true);
            }
            else if (roomItem.GetBaseItem().InteractionType is InteractionType.PRESSUREPAD
                or InteractionType.TRAMPOLINE
                or InteractionType.TREADMILL
                or InteractionType.CROSSTRAINER)
            {
                roomItem.ExtraData = "0";
                roomItem.UpdateState(false, true);
            }
            else if (roomItem.GetBaseItem().InteractionType == InteractionType.FOOTBALL)
            {
                if (!user.AllowMoveToRoller || roomItem.InteractionCountHelper > 0 || this._room.OldFoot)
                {
                    continue;
                }

                switch (user.RotBody)
                {
                    case 0:
                        roomItem.MovementDir = MovementDirection.down;
                        break;
                    case 1:
                        roomItem.MovementDir = MovementDirection.downleft;
                        break;
                    case 2:
                        roomItem.MovementDir = MovementDirection.left;
                        break;
                    case 3:
                        roomItem.MovementDir = MovementDirection.upleft;
                        break;
                    case 4:
                        roomItem.MovementDir = MovementDirection.up;
                        break;
                    case 5:
                        roomItem.MovementDir = MovementDirection.upright;
                        break;
                    case 6:
                        roomItem.MovementDir = MovementDirection.right;
                        break;
                    case 7:
                        roomItem.MovementDir = MovementDirection.downright;
                        break;
                }
                roomItem.InteractionCountHelper = 6;
                roomItem.InteractingUser = user.VirtualId;
                roomItem.ReqUpdate(1);
            }
        }

        this.UpdateUserStatus(user, true);
        this._room.GetGameMap().RemoveTakingSquare(user.SetX, user.SetY);

        user.SetStep = false;
        user.AllowMoveToRoller = false;

        if (user.SetMoveWithBall)
        {
            user.SetMoveWithBall = false;
            user.MoveWithBall = false;
        }
        return false;
    }

    public void Destroy()
    {
        this._usersByUsername.Clear();
        this._usersByUserID.Clear();
        this.OnUserEnter = null;
        this._pets.Clear();
        this._bots.Clear();
        this._users.Clear();
        this._usersRank.Clear();
    }
}
