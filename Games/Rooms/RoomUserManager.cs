namespace WibboEmulator.Games.Rooms;
using System.Collections.Concurrent;
using System.Drawing;
using WibboEmulator.Communication.Packets.Outgoing.Handshake;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Avatar;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Permissions;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Session;
using WibboEmulator.Database.Daos.Bot;
using System.Data;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Quests;
using WibboEmulator.Games.Roleplays.Enemy;
using WibboEmulator.Games.Rooms.AI;
using WibboEmulator.Games.Rooms.Games.Teams;
using WibboEmulator.Games.Rooms.Map;
using WibboEmulator.Games.Rooms.Map.Movement;
using WibboEmulator.Games.Rooms.PathFinding;
using WibboEmulator.Utilities;
using WibboEmulator.Core.Language;
using WibboEmulator.Games.Roleplays;
using WibboEmulator.Games.Roleplays.Item;
using WibboEmulator.Games.Roleplays.Troc;

public class RoomUserManager(Room room)
{
    private readonly ConcurrentDictionary<string, RoomUser> _usersByUsername = new();
    private readonly ConcurrentDictionary<int, RoomUser> _usersByUserID = new();

    private readonly ConcurrentDictionary<int, RoomUser> _users = new();
    private readonly ConcurrentDictionary<int, RoomUser> _pets = new();
    private readonly ConcurrentDictionary<int, RoomUser> _bots = new();

    private readonly List<int> _staffIds = [];

    private int _primaryPrivateUserID = 1;
    public int BotPetCount => this._pets.Count + this._bots.Count;

    public event EventHandler OnUserEnter;
    public event EventHandler OnUserExit;

    public void UserEnter(RoomUser thisUser) => this.OnUserEnter?.Invoke(thisUser, new());

    public void UserExit(RoomUser thisUser) => this.OnUserExit?.Invoke(thisUser, new());

    public int RoomUserCount => room.RoomData.UsersNow;

