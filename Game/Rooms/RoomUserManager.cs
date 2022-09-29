using WibboEmulator.Communication.Packets.Outgoing.GameCenter;
using WibboEmulator.Communication.Packets.Outgoing.Handshake;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Avatar;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Permissions;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Session;

using WibboEmulator.Core;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Items;
using WibboEmulator.Game.Pets;
using WibboEmulator.Game.Quests;
using WibboEmulator.Game.Roleplay;
using WibboEmulator.Game.Roleplay.Enemy;
using WibboEmulator.Game.Roleplay.Player;
using WibboEmulator.Game.Rooms.AI;
using WibboEmulator.Game.Rooms.Games;
using WibboEmulator.Game.Rooms.Map.Movement;
using WibboEmulator.Game.Rooms.PathFinding;
using System.Collections.Concurrent;
using System.Drawing;

namespace WibboEmulator.Game.Rooms
{
    public delegate void UserAndItemDelegate(RoomUser user, Item item);

    public class RoomUserManager
    {
        private Room _room;
        private readonly ConcurrentDictionary<string, RoomUser> _usersByUsername;
        private readonly ConcurrentDictionary<int, RoomUser> _usersByUserID;

        private readonly ConcurrentDictionary<int, RoomUser> _users;
        private readonly ConcurrentDictionary<int, RoomUser> _pets;
        private readonly ConcurrentDictionary<int, RoomUser> _bots;

        private readonly List<int> _usersRank;

        private int _primaryPrivateUserID;
        public int BotPetCount => this._pets.Count + this._bots.Count;

        public event RoomEventDelegate OnUserEnter;

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

        public void UserEnter(RoomUser thisUser)
        {
            if (this.OnUserEnter != null)
            {
                this.OnUserEnter(thisUser, null);
            }
        }

        public int GetRoomUserCount()
        {
            return this._room.RoomData.UsersNow;
        }

        public RoomUser DeploySuperBot(RoomBot Bot)
        {
            int key = this._primaryPrivateUserID++;
            RoomUser roomUser = new RoomUser(0, this._room.Id, key, this._room);

            Bot.Id = -key;

            this._users.TryAdd(key, roomUser);

            roomUser.SetPos(Bot.X, Bot.Y, Bot.Z);
            roomUser.SetRot(Bot.Rot, false);

            roomUser.BotData = Bot;
            roomUser.BotAI = Bot.GenerateBotAI(roomUser.VirtualId);

            roomUser.BotAI.Init(Bot.Id, roomUser, this._room);

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
                this._bots.TryAdd(roomUser.BotData.Id, roomUser);
            }

            return roomUser;
        }

        public bool UpdateClientUsername(RoomUser User, string OldUsername, string NewUsername)
        {
            if (!this._usersByUsername.ContainsKey(OldUsername.ToLower()))
            {
                return false;
            }

            this._usersByUsername.TryRemove(OldUsername.ToLower(), out User);
            this._usersByUsername.TryAdd(NewUsername.ToLower(), User);
            return true;
        }

        public RoomUser DeployBot(RoomBot Bot, Pet PetData)
        {
            int key = this._primaryPrivateUserID++;
            RoomUser roomUser = new RoomUser(0, this._room.Id, key, this._room);

            this._users.TryAdd(key, roomUser);

            roomUser.SetPos(Bot.X, Bot.Y, Bot.Z);
            roomUser.SetRot(Bot.Rot, false);

            roomUser.BotData = Bot;

            if (this._room.IsRoleplay)
            {
                RPEnemy Enemy;
                if (Bot.IsPet)
                {
                    Enemy = WibboEnvironment.GetGame().GetRoleplayManager().GetEnemyManager().GetEnemyPet(Bot.Id);
                }
                else
                {
                    Enemy = WibboEnvironment.GetGame().GetRoleplayManager().GetEnemyManager().GetEnemyBot(Bot.Id);
                }

                if (Enemy != null)
                {
                    roomUser.BotData.RoleBot = new RoleBot(Enemy);
                    if (Bot.IsPet)
                    {
                        roomUser.BotData.AiType = BotAIType.RoleplayPet;
                    }
                    else
                    {
                        roomUser.BotData.AiType = BotAIType.RoleplayBot;
                    }
                }
            }

            roomUser.BotAI = Bot.GenerateBotAI(roomUser.VirtualId);

            if (roomUser.IsPet)
            {
                roomUser.BotAI.Init(Bot.Id, roomUser, this._room);
                roomUser.PetData = PetData;
                roomUser.PetData.VirtualId = roomUser.VirtualId;
            }
            else
            {
                roomUser.BotAI.Init(Bot.Id, roomUser, this._room);
            }

            roomUser.SetStatus("flatctrl", "4");

            if (Bot.Status == 1)
            {
                roomUser.SetStatus("sit", "0.5");
                roomUser.IsSit = true;
            }

            if (Bot.Status == 2)
            {
                roomUser.SetStatus("lay", "0.7");
                roomUser.IsLay = true;
            }

            this.UpdateUserStatus(roomUser, false);
            roomUser.UpdateNeeded = true;

            if (Bot.IsDancing)
            {
                roomUser.DanceId = 3;
                this._room.SendPacket(new DanceComposer(roomUser.VirtualId, 3));
            }

            if (Bot.Enable > 0)
            {
                roomUser.ApplyEffect(Bot.Enable);
            }

            if (Bot.Handitem > 0)
            {
                roomUser.CarryItem(Bot.Handitem, true);
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
                    this._pets.TryAdd(roomUser.PetData.PetId, roomUser);
                }
            }
            else if (this._bots.ContainsKey(roomUser.BotData.Id))
            {
                this._bots[roomUser.BotData.Id] = roomUser;
            }
            else
            {
                this._bots.TryAdd(roomUser.BotData.Id, roomUser);
            }