    public RoomUser DeploySuperBot(RoomBot bot)
    {
        var key = this._primaryPrivateUserID++;
        var roomUser = new RoomUser(0, room.Id, key, room);

        bot.Id = -key;

        _ = this._users.TryAdd(key, roomUser);

        roomUser.SetPos(bot.X, bot.Y, bot.Z);
        roomUser.SetRot(bot.Rot, false);

        roomUser.BotData = bot;
        roomUser.BotAI = bot.GenerateBotAI(roomUser.VirtualId);
        roomUser.BotAI.Initialize(bot.Id, roomUser, room);

        roomUser.SetStatus("flatctrl", "4");
        this.UpdateUserStatus(roomUser, false);
        roomUser.UpdateNeeded = true;

        room.SendPacket(new UsersComposer(roomUser));

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
        var roomUser = new RoomUser(0, room.Id, key, room);

        _ = this._users.TryAdd(key, roomUser);

        roomUser.SetPos(bot.X, bot.Y, bot.Z);
        roomUser.SetRot(bot.Rot, false);

        roomUser.BotData = bot;

        if (room.IsRoleplay)
        {
            RPEnemy enemy;
            if (bot.IsPet)
            {
                enemy = RPEnemyManager.GetEnemyPet(bot.Id);
            }
            else
            {
                enemy = RPEnemyManager.GetEnemyBot(bot.Id);
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
        roomUser.BotAI.Initialize(bot.Id, roomUser, room);

        if (roomUser.IsPet)
        {
            roomUser.PetData = petData;
            roomUser.PetData.VirtualId = roomUser.VirtualId;
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
            room.SendPacket(new DanceComposer(roomUser.VirtualId, 3));
        }

        if (bot.Enable > 0)
        {
            roomUser.ApplyEffect(bot.Enable);
        }

        if (bot.Handitem > 0)
        {
            roomUser.CarryItem(bot.Handitem, true);
        }

        room.SendPacket(new UsersComposer(roomUser));

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

        room.SendPacket(new UserRemoveComposer(roomUserByVirtualId.VirtualId));

        room.GameMap.RemoveTakingSquare(roomUserByVirtualId.SetX, roomUserByVirtualId.SetY);
        room.GameMap.RemoveUserFromMap(roomUserByVirtualId, new Point(roomUserByVirtualId.X, roomUserByVirtualId.Y));

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

        if (!room.GameMap.ValidTile(x, y))
        {
            return;
        }

        var effectId = (int)room.GameMap.EffectMap[x, y];
        if (effectId > 0)
        {
            var itemEffectType = effectId.ToEnum(ItemEffectType.None);
            if (itemEffectType == user.CurrentItemEffect)
            {
                return;
            }

            switch (itemEffectType)
            {
                case ItemEffectType.None:
                    user.ApplyEffect(0);
                    user.CurrentItemEffect = itemEffectType;
                    break;
                case ItemEffectType.Swim:
                    user.ApplyEffect(29);
                    if (user.Client != null)
                    {
                        QuestManager.ProgressUserQuest(user.Client, QuestType.ExploreFindItem, 1948);
                    }

                    user.CurrentItemEffect = itemEffectType;
                    break;
                case ItemEffectType.SwimLow:
                    user.ApplyEffect(30);
                    user.CurrentItemEffect = itemEffectType;
                    break;
                case ItemEffectType.SwimHalloween:
                    user.ApplyEffect(37);
                    user.CurrentItemEffect = itemEffectType;
                    break;
                case ItemEffectType.IceSkates:
                    if (user.Client != null)
                    {
                        if (user.Client.User.Gender == "M")
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

                    user.CurrentItemEffect = ItemEffectType.IceSkates;
                    if (user.Client != null)
                    {
                        QuestManager.ProgressUserQuest(user.Client, QuestType.ExploreFindItem, 1413);
                    }

                    break;
                case ItemEffectType.NormalSkates:
                    if (user.Client != null)
                    {
                        if (user.Client.User.Gender == "M")
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
                    if (user.Client != null)
                    {
                        QuestManager.ProgressUserQuest(user.Client, QuestType.ExploreFindItem, 2199);
                    }

                    break;
                case ItemEffectType.Trampoline:
                    user.ApplyEffect(193);
                    user.CurrentItemEffect = itemEffectType;
                    break;
                case ItemEffectType.Treadmill:
                    user.ApplyEffect(194);
                    user.CurrentItemEffect = itemEffectType;
                    break;
                case ItemEffectType.Crosstrainer:
                    user.ApplyEffect(195);
                    user.CurrentItemEffect = itemEffectType;
                    break;

            }
        }
        else
        {
            if (user.CurrentItemEffect == ItemEffectType.None || effectId != 0)
            {
                return;
            }

            user.ApplyEffect(0);
            user.CurrentItemEffect = ItemEffectType.None;
        }
    }

    public List<RoomUser> GetUsersForSquare(int x, int y) => [.. room.GameMap.GetRoomUsers(new Point(x, y)).OrderBy(u => u.IsBot)];

    public RoomUser GetUserForSquare(int x, int y) => Enumerable.FirstOrDefault(room.GameMap.GetRoomUsers(new Point(x, y)).OrderBy(u => u.IsBot));

    public RoomUser GetUserForSquareNotBot(int x, int y) => room.GameMap.GetRoomUsers(new Point(x, y)).FirstOrDefault(u => !u.IsBot);

    public bool AddAvatarToRoom(GameClient session)
    {
        if (room == null)
        {
            return false;
        }

        if (session == null || session.User == null)
        {
            return false;
        }

        var personalID = this._primaryPrivateUserID++;

        var user = new RoomUser(session.User.Id, room.Id, personalID, room)
        {
            IsSpectator = session.User.IsSpectator
        };

        if (!this._users.TryAdd(personalID, user))
        {
            return false;
        }

        if (session.User.Rank > 5 && !this._staffIds.Contains(user.UserId))
        {
            this._staffIds.Add(user.UserId);
        }

        session.User.RoomId = room.Id;
        session.User.LoadingRoomId = 0;

        var username = session.User.Username;
        var userId = session.User.Id;

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

        var roomModel = room.GameMap.Model;
        if (roomModel == null)
        {
            return false;
        }

        user.SetPos(roomModel.DoorX, roomModel.DoorY, roomModel.DoorZ);
        user.SetRot(roomModel.DoorOrientation, false);

        if (user.Client != null && user.Client.User != null)
        {
            if (session.User.IsTeleporting)
            {
                var roomItem = room.RoomItemHandling.GetItem(user.Client.User.TeleporterId);
                if (roomItem != null)
                {
                    GameMap.TeleportToItem(user, roomItem);

                    roomItem.InteractingUser2 = session.User.Id;
                    roomItem.ReqUpdate(1);
                }
            }

            user.Client.User.IsTeleporting = false;
            user.Client.User.TeleporterId = 0;
            user.Client.User.TeleportingRoomID = 0;
        }

        if (!user.IsSpectator)
        {
            room.SendPacket(new UsersComposer(user));
        }

        if (user.IsSpectator)
        {
            var roomUserByRank = room.RoomUserManager.StaffRoomUsers;
            if (roomUserByRank.Count > 0)
            {
                foreach (var staffUser in roomUserByRank)
                {
                    if (staffUser != null && staffUser.Client != null && staffUser.Client.User != null && staffUser.Client.User.HasPermission("show_invisible"))
                    {
                        staffUser.SendWhisperChat(user.Username + " est entré dans l'appart en mode invisible !", true);
                    }
                }
            }
        }

        if (session.User.HasPermission("owner_all_rooms"))
        {
            user.SetStatus("flatctrl", "5");
            session.SendPacket(new YouAreOwnerComposer());
            session.SendPacket(new YouAreControllerComposer(5));
        }
        else if (room.CheckRights(session, true))
        {
            user.SetStatus("flatctrl", "4");
            session.SendPacket(new YouAreOwnerComposer());
            session.SendPacket(new YouAreControllerComposer(4));
        }
        else if (room.CheckRights(session))
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
            var emblemId = session.User.BadgeComponent.EmblemId;

            if (emblemId > 0)
            {
                user.CurrentEffect = emblemId;
                room.SendPacket(new AvatarEffectComposer(user.VirtualId, user.CurrentEffect));
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

        if (!user.IsBot && user.Client != null && user.Client.User != null && room.RoomData.OwnerName != user.Client.User.Username)
        {
            QuestManager.ProgressUserQuest(user.Client, QuestType.SocialVisit, 0);
        }

        if (!user.IsBot)
        {
            if (session.User.RolePlayId > 0 && room.RoomData.OwnerId != session.User.RolePlayId)
            {
                var rpManager = RoleplayManager.GetRolePlay(session.User.RolePlayId);
                if (rpManager != null)
                {
                    var rp = rpManager.GetPlayer(session.User.Id);
                    if (rp != null)
                    {
                        rpManager.RemovePlayer(session.User.Id);
                    }
                }
                session.User.RolePlayId = 0;
            }

            if (room.IsRoleplay && room.RoomData.OwnerId != session.User.RolePlayId)
            {
                var rpManager = RoleplayManager.GetRolePlay(room.RoomData.OwnerId);
                if (rpManager != null)
                {
                    var rp = rpManager.GetPlayer(session.User.Id);
                    if (rp == null)
                    {
                        rpManager.AddPlayer(session.User.Id);
                    }
                }

                session.User.RolePlayId = room.RoomData.OwnerId;
            }
        }

        user.InGame = room.IsRoleplay;

        return true;
    }

    public void RemoveUserFromRoom(GameClient session, bool notifyClient, bool notifyKick)
    {
        if (session == null)
        {
            return;
        }

        if (session.User == null)
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

        var user = this.GetRoomUserByUserId(session.User.Id);
        if (user == null)
        {
            return;
        }

        if (!user.IsSpectator)
        {
            room.RoomUserManager.UserExit(user);
        }

        if (this._staffIds.Contains(user.UserId))
        {
            _ = this._staffIds.Remove(user.UserId);
        }

        if (user.Team != TeamType.None)
        {
            room.TeamManager.OnUserLeave(user);
            room.GameManager.UpdateGatesTeamCounts();

            session.SendPacket(new IsPlayingComposer(false));
        }

        room.JankenManager.RemovePlayer(user);

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

        if (room.HasActiveTrade(session.User.Id))
        {
            room.TryStopTrade(session.User.Id);
        }

        if (user.Roleplayer != null)
        {
            RPTrocManager.RemoveTrade(user.Roleplayer.TradeId);
        }

        if (user.IsSpectator)
        {
            var roomUserByRank = room.RoomUserManager.StaffRoomUsers;
            if (roomUserByRank.Count > 0)
            {
                foreach (var staffUser in roomUserByRank)
                {
                    if (staffUser != null && staffUser.Client != null && staffUser.Client.User != null && staffUser.Client.User.HasPermission("show_invisible"))
                    {
                        staffUser.SendWhisperChat(user.Username + " était en mode invisible. Il vient de partir de l'appartement.", true);
                    }
                }
            }
        }

        session.User.RoomId = 0;
        session.User.LoadingRoomId = 0;

        session.User.ForceUse = -1;

        this.RemoveRoomUser(user);

        user.Freeze = true;
        user.FreezeEndCounter = 0;
        user.Dispose();

        _ = this._usersByUserID.TryRemove(user.UserId, out _);
        _ = this._usersByUsername.TryRemove(session.User.Username.ToLower(), out _);
    }

    private void RemoveRoomUser(RoomUser user)
    {
        room.GameMap.RemoveTakingSquare(user.SetX, user.SetY);
        room.GameMap.RemoveUserFromMap(user, new Point(user.X, user.Y));

        room.SendPacket(new UserRemoveComposer(user.VirtualId));

        _ = this._users.TryRemove(user.VirtualId, out _);
    }

    public void UpdateUserCount(int count)
    {
        if (room.RoomData.UsersNow == count)
        {
            return;
        }

        room.RoomData.UsersNow = count;
    }

    public RoomUser GetRoomUserByVirtualId(int virtualId)
    {
        if (!this._users.TryGetValue(virtualId, out var user))
        {
            return null;
        }

        return user;
    }

    public RoomUser GetRoomUserByUserId(int id)
    {
        if (this._usersByUserID.TryGetValue(id, out var value))
        {
            return value;
        }
        else
        {
            return null;
        }
    }

    public RoomUser GetUserByTracker(string webIP)
    {
        foreach (var user in this.UserList)
        {
            if (user == null)
            {
                continue;
            }

            if (user.Client == null)
            {
                continue;
            }

            if (user.Client.User == null)
            {
                continue;
            }

            if (user.Client.Connection == null)
            {
                continue;
            }

            if (user.Client.User.IP != webIP)
            {
                continue;
            }

            return user;
        }

        return null;
    }

    public List<RoomUser> RoomUsers => this.UserList.Where(x => !x.IsBot).ToList();

    public ICollection<RoomUser> UserList => this._users.Values;

    public RoomUser GetBotByName(string name) => this._bots.Values.FirstOrDefault(b => b.IsBot && b.BotData.Name == name);

    public RoomUser GetBotOrPetByName(string name) => this._bots.Values.Concat(this._pets.Values).FirstOrDefault(b => (b.IsBot && b.BotData.Name == name) || (b.IsPet && b.BotData.Name == name));

    public List<RoomUser> StaffRoomUsers
    {
        get
        {
            var list = new List<RoomUser>();
            foreach (var userId in this._staffIds)
            {
                var roomUser = this.GetRoomUserByUserId(userId);
                if (roomUser != null)
                {
                    list.Add(roomUser);
                }
            }
            return list;
        }
    }

    public RoomUser GetRoomUserByName(string pName)
    {
        if (this._usersByUsername.TryGetValue(pName.ToLower(), out var value))
        {
            return value;
        }
        else
        {
            return null;
        }
    }

    public void SaveBots(IDbConnection dbClient)
    {
        var botList = this.Bots;
        if (botList.Count <= 0)
        {
            return;
        }

        BotUserDao.SaveBots(dbClient, botList);
    }

    public void SavePets(IDbConnection dbClient)
    {
        var petlist = this.Pets;
        if (petlist.Count <= 0)
        {
            return;
        }

        BotPetDao.SavePet(dbClient, petlist);
    }

    public List<RoomUser> Bots
    {
        get
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
    }

    public List<RoomUser> Pets
    {
        get
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
    }

    public void SerializeStatusUpdates()
    {
        var users = new List<RoomUser>();
        var roomUsers = this.UserList;

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
            room.SendPacket(new UserUpdateComposer(users));
        }
    }

    public void UpdateUserStatusses()
    {
        foreach (var user in this.UserList.ToList())
        {
            this.UpdateUserStatus(user, false);
        }
    }

    private bool IsValid(RoomUser user) => user.IsBot || (user.Client != null && user.Client.User != null && user.Client.User.RoomId == room.Id);

    public bool TryGetPet(int petId, out RoomUser pet) => this._pets.TryGetValue(petId, out pet);

    public bool TryGetBot(int botId, out RoomUser bot) => this._bots.TryGetValue(botId, out bot);

    public void UpdateUserStatus(RoomUser user, bool cycleGameItems)
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

        var roomItemForSquare = room.GameMap.GetCoordinatedItems(new Point(user.X, user.Y)).OrderBy(p => p.Z).ToList();

        var newZ = room.GameMap.SqAbsoluteHeight(user.X, user.Y, roomItemForSquare);
        if (user.RidingHorse && !user.IsPet)
        {
            newZ += 1;
        }

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

            if (cycleGameItems)
            {
                if (roomItem.EffectId != 0 && !user.IsBot)
                {
                    user.ApplyEffect(roomItem.EffectId);
                }

                roomItem.UserWalksOnFurni(user, roomItem);
            }

            if (roomItem.ItemData.IsSeat && !user.RidingHorse)
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

            switch (roomItem.ItemData.InteractionType)
            {
                case InteractionType.BED:
                    if (user.RidingHorse)
                    {
                        break;
                    }
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
                case InteractionType.PRESSURE_PAD:
                case InteractionType.TRAMPOLINE:
                case InteractionType.TREADMILL:
                case InteractionType.CROSSTRAINER:
                    roomItem.ExtraData = "1";
                    roomItem.UpdateState(false);
                    break;
                case InteractionType.GUILD_GATE:
                    roomItem.ExtraData = "1;" + roomItem.GroupId;
                    roomItem.UpdateState(false);
                    break;
                case InteractionType.TELEPORT_ARROW:
                    if (!cycleGameItems || user.IsBot || user.Client == null)
                    {
                        break;
                    }

                    if (roomItem.InteractingUser != 0)
                    {
                        break;
                    }

                    user.CanWalk = true;
                    roomItem.InteractingUser = user.Client.User.Id;
                    roomItem.ReqUpdate(2);
                    break;
                case InteractionType.BANZAI_GATE_BLUE:
                case InteractionType.BANZAI_GATE_RED:
                case InteractionType.BANZAI_GATE_YELLOW:
                case InteractionType.BANZAI_GATE_GREEN:
                    if (cycleGameItems && !user.IsBot)
                    {
                        var effectId = (int)roomItem.Team + 32;
                        var managerForBanzai = room.TeamManager;
                        if (user.Team != roomItem.Team)
                        {
                            if (user.Team != TeamType.None)
                            {
                                managerForBanzai.OnUserLeave(user);
                            }

                            user.Team = roomItem.Team;
                            managerForBanzai.AddUser(user);

                            room.GameManager.UpdateGatesTeamCounts();
                            if (user.CurrentEffect != effectId)
                            {
                                user.ApplyEffect(effectId);
                            }

                            user.Client?.SendPacket(new IsPlayingComposer(true));
                        }
                        else
                        {
                            managerForBanzai.OnUserLeave(user);
                            room.GameManager.UpdateGatesTeamCounts();
                            if (user.CurrentEffect == effectId)
                            {
                                user.ApplyEffect(0);
                            }

                            user.Client?.SendPacket(new IsPlayingComposer(false));

                            user.Team = TeamType.None;
                            continue;
                        }
                    }
                    break;
                case InteractionType.BANZAI_BLOB_2:
                    if (cycleGameItems && user.Team != TeamType.None && !user.IsBot)
                    {
                        room.GameItemHandler.OnWalkableBanzaiBlob2(user, roomItem);
                    }
                    break;
                case InteractionType.BANZAI_BLOB:
                    if (cycleGameItems && user.Team != TeamType.None && !user.IsBot)
                    {
                        room.GameItemHandler.OnWalkableBanzaiBlob(user, roomItem);
                    }
                    break;
                case InteractionType.BANZAI_TELE:
                    if (cycleGameItems)
                    {
                        room.GameItemHandler.OnTeleportRoomUserEnter(user, roomItem);
                    }

                    break;
                case InteractionType.FREEZE_YELLOW_GATE:
                case InteractionType.FREEZE_RED_GATE:
                case InteractionType.FREEZE_GREEN_GATE:
                case InteractionType.FREEZE_BLUE_GATE:
                    if (cycleGameItems && !user.IsBot)
                    {
                        var effectId = (int)roomItem.Team + 39;
                        var managerForFreeze = room.TeamManager;
                        if (user.Team != roomItem.Team)
                        {
                            if (user.Team != TeamType.None)
                            {
                                managerForFreeze.OnUserLeave(user);
                            }

                            user.Team = roomItem.Team;
                            managerForFreeze.AddUser(user);
                            room.GameManager.UpdateGatesTeamCounts();
                            if (user.CurrentEffect != effectId)
                            {
                                user.ApplyEffect(effectId);
                            }

                            user.Client?.SendPacket(new IsPlayingComposer(true));
                        }
                        else
                        {
                            managerForFreeze.OnUserLeave(user);
                            room.GameManager.UpdateGatesTeamCounts();
                            if (user.CurrentEffect == effectId)
                            {
                                user.ApplyEffect(0);
                            }

                            user.Client?.SendPacket(new IsPlayingComposer(false));

                            user.Team = TeamType.None;
                        }
                    }
                    break;
                case InteractionType.FOOTBALL_GATE:
                    if (cycleGameItems || string.IsNullOrEmpty(roomItem.ExtraData) || !roomItem.ExtraData.Contains(',') || user == null || user.IsBot || user.IsTransf || user.IsSpectator || user.Client == null)
                    {
                        break;
                    }

                    if (user.Client.User.LastMovFGate && user.Client.User.BackupGender == user.Client.User.Gender)
                    {
                        user.Client.User.LastMovFGate = false;
                        user.Client.User.Look = user.Client.User.BackupLook;
                    }
                    else
                    {
                        // mini Fix
                        var gateLook = (user.Client.User.Gender == "M") ? roomItem.ExtraData.Split(',')[0] : roomItem.ExtraData.Split(',')[1];
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
                        var parts = user.Client.User.Look.Split('.');
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

                        user.Client.
                        User.BackupLook = user.Client.User.Look;
                        user.Client.User.BackupGender = user.Client.User.Gender;
                        user.Client.User.Look = newLook;
                        user.Client.User.LastMovFGate = true;
                    }

                    user.Client.SendPacket(new UserChangeComposer(user, true));

                    if (user.Client.User.InRoom)
                    {
                        room.SendPacket(new UserChangeComposer(user, false));
                    }
                    break;
                case InteractionType.FREEZE_TILE_BLOCK:
                    if (!cycleGameItems)
                    {
                        break;
                    }

                    room.Freeze.OnWalkFreezeBlock(roomItem, user);
                    break;
                default:
                    break;
            }
        }

        if (user == null)
        {
            return;
        }

        if (cycleGameItems)
        {
            room.BattleBanzai.HandleBanzaiTiles(user.Coordinate, user.Team, user);
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

        foreach (var user in this.UserList.OrderBy(a => Guid.NewGuid()).ToList())
        {
            if (!this.IsValid(user))
            {
                toRemove.Add(user);
            }

            if (user.IsDispose)
            {
                continue;
            }

            if (user.RidingHorse && user.IsPet)
            {
                continue;
            }

            if (room.IsRoleplay)
            {
                var rpManager = RoleplayManager.GetRolePlay(room.RoomData.OwnerId);
                if (rpManager != null)
                {
                    if (user.IsBot)
                    {
                        user.BotData?.RoleBot?.OnCycle(user, room);
                    }
                    else
                    {
                        user.Roleplayer?.OnCycle(user, rpManager);
                    }
                }
            }

            user.IdleTime++;

            if (!user.IsAsleep && user.IdleTime >= 600 && !user.IsBot)
            {
                user.IsAsleep = true;
                room.SendPacket(new SleepComposer(user.VirtualId, true));
            }

            if (user.CarryItemId > 0 && user.CarryTimer > 0)
            {
                user.CarryTimer--;
                if (user.CarryTimer <= 0)
                {
                    user.CarryItem(0);
                }
            }

            if (user.SignId > 0 && user.SignTimer > 0)
            {
                user.SignTimer--;
                if (user.SignTimer <= 0)
                {
                    user.Sign(0);
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

            room.Freeze.CycleUser(user);

            if (user.SetStep)
            {
                if (this.SetStepForUser(user))
                {
                    continue;
                }

                if (user.RidingHorse && !user.IsPet)
                {
                    var roomUserByVirtualId = this.GetRoomUserByVirtualId(user.HorseID);
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

            if (user.IsWalking && !user.Freezed && !user.Freeze && !(room.FreezeRoom && user.Client != null && user.Client.User.Rank < 6))
            {
                this.CalculatePath(user);

                user.UpdateNeeded = true;
                if (user.RidingHorse && !user.IsPet)
                {
                    var roomUserByVirtualId = this.GetRoomUserByVirtualId(user.HorseID);
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
                    var roomUserByVirtualId = this.GetRoomUserByVirtualId(user.HorseID);
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
            var clientByUserId = GameClientManager.GetClientByUserID(user.UserId);
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
        var gameMap = room.GameMap;

        var nextStep = Pathfinder.GetNextStep(user.X, user.Y, user.GoalX, user.GoalY, gameMap.MapGame, gameMap.ItemHeightMap, gameMap.UserOnMap, gameMap.SquareTaking, gameMap.Model.MapSizeX, gameMap.Model.MapSizeY, user.AllowOverride, gameMap.DiagonalEnabled, room.RoomData.AllowWalkthrough, gameMap.ObliqueDisable);
        if (user.WalkSpeed)
        {
            nextStep = Pathfinder.GetNextStep(nextStep.X, nextStep.Y, user.GoalX, user.GoalY, gameMap.MapGame, gameMap.ItemHeightMap, gameMap.UserOnMap, gameMap.SquareTaking, gameMap.Model.MapSizeX, gameMap.Model.MapSizeY, user.AllowOverride, gameMap.DiagonalEnabled, room.RoomData.AllowWalkthrough, gameMap.ObliqueDisable);
        }

        if (user.BreakWalkEnable && user.StopWalking)
        {
            user.StopWalking = false;
            this.UpdateUserStatus(user, false);
            user.RemoveStatus("mv");

            if (user.RidingHorse && !user.IsPet)
            {
                var roomUserByVirtualId = this.GetRoomUserByVirtualId(user.HorseID);
                roomUserByVirtualId.IsWalking = false;
                this.UpdateUserStatus(roomUserByVirtualId, false);
                roomUserByVirtualId.RemoveStatus("mv");
            }
        }
        else if ((nextStep.X == user.X && nextStep.Y == user.Y) || room.GameItemHandler.CheckGroupGate(user, new Point(nextStep.X, nextStep.Y)))
        {
            user.IsWalking = false;
            this.UpdateUserStatus(user, false);
            user.RemoveStatus("mv");

            if (user.RidingHorse && !user.IsPet)
            {
                var roomUserByVirtualId = this.GetRoomUserByVirtualId(user.HorseID);
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
                var roomUserByVirtualId = this.GetRoomUserByVirtualId(user.HorseID);
                this.HandleSetMovement(nextStep, roomUserByVirtualId);
                roomUserByVirtualId.UpdateNeeded = true;
            }

            if (user.IsSit || user.IsLay)
            {
                user.IsSit = false;
                user.IsLay = false;
            }

            room.Soccer.OnUserWalk(user, nextStep.X == user.GoalX && nextStep.Y == user.GoalY);
            room.BattleBanzai.OnUserWalk(user);
        }
    }

    private void HandleSetMovement(SquarePoint nextStep, RoomUser user)
    {
        var nextX = nextStep.X;
        var nextY = nextStep.Y;

        var nextZ = room.GameMap.SqAbsoluteHeight(nextX, nextY);
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

        room.GameMap.AddTakingSquare(nextX, nextY);

        this.UpdateUserEffect(user, user.SetX, user.SetY);
    }

    private bool SetStepForUser(RoomUser user)
    {
        room.GameMap.UpdateUserMovement(user.Coordinate, new Point(user.SetX, user.SetY), user);

        var coordinatedItems = room.GameMap.GetCoordinatedItems(new Point(user.X, user.Y)).ToList();

        user.X = user.SetX;
        user.Y = user.SetY;
        user.Z = user.SetZ;

        room.CollisionUser(user);

        if (user.IsBot)
        {
            var botCollisionUser = room.GameMap.LookHasUserNearNotBot(user.X, user.Y);
            if (botCollisionUser != null)
            {
                room.WiredHandler.TriggerBotCollision(botCollisionUser, user.BotData.Name);
            }
        }

        if (room.IsRoleplay)
        {
            var rp = user.Roleplayer;
            if (rp != null && !rp.Dead)
            {
                var itemTmp = room.RoomItemHandling.GetFirstTempDrop(user.X, user.Y);
                if (itemTmp != null && itemTmp.InteractionType == InteractionTypeTemp.Money)
                {
                    rp.Money += itemTmp.Value;
                    rp.SendUpdate();
                    if (user.Client != null)
                    {
                        user.SendWhisperChat(string.Format(LanguageManager.TryGetValue("rp.pickdollard", user.Client.Language), itemTmp.Value));
                    }

                    if (user.Client != null)
                    {
                        user.OnChat(LanguageManager.TryGetValue("rp.chat.takeitem", user.Client.Language));
                    }
                    room.RoomItemHandling.RemoveTempItem(itemTmp.Id);
                }
                else if (itemTmp != null && itemTmp.InteractionType == InteractionTypeTemp.RpItem)
                {
                    var rpItem = RPItemManager.GetItem(itemTmp.Value);
                    if (rpItem != null)
                    {
                        if (!rpItem.AllowStack && rp.GetInventoryItem(rpItem.Id) != null)
                        {
                            if (user.Client != null)
                            {
                                user.SendWhisperChat(LanguageManager.TryGetValue("rp.itemown", user.Client.Language));
                            }
                        }
                        else
                        {
                            rp.AddInventoryItem(rpItem.Id);

                            if (user.Client != null)
                            {
                                user.SendWhisperChat(LanguageManager.TryGetValue("rp.itempick", user.Client.Language));
                            }
                        }
                    }

                    if (user.Client != null)
                    {
                        user.OnChat(LanguageManager.TryGetValue("rp.chat.takeitem", user.Client.Language));
                    }
                    room.RoomItemHandling.RemoveTempItem(itemTmp.Id);
                }
            }
        }

        foreach (var roomItem in coordinatedItems)
        {
            roomItem.UserWalksOffFurni(user, roomItem);

            if (roomItem.ItemData.InteractionType == InteractionType.GUILD_GATE)
            {
                roomItem.ExtraData = "0;" + roomItem.GroupId;
                roomItem.UpdateState(false);
            }
            else if (roomItem.ItemData.InteractionType is InteractionType.PRESSURE_PAD
                or InteractionType.TRAMPOLINE
                or InteractionType.TREADMILL
                or InteractionType.CROSSTRAINER)
            {
                roomItem.ExtraData = "0";
                roomItem.UpdateState(false);
            }
            else if (roomItem.ItemData.InteractionType == InteractionType.FOOTBALL)
            {
                if (!user.AllowMoveToRoller || roomItem.InteractionCountHelper > 0 || room.OldFoot)
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
        room.GameMap.RemoveTakingSquare(user.SetX, user.SetY);

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
        this._staffIds.Clear();
    }
}