            return roomUser;
        }

        public void RemoveBot(int VirtualId, bool Kicked)
        {
            RoomUser roomUserByVirtualId = this.GetRoomUserByVirtualId(VirtualId);
            if (roomUserByVirtualId == null || !roomUserByVirtualId.IsBot)
            {
                return;
            }

            if (roomUserByVirtualId.IsPet)
            {
                this._pets.TryRemove(roomUserByVirtualId.PetData.PetId, out RoomUser PetRemoval);
            }
            else
            {
                this._bots.TryRemove(roomUserByVirtualId.BotData.Id, out RoomUser BotRemoval);
            }

            roomUserByVirtualId.BotAI.OnSelfLeaveRoom(Kicked);

            this._room.SendPacket(new UserRemoveComposer(roomUserByVirtualId.VirtualId));

            this._room.GetGameMap().RemoveTakingSquare(roomUserByVirtualId.SetX, roomUserByVirtualId.SetY);
            this._room.GetGameMap().RemoveUserFromMap(roomUserByVirtualId, new Point(roomUserByVirtualId.X, roomUserByVirtualId.Y));

            this._users.TryRemove(roomUserByVirtualId.VirtualId, out RoomUser toRemove);

        }

        private void UpdateUserEffect(RoomUser User, int x, int y)
        {
            if (User == null)
            {
                return;
            }

            if (User.IsPet)
            {
                return;
            }

            if (!this._room.GetGameMap().ValidTile(x, y))
            {
                return;
            }

            byte pByte = this._room.GetGameMap().EffectMap[x, y];
            if (pByte > 0)
            {
                ItemEffectType itemEffectType = ByteToItemEffectType.Parse(pByte);
                if (itemEffectType == User.CurrentItemEffect)
                {
                    return;
                }

                switch (itemEffectType)
                {
                    case ItemEffectType.NONE:
                        User.ApplyEffect(0);
                        User.CurrentItemEffect = itemEffectType;
                        break;
                    case ItemEffectType.SWIM:
                        User.ApplyEffect(29);
                        if (User.GetClient() != null)
                        {
                            WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(User.GetClient(), QuestType.EXPLORE_FIND_ITEM, 1948);
                        }

                        User.CurrentItemEffect = itemEffectType;
                        break;
                    case ItemEffectType.SWIMLOW:
                        User.ApplyEffect(30);
                        User.CurrentItemEffect = itemEffectType;
                        break;
                    case ItemEffectType.SWIMHALLOWEEN:
                        User.ApplyEffect(37);
                        User.CurrentItemEffect = itemEffectType;
                        break;
                    case ItemEffectType.ICESKATES:
                        if (User.GetClient() != null)
                        {
                            if (User.GetClient().GetUser().Gender == "M")
                            {
                                User.ApplyEffect(38);
                            }
                            else
                            {
                                User.ApplyEffect(39);
                            }
                        }
                        else
                        {
                            User.ApplyEffect(38);
                        }

                        User.CurrentItemEffect = ItemEffectType.ICESKATES;
                        if (User.GetClient() != null)
                        {
                            WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(User.GetClient(), QuestType.EXPLORE_FIND_ITEM, 1413);
                        }

                        break;
                    case ItemEffectType.NORMALSKATES:
                        if (User.GetClient() != null)
                        {
                            if (User.GetClient().GetUser().Gender == "M")
                            {
                                User.ApplyEffect(55);
                            }
                            else
                            {
                                User.ApplyEffect(56);
                            }
                        }
                        else
                        {
                            User.ApplyEffect(55);
                        }

                        User.CurrentItemEffect = itemEffectType;
                        if (User.GetClient() != null)
                        {
                            WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(User.GetClient(), QuestType.EXPLORE_FIND_ITEM, 2199);
                        }

                        break;
                    case ItemEffectType.TRAMPOLINE:
                        User.ApplyEffect(193);
                        User.CurrentItemEffect = itemEffectType;
                        break;
                    case ItemEffectType.TREADMILL:
                        User.ApplyEffect(194);
                        User.CurrentItemEffect = itemEffectType;
                        break;
                    case ItemEffectType.CROSSTRAINER:
                        User.ApplyEffect(195);
                        User.CurrentItemEffect = itemEffectType;
                        break;

                }
            }
            else
            {
                if (User.CurrentItemEffect == ItemEffectType.NONE || pByte != 0)
                {
                    return;
                }

                User.ApplyEffect(0);
                User.CurrentItemEffect = ItemEffectType.NONE;
            }
        }

        public List<RoomUser> GetUsersForSquare(int x, int y)
        {
            return this._room.GetGameMap().GetRoomUsers(new Point(x, y)).OrderBy(u => u.IsBot == true).ToList();
        }

        public RoomUser GetUserForSquare(int x, int y)
        {
            return Enumerable.FirstOrDefault<RoomUser>(this._room.GetGameMap().GetRoomUsers(new Point(x, y)).OrderBy(u => u.IsBot == true));
        }

        public RoomUser GetUserForSquareNotBot(int x, int y)
        {
            return Enumerable.FirstOrDefault<RoomUser>(this._room.GetGameMap().GetRoomUsers(new Point(x, y)).Where(u => u.IsBot == false));
        }

        public bool AddAvatarToRoom(Client Session)
        {
            if (this._room == null)
            {
                return false;
            }

            if (Session == null || Session.GetUser() == null)
            {
                return false;
            }

            int PersonalID = this._primaryPrivateUserID++;

            RoomUser User = new RoomUser(Session.GetUser().Id, this._room.Id, PersonalID, this._room)
            {
                IsSpectator = Session.GetUser().SpectatorMode
            };

            if (!this._users.TryAdd(PersonalID, User))
            {
                return false;
            }

            if (Session.GetUser().Rank > 5 && !this._usersRank.Contains(User.UserId))
            {
                this._usersRank.Add(User.UserId);
            }

            Session.GetUser().CurrentRoomId = this._room.Id;
            Session.GetUser().LoadingRoomId = 0;

            string Username = Session.GetUser().Username;
            int UserId = Session.GetUser().Id;

            if (this._usersByUsername.ContainsKey(Username.ToLower()))
            {
                this._usersByUsername.TryRemove(Username.ToLower(), out User);
            }

            if (this._usersByUserID.ContainsKey(UserId))
            {
                this._usersByUserID.TryRemove(UserId, out User);
            }

            this._usersByUsername.TryAdd(Username.ToLower(), User);
            this._usersByUserID.TryAdd(UserId, User);

            RoomModelDynamic Model = this._room.GetGameMap().Model;
            if (Model == null)
            {
                return false;
            }

            User.SetPos(Model.DoorX, Model.DoorY, Model.DoorZ);
            User.SetRot(Model.DoorOrientation, false);

            if (Session.GetUser().IsTeleporting)
            {
                Item roomItem = this._room.GetRoomItemHandler().GetItem(User.GetClient().GetUser().TeleporterId);
                if (roomItem != null)
                {
                    roomItem.GetRoom().GetGameMap().TeleportToItem(User, roomItem);

                    roomItem.InteractingUser2 = Session.GetUser().Id;
                    roomItem.ReqUpdate(1);
                }
            }

            if (User.GetClient() != null && User.GetClient().GetUser() != null)
            {
                User.GetClient().GetUser().IsTeleporting = false;
                User.GetClient().GetUser().TeleporterId = 0;
                User.GetClient().GetUser().TeleportingRoomID = 0;
            }

            if (!User.IsSpectator)
            {
                this._room.SendPacket(new UsersComposer(User));
            }

            if (User.IsSpectator)
            {
                List<RoomUser> roomUserByRank = this._room.GetRoomUserManager().GetStaffRoomUser();
                if (roomUserByRank.Count > 0)
                {
                    foreach (RoomUser StaffUser in roomUserByRank)
                    {
                        if (StaffUser != null && StaffUser.GetClient() != null && (StaffUser.GetClient().GetUser() != null && StaffUser.GetClient().GetUser().HasPermission("perm_show_invisible")))
                        {
                            StaffUser.SendWhisperChat(User.GetUsername() + " est entré dans l'appart en mode invisible !", true);
                        }
                    }
                }
            }

            if(Session.GetUser().HasPermission("perm_owner_all_rooms"))
            {
                User.SetStatus("flatctrl", "5");
                Session.SendPacket(new YouAreOwnerComposer());
                Session.SendPacket(new YouAreControllerComposer(5));
            }
            else if (this._room.CheckRights(Session, true))
            {
                User.SetStatus("flatctrl", "4");
                Session.SendPacket(new YouAreOwnerComposer());
                Session.SendPacket(new YouAreControllerComposer(4));
            }
            else if (this._room.CheckRights(Session))
            {
                User.SetStatus("flatctrl", "1");
                Session.SendPacket(new YouAreControllerComposer(1));
            }
            else
            {
                User.RemoveStatus("flatctrl");
                Session.SendPacket(new YouAreNotControllerComposer());
            }

            if (!User.IsBot)
            {
                if (Session.GetUser().GetBadgeComponent().HasBadgeSlot("ADM")) // STAFF
                {
                    User.CurrentEffect = 540;
                }
                else if (Session.GetUser().GetBadgeComponent().HasBadgeSlot("PRWRD1")) // PROWIRED
                {
                    User.CurrentEffect = 580;
                }
                else if (Session.GetUser().GetBadgeComponent().HasBadgeSlot("GPHWIB")) // GRAPHISTE
                {
                    User.CurrentEffect = 557;
                }
                else if (Session.GetUser().GetBadgeComponent().HasBadgeSlot("wibbo.helpeur")) // HELPEUR
                {
                    User.CurrentEffect = 544;
                }
                else if (Session.GetUser().GetBadgeComponent().HasBadgeSlot("WIBARC")) // ARCHI
                {
                    User.CurrentEffect = 546;
                }
                else if (Session.GetUser().GetBadgeComponent().HasBadgeSlot("CRPOFFI")) // CROUPIER
                {
                    User.CurrentEffect = 570;
                }
                else if (Session.GetUser().GetBadgeComponent().HasBadgeSlot("ZEERSWS")) // WIBBOSTATIONORIGINERADIO
                {
                    User.CurrentEffect = 552;
                }
                else if (Session.GetUser().GetBadgeComponent().HasBadgeSlot("WBASSO")) // ASSOCIER
                {
                    User.CurrentEffect = 576;
                }
                else if (Session.GetUser().GetBadgeComponent().HasBadgeSlot("WIBBOCOM")) // AGENT DE COMMUNICATION
                {
                    User.CurrentEffect = 581;
                }

                if (User.CurrentEffect > 0)
                {
                    this._room.SendPacket(new AvatarEffectComposer(User.VirtualId, User.CurrentEffect));
                }
            }

            User.UpdateNeeded = true;

            foreach (RoomUser Bot in this._bots.Values.ToList())
            {
                if (Bot == null || Bot.BotAI == null)
                {
                    continue;
                }

                Bot.BotAI.OnUserEnterRoom(User);
            }

            if (!User.IsBot && this._room.RoomData.OwnerName != User.GetClient().GetUser().Username)
            {
                WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(User.GetClient(), QuestType.SOCIAL_VISIT, 0);
            }

            if (!User.IsBot)
            {
                if (Session.GetUser().RolePlayId > 0 && this._room.RoomData.OwnerId != Session.GetUser().RolePlayId)
                {
                    RolePlayerManager RPManager = WibboEnvironment.GetGame().GetRoleplayManager().GetRolePlay(Session.GetUser().RolePlayId);
                    if (RPManager != null)
                    {
                        RolePlayer Rp = RPManager.GetPlayer(Session.GetUser().Id);
                        if (Rp != null)
                        {
                            RPManager.RemovePlayer(Session.GetUser().Id);
                        }
                    }
                    Session.GetUser().RolePlayId = 0;
                }

                if (this._room.IsRoleplay && this._room.RoomData.OwnerId != Session.GetUser().RolePlayId)
                {
                    RolePlayerManager RPManager = WibboEnvironment.GetGame().GetRoleplayManager().GetRolePlay(this._room.RoomData.OwnerId);
                    if (RPManager != null)
                    {
                        RolePlayer Rp = RPManager.GetPlayer(Session.GetUser().Id);
                        if (Rp == null)
                        {
                            RPManager.AddPlayer(Session.GetUser().Id);
                        }
                    }

                    Session.GetUser().RolePlayId = this._room.RoomData.OwnerId;
                }
            }


            User.InGame = this._room.IsRoleplay;

            return true;
        }

        public void RemoveUserFromRoom(Client Session, bool NotifyClient, bool NotifyKick)
        {
            try
            {
                if (Session == null)
                {
                    return;
                }

                if (Session.GetUser() == null)
                {
                    return;
                }

                if (NotifyClient)
                {
                    if (NotifyKick)
                    {
                        Session.SendPacket(new GenericErrorComposer(4008));
                    }
                    Session.SendPacket(new CloseConnectionComposer());
                }

                RoomUser User = this.GetRoomUserByUserId(Session.GetUser().Id);
                if (User == null)
                {
                    return;
                }

                if (this._usersRank.Contains(User.UserId))
                {
                    this._usersRank.Remove(User.UserId);
                }

                if (User.Team != TeamType.NONE)
                {
                    this._room.GetTeamManager().OnUserLeave(User);
                    this._room.GetGameManager().UpdateGatesTeamCounts();

                    Session.SendPacket(new IsPlayingComposer(false));
                }

                if (this._room.GotJanken())
                {
                    this._room.GetJanken().RemovePlayer(User);
                }

                if (User.RidingHorse)
                {
                    User.RidingHorse = false;
                    RoomUser roomUserByVirtualId = this.GetRoomUserByVirtualId(User.HorseID);
                    if (roomUserByVirtualId != null)
                    {
                        roomUserByVirtualId.RidingHorse = false;
                        roomUserByVirtualId.HorseID = 0;
                    }
                }

                if (User.IsSit || User.IsLay)
                {
                    User.IsSit = false;
                    User.IsLay = false;
                }

                if (this._room.HasActiveTrade(Session.GetUser().Id))
                {
                    this._room.TryStopTrade(Session.GetUser().Id);
                }

                if (User.Roleplayer != null)
                {
                    WibboEnvironment.GetGame().GetRoleplayManager().GetTrocManager().RemoveTrade(User.Roleplayer.TradeId);
                }

                if (User.IsSpectator)
                {
                    List<RoomUser> roomUserByRank = this._room.GetRoomUserManager().GetStaffRoomUser();
                    if (roomUserByRank.Count > 0)
                    {
                        foreach (RoomUser StaffUser in roomUserByRank)
                        {
                            if (StaffUser != null && StaffUser.GetClient() != null && (StaffUser.GetClient().GetUser() != null && StaffUser.GetClient().GetUser().HasPermission("perm_show_invisible")))
                            {
                                StaffUser.SendWhisperChat(User.GetUsername() + " était en mode invisible. Il vient de partir de l'appartement.", true);
                            }
                        }
                    }
                }

                Session.GetUser().CurrentRoomId = 0;
                Session.GetUser().LoadingRoomId = 0;

                Session.GetUser().ForceUse = -1;

                this._usersByUserID.TryRemove(User.UserId, out User);
                this._usersByUsername.TryRemove(Session.GetUser().Username.ToLower(), out User);

                this.RemoveRoomUser(User);

                User.Freeze = true;
                User.FreezeEndCounter = 0;
                User.Dispose();
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogCriticalException("Error during removing user (" + Session.ConnectionID + ") from room:" + (ex).ToString());
            }
        }

        private void RemoveRoomUser(RoomUser user)
        {
            this._room.GetGameMap().RemoveTakingSquare(user.SetX, user.SetY);
            this._room.GetGameMap().RemoveUserFromMap(user, new Point(user.X, user.Y));

            this._room.SendPacket(new UserRemoveComposer(user.VirtualId));

            this._users.TryRemove(user.VirtualId, out RoomUser toRemove);
        }

        public void UpdateUserCount(int count)
        {
            if (this._room.RoomData.UsersNow == count)
            {
                return;
            }

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                RoomDao.UpdateUsersNow(dbClient, this._room.Id, count);
            }

            this._room.RoomData.UsersNow = count;
        }

        public RoomUser GetRoomUserByVirtualId(int VirtualId)
        {
            if (!this._users.TryGetValue(VirtualId, out RoomUser User))
            {
                return null;
            }

            return User;
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

        public RoomUser GetUserByTracker(string IPWeb, string MachineId)
        {
            foreach (RoomUser User in this.GetUserList())
            {
                if (User == null)
                {
                    continue;
                }

                if (User.GetClient() == null)
                {
                    continue;
                }

                if (User.GetClient().GetUser() == null)
                {
                    continue;
                }

                if (User.GetClient().GetConnection() == null)
                {
                    continue;
                }

                if (User.GetClient().MachineId != MachineId)
                {
                    continue;
                }

                if (User.GetClient().GetUser().IP != IPWeb)
                {
                    continue;
                }

                return User;
            }

            return null;
        }

        public List<RoomUser> GetRoomUsers()
        {
            List<RoomUser> List = new List<RoomUser>();

            List = this.GetUserList().Where(x => (!x.IsBot)).ToList();

            return List;
        }

        public ICollection<RoomUser> GetUserList()
        {
            return this._users.Values;
        }

        public RoomUser GetBotByName(string name)
        {
            return this._bots.Values.Where(b => b.IsBot && b.BotData.Name == name).FirstOrDefault();
        }

        public RoomUser GetBotOrPetByName(string name)
        {
            return this._bots.Values.Concat(this._pets.Values).Where(b => (b.IsBot && b.BotData.Name == name) || (b.IsPet && b.BotData.Name == name)).FirstOrDefault();
        }

        public List<RoomUser> GetStaffRoomUser()
        {
            List<RoomUser> list = new List<RoomUser>();
            foreach (int UserId in this._usersRank)
            {
                RoomUser roomUser = this.GetRoomUserByUserId(UserId);
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
            List<RoomUser> botList = this.GetBots();
            if (botList.Count <= 0)
            {
                return;
            }

            BotUserDao.SaveBots(dbClient, botList);
        }

        public void SavePets(IQueryAdapter dbClient)
        {
            List<RoomUser> Petlist = this.GetPets();
            if (Petlist.Count <= 0)
            {
                return;
            }

            BotPetDao.SavePet(dbClient, Petlist);
        }

        public List<RoomUser> GetBots()
        {
            List<RoomUser> Bots = new List<RoomUser>();
            foreach (RoomUser User in this._bots.Values.ToList())
            {
                if (User == null || !User.IsBot || User.IsPet)
                {
                    continue;
                }

                Bots.Add(User);
            }

            return Bots;
        }

        public List<RoomUser> GetPets()
        {
            List<RoomUser> Pets = new List<RoomUser>();
            foreach (RoomUser User in this._pets.Values.ToList())
            {
                if (User == null || !User.IsPet)
                {
                    continue;
                }

                Pets.Add(User);
            }

            return Pets;
        }

        public void SerializeStatusUpdates()
        {
            List<RoomUser> Users = new List<RoomUser>();
            ICollection<RoomUser> RoomUsers = this.GetUserList();

            if (RoomUsers == null)
            {
                return;
            }

            foreach (RoomUser User in RoomUsers.ToList())
            {
                if (User == null || !User.UpdateNeeded)
                {
                    continue;
                }

                User.UpdateNeeded = false;
                Users.Add(User);
            }

            if (Users.Count > 0)
            {
                this._room.SendPacket(new UserUpdateComposer(Users));
            }
        }

        public void UpdateUserStatusses()
        {
            foreach (RoomUser User in this.GetUserList().ToList())
            {
                this.UpdateUserStatus(User, false);
            }
        }

        private bool IsValid(RoomUser user)
        {
            return user.IsBot || user.GetClient() != null && user.GetClient().GetUser() != null && user.GetClient().GetUser().CurrentRoomId == this._room.Id;
        }

        public bool TryGetPet(int PetId, out RoomUser Pet)
        {
            return this._pets.TryGetValue(PetId, out Pet);
        }

        public bool TryGetBot(int BotId, out RoomUser Bot)
        {
            return this._bots.TryGetValue(BotId, out Bot);
        }

        public void UpdateUserStatus(RoomUser User, bool cyclegameitems)
        {
            if (User == null)
            {
                return;
            }

            if (User.ContainStatus("lay") || User.ContainStatus("sit") || User.ContainStatus("sign"))
            {
                if (User.ContainStatus("lay"))
                {
                    User.RemoveStatus("lay");
                }

                if (User.ContainStatus("sit"))
                {
                    User.RemoveStatus("sit");
                }

                if (User.ContainStatus("sign"))
                {
                    User.RemoveStatus("sign");
                }

                User.UpdateNeeded = true;
            }

            List<Item> roomItemForSquare = this._room.GetGameMap().GetCoordinatedItems(new Point(User.X, User.Y)).OrderBy(p => p.Z).ToList();

            double newZ = !User.RidingHorse || User.IsPet ? this._room.GetGameMap().SqAbsoluteHeight(User.X, User.Y, roomItemForSquare) : this._room.GetGameMap().SqAbsoluteHeight(User.X, User.Y, roomItemForSquare) + 1.0;
            if (newZ != User.Z)
            {
                User.Z = newZ;
                User.UpdateNeeded = true;
            }

            foreach (Item roomItem in roomItemForSquare)
            {
                if (cyclegameitems)
                {
                    roomItem.UserWalksOnFurni(User, roomItem);

                    if (roomItem.EffectId != 0 && !User.IsBot)
                    {
                        User.ApplyEffect(roomItem.EffectId);
                    }
                }

                if (roomItem.GetBaseItem().IsSeat)
                {
                    if (!User.ContainStatus("sit"))
                    {
                        User.SetStatus("sit", roomItem.Height.ToString());
                        User.IsSit = true;
                    }
                    User.Z = roomItem.Z;
                    User.RotHead = roomItem.Rotation;
                    User.RotBody = roomItem.Rotation;
                    User.UpdateNeeded = true;
                }

                switch (roomItem.GetBaseItem().InteractionType)
                {
                    case InteractionType.BED:
                        if (!User.ContainStatus("lay"))
                        {
                            User.SetStatus("lay", roomItem.Height.ToString() + " null");
                            User.IsLay = true;
                        }
                        User.Z = roomItem.Z;
                        User.RotHead = roomItem.Rotation;
                        User.RotBody = roomItem.Rotation;
                        User.UpdateNeeded = true;
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
                        if (!cyclegameitems || User.IsBot)
                        {
                            break;
                        }

                        if (roomItem.InteractingUser != 0)
                        {
                            break;
                        }

                        User.CanWalk = true;
                        roomItem.InteractingUser = User.GetClient().GetUser().Id;
                        roomItem.ReqUpdate(2);
                        break;
                    case InteractionType.BANZAIGATEBLUE:
                    case InteractionType.BANZAIGATERED:
                    case InteractionType.BANZAIGATEYELLOW:
                    case InteractionType.BANZAIGATEGREEN:
                        if (cyclegameitems && !User.IsBot)
                        {
                            int EffectId = ((int)roomItem.Team + 32);
                            TeamManager managerForBanzai = this._room.GetTeamManager();
                            if (User.Team != roomItem.Team)
                            {
                                if (User.Team != TeamType.NONE)
                                {
                                    managerForBanzai.OnUserLeave(User);
                                }

                                User.Team = roomItem.Team;
                                managerForBanzai.AddUser(User);

                                this._room.GetGameManager().UpdateGatesTeamCounts();
                                if (User.CurrentEffect != EffectId)
                                {
                                    User.ApplyEffect(EffectId);
                                }

                                if (User.GetClient() != null)
                                {
                                    User.GetClient().SendPacket(new IsPlayingComposer(true));
                                }
                            }
                            else
                            {
                                managerForBanzai.OnUserLeave(User);
                                this._room.GetGameManager().UpdateGatesTeamCounts();
                                if (User.CurrentEffect == EffectId)
                                {
                                    User.ApplyEffect(0);
                                }

                                if (User.GetClient() != null)
                                {
                                    User.GetClient().SendPacket(new IsPlayingComposer(false));
                                }

                                User.Team = TeamType.NONE;
                                continue;
                            }
                        }
                        break;
                    case InteractionType.BANZAIBLO:
                        if (cyclegameitems && User.Team != TeamType.NONE && !User.IsBot)
                        {
                            this._room.GetGameItemHandler().OnWalkableBanzaiBlo(User, roomItem);
                        }
                        break;
                    case InteractionType.BANZAIBLOB:
                        if (cyclegameitems && User.Team != TeamType.NONE && !User.IsBot)
                        {
                            this._room.GetGameItemHandler().OnWalkableBanzaiBlob(User, roomItem);
                        }
                        break;
                    case InteractionType.BANZAITELE:
                        if (cyclegameitems)
                        {
                            this._room.GetGameItemHandler().OnTeleportRoomUserEnter(User, roomItem);
                        }

                        break;
                    case InteractionType.FREEZEYELLOWGATE:
                    case InteractionType.FREEZEREDGATE:
                    case InteractionType.FREEZEGREENGATE:
                    case InteractionType.FREEZEBLUEGATE:
                        if (cyclegameitems && !User.IsBot)
                        {
                            int EffectId = ((int)roomItem.Team + 39);
                            TeamManager managerForFreeze = this._room.GetTeamManager();
                            if (User.Team != roomItem.Team)
                            {
                                if (User.Team != TeamType.NONE)
                                {
                                    managerForFreeze.OnUserLeave(User);
                                }

                                User.Team = roomItem.Team;
                                managerForFreeze.AddUser(User);
                                this._room.GetGameManager().UpdateGatesTeamCounts();
                                if (User.CurrentEffect != EffectId)
                                {
                                    User.ApplyEffect(EffectId);
                                }

                                if (User.GetClient() != null)
                                {
                                    User.GetClient().SendPacket(new IsPlayingComposer(true));
                                }
                            }
                            else
                            {
                                managerForFreeze.OnUserLeave(User);
                                this._room.GetGameManager().UpdateGatesTeamCounts();
                                if (User.CurrentEffect == EffectId)
                                {
                                    User.ApplyEffect(0);
                                }

                                if (User.GetClient() != null)
                                {
                                    User.GetClient().SendPacket(new IsPlayingComposer(false));
                                }

                                User.Team = TeamType.NONE;
                            }
                        }
                        break;
                    case InteractionType.FBGATE:
                        if (cyclegameitems || string.IsNullOrEmpty(roomItem.ExtraData) || !roomItem.ExtraData.Contains(',') || User == null || User.IsBot || User.IsTransf || User.IsSpectator)
                        {
                            break;
                        }

                        if (User.GetClient().GetUser().LastMovFGate && User.GetClient().GetUser().BackupGender == User.GetClient().GetUser().Gender)
                        {
                            User.GetClient().GetUser().LastMovFGate = false;
                            User.GetClient().GetUser().Look = User.GetClient().GetUser().BackupLook;
                        }
                        else
                        {
                            // mini Fix
                            string _gateLook = ((User.GetClient().GetUser().Gender.ToUpper() == "M") ? roomItem.ExtraData.Split(',')[0] : roomItem.ExtraData.Split(',')[1]);
                            if (_gateLook == "")
                            {
                                break;
                            }

                            string gateLook = "";
                            foreach (string part in _gateLook.Split('.'))
                            {
                                if (part.StartsWith("hd"))
                                {
                                    continue;
                                }

                                gateLook += part + ".";
                            }
                            gateLook = gateLook.Substring(0, gateLook.Length - 1);

                            // Generating New Look.
                            string[] Parts = User.GetClient().GetUser().Look.Split('.');
                            string NewLook = "";
                            foreach (string Part in Parts)
                            {
                                if (/*Part.StartsWith("hd") || */Part.StartsWith("sh") || Part.StartsWith("cp") || Part.StartsWith("cc") || Part.StartsWith("ch") || Part.StartsWith("lg") || Part.StartsWith("ca") || Part.StartsWith("wa"))
                                {
                                    continue;
                                }

                                NewLook += Part + ".";
                            }
                            NewLook += gateLook;

                            User.GetClient().GetUser().BackupLook = User.GetClient().GetUser().Look;
                            User.GetClient().GetUser().BackupGender = User.GetClient().GetUser().Gender;
                            User.GetClient().GetUser().Look = NewLook;
                            User.GetClient().GetUser().LastMovFGate = true;
                        }

                        User.GetClient().SendPacket(new UserChangeComposer(User, true));

                        if (User.GetClient().GetUser().InRoom)
                        {
                            this._room.SendPacket(new UserChangeComposer(User, false));
                        }
                        break;
                    case InteractionType.FREEZETILEBLOCK:
                        if (!cyclegameitems)
                        {
                            break;
                        }

                        this._room.GetFreeze().OnWalkFreezeBlock(roomItem, User);
                        break;
                    default:
                        break;
                }
            }
            if (cyclegameitems)
            {
                this._room.GetBanzai().HandleBanzaiTiles(User.Coordinate, User.Team, User);
            }

            if (User.IsSit || User.IsLay)
            {
                if (User.IsSit)
                {
                    if (!User.ContainStatus("sit"))
                    {
                        if (User.IsTransf)
                        {
                            User.SetStatus("sit", "0");
                        }
                        else
                        {
                            User.SetStatus("sit", "0.5");
                        }

                        User.UpdateNeeded = true;
                    }

                }
                else if (User.IsLay)
                {

                    if (!User.ContainStatus("lay"))
                    {
                        if (User.IsTransf)
                        {
                            User.SetStatus("lay", "0");
                        }
                        else
                        {
                            User.SetStatus("lay", "0.7");
                        }

                        User.UpdateNeeded = true;
                    }

                }
            }
        }

        public void OnCycle(ref int idleCount)
        {
            int userCounter = 0;

            List<RoomUser> ToRemove = new List<RoomUser>();

            foreach (RoomUser User in this.GetUserList().ToList())
            {
                if (!this.IsValid(User))
                {
                    if (User.GetClient() != null && User.GetClient().GetUser() != null)
                    {
                        this.RemoveUserFromRoom(User.GetClient(), false, false);
                    }
                    else
                    {
                        this.RemoveRoomUser(User);
                    }
                }

                if (User.IsDispose)
                {
                    continue;
                }

                if (User.RidingHorse && User.IsPet)
                {
                    continue;
                }

                if (this._room.IsRoleplay)
                {
                    RolePlayerManager RPManager = WibboEnvironment.GetGame().GetRoleplayManager().GetRolePlay(this._room.RoomData.OwnerId);
                    if (RPManager != null)
                    {
                        if (User.IsBot)
                        {
                            if (User.BotData.RoleBot != null)
                            {
                                User.BotData.RoleBot.OnCycle(User, this._room);
                            }
                        }
                        else
                        {
                            RolePlayer Rp = User.Roleplayer;
                            if (Rp != null)
                            {
                                Rp.OnCycle(User, RPManager);
                            }
                        }
                    }
                }

                User.IdleTime++;

                if (!User.IsAsleep && User.IdleTime >= 600 && !User.IsBot)
                {
                    User.IsAsleep = true;
                    this._room.SendPacket(new SleepComposer(User.VirtualId, true));
                }

                if (User.CarryItemID > 0 && User.CarryTimer > 0)
                {
                    User.CarryTimer--;
                    if (User.CarryTimer <= 0)
                    {
                        User.CarryItem(0);
                    }
                }

                if (User.UserTimer > 0)
                {
                    User.UserTimer--;
                }

                if (User.FreezeEndCounter > 0)
                {
                    User.FreezeEndCounter--;
                    if (User.FreezeEndCounter <= 0)
                    {
                        User.Freeze = false;
                    }
                }

                if (User.TimerResetEffect > 0)
                {
                    User.TimerResetEffect--;
                    if (User.TimerResetEffect <= 0)
                    {
                        User.ApplyEffect(User.CurrentEffect, true);
                    }
                }

                if (this._room.GotFreeze())
                {
                    this._room.GetFreeze().CycleUser(User);
                }

                if (User.SetStep)
                {
                    if (this.SetStepForUser(User))
                    {
                        continue;
                    }

                    if (User.RidingHorse && !User.IsPet)
                    {
                        RoomUser roomUserByVirtualId = this.GetRoomUserByVirtualId(Convert.ToInt32(User.HorseID));
                        if (this.SetStepForUser(roomUserByVirtualId))
                        {
                            continue;
                        }
                    }
                }
                else
                {
                    User.AllowMoveToRoller = true;
                    User.AllowBall = true;
                    User.MoveWithBall = false;
                }

                if (User.IsWalking && !User.Freezed && !User.Freeze && !(this._room.FreezeRoom && (User.GetClient() != null && User.GetClient().GetUser().Rank < 6)))
                {
                    this.CalculatePath(User);

                    User.UpdateNeeded = true;
                    if (User.RidingHorse && !User.IsPet)
                    {
                        RoomUser roomUserByVirtualId = this.GetRoomUserByVirtualId(Convert.ToInt32(User.HorseID));
                        roomUserByVirtualId.UpdateNeeded = true;
                    }
                }
                else if (User.ContainStatus("mv"))
                {
                    User.RemoveStatus("mv");
                    User.IsWalking = false;
                    User.UpdateNeeded = true;

                    if (User.RidingHorse && !User.IsPet)
                    {
                        RoomUser roomUserByVirtualId = this.GetRoomUserByVirtualId(Convert.ToInt32(User.HorseID));
                        roomUserByVirtualId.RemoveStatus("mv");
                        roomUserByVirtualId.IsWalking = false;
                        roomUserByVirtualId.UpdateNeeded = true;
                    }
                }

                if (User.IsBot && User.BotAI != null)
                {
                    User.BotAI.OnTimerTick();
                }
                else if (!User.IsSpectator)
                {
                    userCounter++;
                }
            }

            if (userCounter == 0)
            {
                idleCount++;
            }

            foreach (RoomUser user in ToRemove)
            {
                Client clientByUserId = WibboEnvironment.GetGame().GetClientManager().GetClientByUserID(user.UserId);
                if (clientByUserId != null)
                {
                    this.RemoveUserFromRoom(clientByUserId, true, false);
                }
                else
                {
                    this.RemoveRoomUser(user);
                }
            }
            ToRemove.Clear();

            this.UpdateUserCount(userCounter);
        }

        private void CalculatePath(RoomUser User)
        {
            Gamemap gameMap = this._room.GetGameMap();

            SquarePoint nextStep = Pathfinder.GetNextStep(User.X, User.Y, User.GoalX, User.GoalY, gameMap.GameMap, gameMap.ItemHeightMap, gameMap.UserOnMap, gameMap.SquareTaking, gameMap.Model.MapSizeX, gameMap.Model.MapSizeY, User.AllowOverride, gameMap.DiagonalEnabled, this._room.RoomData.AllowWalkthrough, gameMap.ObliqueDisable);
            if (User.WalkSpeed)
            {
                nextStep = Pathfinder.GetNextStep(nextStep.X, nextStep.Y, User.GoalX, User.GoalY, gameMap.GameMap, gameMap.ItemHeightMap, gameMap.UserOnMap, gameMap.SquareTaking, gameMap.Model.MapSizeX, gameMap.Model.MapSizeY, User.AllowOverride, gameMap.DiagonalEnabled, this._room.RoomData.AllowWalkthrough, gameMap.ObliqueDisable);
            }

            if (User.BreakWalkEnable && User.StopWalking)
            {
                User.StopWalking = false;
                this.UpdateUserStatus(User, false);
                User.RemoveStatus("mv");

                if (User.RidingHorse && !User.IsPet)
                {
                    RoomUser roomUserByVirtualId = this.GetRoomUserByVirtualId(Convert.ToInt32(User.HorseID));
                    roomUserByVirtualId.IsWalking = false;
                    this.UpdateUserStatus(roomUserByVirtualId, false);
                    roomUserByVirtualId.RemoveStatus("mv");
                }
            }
            else if (nextStep.X == User.X && nextStep.Y == User.Y || this._room.GetGameItemHandler().CheckGroupGate(User, new Point(nextStep.X, nextStep.Y)))
            {
                User.IsWalking = false;
                this.UpdateUserStatus(User, false);
                User.RemoveStatus("mv");

                if (User.RidingHorse && !User.IsPet)
                {
                    RoomUser roomUserByVirtualId = this.GetRoomUserByVirtualId(Convert.ToInt32(User.HorseID));
                    roomUserByVirtualId.IsWalking = false;
                    this.UpdateUserStatus(roomUserByVirtualId, false);
                    roomUserByVirtualId.RemoveStatus("mv");
                }
            }
            else
            {
                this.HandleSetMovement(nextStep, User);

                if (User.BreakWalkEnable && !User.StopWalking)
                {
                    User.StopWalking = true;
                }

                if (User.RidingHorse && !User.IsPet)
                {
                    RoomUser roomUserByVirtualId = this.GetRoomUserByVirtualId(Convert.ToInt32(User.HorseID));
                    this.HandleSetMovement(nextStep, roomUserByVirtualId);
                    roomUserByVirtualId.UpdateNeeded = true;
                }

                if (User.IsSit)
                {
                    User.IsSit = false;
                }

                if (User.IsLay)
                {
                    User.IsLay = false;
                }

                this._room.GetSoccer().OnUserWalk(User, nextStep.X == User.GoalX && nextStep.Y == User.GoalY);
                this._room.GetBanzai().OnUserWalk(User);
            }
        }

        private void HandleSetMovement(SquarePoint nextStep, RoomUser User)
        {
            int nextX = nextStep.X;
            int nextY = nextStep.Y;

            double nextZ = this._room.GetGameMap().SqAbsoluteHeight(nextX, nextY);
            if (User.RidingHorse && !User.IsPet)
            {
                nextZ += 1;
            }

            User.RemoveStatus("mv");
            User.RemoveStatus("lay");
            User.RemoveStatus("sit");

            User.SetStatus("mv", nextX + "," + nextY + "," + nextZ);

            int newRot;
            if (User.FacewalkEnabled)
            {
                newRot = User.RotBody;
            }
            else
            {
                newRot = Rotation.Calculate(User.X, User.Y, nextX, nextY, User.MoonwalkEnabled);
            }

            User.RotBody = newRot;
            User.RotHead = newRot;

            User.SetStep = true;
            User.SetX = nextX;
            User.SetY = nextY;
            User.SetZ = nextZ;

            this._room.GetGameMap().AddTakingSquare(nextX, nextY);

            this.UpdateUserEffect(User, User.SetX, User.SetY);
        }

        private bool SetStepForUser(RoomUser User)
        {
            this._room.GetGameMap().UpdateUserMovement(User.Coordinate, new Point(User.SetX, User.SetY), User);

            List<Item> coordinatedItems = this._room.GetGameMap().GetCoordinatedItems(new Point(User.X, User.Y)).ToList();

            
            User.X = User.SetX;
            User.Y = User.SetY;
            User.Z = User.SetZ;

            this._room.CollisionUser(User);

            if (User.IsBot)
            {
                RoomUser BotCollisionUser = this._room.GetGameMap().LookHasUserNearNotBot(User.X, User.Y);
                if (BotCollisionUser != null)
                {
                    this._room.GetWiredHandler().TriggerBotCollision(BotCollisionUser, User.BotData.Name);
                }
            }

            if (this._room.IsRoleplay)
            {
                RolePlayer Rp = User.Roleplayer;
                if (Rp != null && !Rp.Dead)
                {
                    ItemTemp ItemTmp = this._room.GetRoomItemHandler().GetFirstTempDrop(User.X, User.Y);
                    if (ItemTmp != null && ItemTmp.InteractionType == InteractionTypeTemp.MONEY)
                    {
                        Rp.Money += ItemTmp.Value;
                        Rp.SendUpdate();
                        if (User.GetClient() != null)
                        {
                            User.SendWhisperChat(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("rp.pickdollard", User.GetClient().Langue), ItemTmp.Value));
                        }

                        User.OnChat("*Récupère un objet au sol*");
                        this._room.GetRoomItemHandler().RemoveTempItem(ItemTmp.Id);
                    }
                    else if (ItemTmp != null && ItemTmp.InteractionType == InteractionTypeTemp.RPITEM)
                    {
                        RPItem RpItem = WibboEnvironment.GetGame().GetRoleplayManager().GetItemManager().GetItem(ItemTmp.Value);
                        if (RpItem != null)
                        {
                            if (!RpItem.AllowStack && Rp.GetInventoryItem(RpItem.Id) != null)
                            {
                                if (User.GetClient() != null)
                                {
                                    User.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("rp.itemown", User.GetClient().Langue));
                                }
                            }
                            else
                            {
                                Rp.AddInventoryItem(RpItem.Id);

                                if (User.GetClient() != null)
                                {
                                    User.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("rp.itempick", User.GetClient().Langue));
                                }
                            }
                        }

                        User.OnChat("*Récupère un objet au sol*");
                        this._room.GetRoomItemHandler().RemoveTempItem(ItemTmp.Id);
                    }
                }
            }

            foreach (Item roomItem in coordinatedItems)
            {
                roomItem.UserWalksOffFurni(User, roomItem);

                if (roomItem.GetBaseItem().InteractionType == InteractionType.GUILD_GATE)
                {
                    roomItem.ExtraData = "0;" + roomItem.GroupId;
                    roomItem.UpdateState(false, true);
                }
                else if (roomItem.GetBaseItem().InteractionType == InteractionType.PRESSUREPAD
                    || roomItem.GetBaseItem().InteractionType == InteractionType.TRAMPOLINE
                    || roomItem.GetBaseItem().InteractionType == InteractionType.TREADMILL
                    || roomItem.GetBaseItem().InteractionType == InteractionType.CROSSTRAINER)
                {
                    roomItem.ExtraData = "0";
                    roomItem.UpdateState(false, true);
                }
                else if (roomItem.GetBaseItem().InteractionType == InteractionType.FOOTBALL)
                {
                    if (!User.AllowMoveToRoller || roomItem.InteractionCountHelper > 0 || this._room.OldFoot)
                    {
                        continue;
                    }

                    switch (User.RotBody)
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
                    roomItem.InteractingUser = User.VirtualId;
                    roomItem.ReqUpdate(1);
                }
            }

            this.UpdateUserStatus(User, true);
            this._room.GetGameMap().RemoveTakingSquare(User.SetX, User.SetY);

            User.SetStep = false;
            User.AllowMoveToRoller = false;

            if (User.SetMoveWithBall)
            {
                User.SetMoveWithBall = false;
                User.MoveWithBall = false;
            }
            return false;
        }

        public void Destroy()
        {
            this._room = null;
            this._usersByUsername.Clear();
            this._usersByUserID.Clear();
            this.OnUserEnter = null;
            this._pets.Clear();
            this._bots.Clear();
            this._users.Clear();
            this._usersRank.Clear();
        }
    }
}
