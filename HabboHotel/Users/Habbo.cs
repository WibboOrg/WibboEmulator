using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.Core;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.Achievements;
using Butterfly.HabboHotel.ChatMessageStorage;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Roleplay;
using Butterfly.HabboHotel.Roleplay.Player;
using Butterfly.HabboHotel.Rooms;
using Butterfly.HabboHotel.Users.Badges;
using Butterfly.HabboHotel.Users.Inventory;
using Butterfly.HabboHotel.Users.Messenger;
using Butterfly.HabboHotel.WebClients;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Butterfly.HabboHotel.Users
{
    public class Habbo
    {
        public bool forceOpenGift;
        public int forceUse = -1;
        public int forceRot = -1;
        public int ChatColor = -1;
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
        public int FloodCount;
        public DateTime FloodTime;
        public List<int> ClientVolume;
        public string MachineId;
        public Language Langue;

        public List<RoomData> RoomRightsList;
        public List<RoomData> FavoriteRooms;
        public List<RoomData> UsersRooms;

        public List<int> MutedUsers;
        public Dictionary<string, UserAchievement> Achievements;
        public List<int> RatedRooms;
        private HabboMessenger Messenger;
        private BadgeComponent BadgeComponent;
        private InventoryComponent InventoryComponent;
        private ChatMessageManager chatMessageManager;
        private GameClient mClient;
        public bool SpectatorMode;
        public bool Disconnected;
        public bool HasFriendRequestsDisabled;
        public int FavouriteGroupId;
        public List<int> MyGroups;

        public bool spamEnable;
        public int spamProtectionTime;
        public DateTime spamFloodTime;
        public DateTime everyoneTimer;

        public Dictionary<int, int> quests;
        public int CurrentQuestId;
        public int LastCompleted;
        public int LastQuestId;
        private bool HabboinfoSaved;
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

        public bool IgnoreAll;

        public DateTime LastGiftPurchaseTime;

        public bool InRoom => this.CurrentRoomId >= 1;

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
                    return ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(this.CurrentRoomId);
                }
            }
        }

        public bool SendWebPacket(IServerPacket Message)
        {
            WebClient ClientWeb = ButterflyEnvironment.GetGame().GetClientWebManager().GetClientByUserID(this.Id);
            if (ClientWeb != null)
            {
                ClientWeb.SendPacket(Message);
                return true;
            }

            return false;
        }

        public string GetQueryString
        {
            get
            {
                TimeSpan TimeOnline = DateTime.Now - this.OnlineTime;
                int TimeOnlineSec = (int)TimeOnline.TotalSeconds;
                this.HabboinfoSaved = true;

                return "UPDATE users SET users.online = '0', users.last_online = '" + ButterflyEnvironment.GetUnixTimestamp() + "', activity_points = '" + this.Duckets + "', credits = '" + this.Credits + "' WHERE id = '" + this.Id + "'; " +
                    "UPDATE user_stats SET group_id = " + this.FavouriteGroupId + ", online_time = online_time + " + TimeOnlineSec + ", quest_id = '" + this.CurrentQuestId + "', Respect = '" + this.Respect + "', daily_respect_points = '" + this.DailyRespectPoints + "', daily_pet_respect_points = '" + this.DailyPetRespectPoints + "' WHERE id = " + this.Id + ";";
            }
        }

        public Habbo(int Id, string Username, int Rank, string Motto, string Look, string Gender, int Credits,
            int WPoint, int ActivityPoints, int HomeRoom, int Respect, int DailyRespectPoints,
            int DailyPetRespectPoints, bool HasFriendRequestsDisabled, int currentQuestID, int achievementPoints,
            int LastOnline, int FavoriteGroup, int accountCreated, bool accepttrading, string ip, bool HideInroom,
            bool HideOnline, int MazoHighScore, int Mazo, string clientVolume, bool nuxenable, string MachineId, bool ChangeName, Language Langue, bool IgnoreAll)
        {
            this.Id = Id;
            this.Username = Username;
            this.Rank = Rank;
            this.Motto = Motto;
            this.MachineId = MachineId;
            this.Look = ButterflyEnvironment.GetFigureManager().ProcessFigure(Look, Gender, true);
            this.Gender = Gender.ToLower();
            this.Credits = Credits;
            this.WibboPoints = WPoint;
            this.Duckets = ActivityPoints;
            this.AchievementPoints = achievementPoints;
            this.CurrentRoomId = 0;
            this.LoadingRoomId = 0;
            this.HomeRoom = HomeRoom;
            this.FavoriteRooms = new List<RoomData>();
            this.MutedUsers = new List<int>();
            this.Achievements = new Dictionary<string, UserAchievement>();
            this.RatedRooms = new List<int>();
            this.Respect = Respect;
            this.DailyRespectPoints = DailyRespectPoints;
            this.DailyPetRespectPoints = DailyPetRespectPoints;
            this.IsTeleporting = false;
            this.TeleporterId = 0;
            this.UsersRooms = new List<RoomData>();
            this.HasFriendRequestsDisabled = HasFriendRequestsDisabled;
            this.ClientVolume = new List<int>(3);
            this.CanChangeName = ChangeName;
            this.Langue = Langue;
            this.IgnoreAll = IgnoreAll;

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
        }

        public void Init(GameClient client, UserData.UserData data)
        {
            this.mClient = client;
            this.BadgeComponent = new BadgeComponent(this.Id, data.badges);
            this.InventoryComponent = new InventoryComponent(this.Id, client);
            this.InventoryComponent.SetActiveState(client);
            this.quests = data.quests;
            this.chatMessageManager = new ChatMessageManager();
            this.chatMessageManager.LoadUserChatlogs(this.Id);
            this.Messenger = new HabboMessenger(this.Id)
            {
                AppearOffline = this.HideOnline
            };
            this.Messenger.Init(data.friends, data.requests, data.Relationships);
            this.MyGroups = data.MyGroups;

            this.UpdateRooms();
        }


        public void UpdateRooms()
        {
            try
            {
                this.UsersRooms.Clear();

                DataTable table;
                using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    queryreactor.SetQuery("SELECT * FROM rooms WHERE owner = @name ORDER BY id ASC");
                    queryreactor.AddParameter("name", this.Username);
                    table = queryreactor.GetTable();
                }

                foreach (DataRow dRow in table.Rows)
                {
                    this.UsersRooms.Add(ButterflyEnvironment.GetGame().GetRoomManager().FetchRoomData(Convert.ToInt32(dRow["id"]), dRow));
                }
            }

            catch (Exception ex)
            {
                Logging.LogCriticalException("Bug while updating own rooms: " + (ex).ToString());
            }
        }

        public void PrepareRoom(int Id, string Password = "", bool override_doorbell = false)
        {
            if (this.GetClient() == null || this.GetClient().GetHabbo() == null)
            {
                return;
            }

            if (this.GetClient().GetHabbo().InRoom)
            {
                Room OldRoom = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(this.GetClient().GetHabbo().CurrentRoomId);

                if (OldRoom != null)
                {
                    OldRoom.GetRoomUserManager().RemoveUserFromRoom(this.GetClient(), false, false);
                }
            }

            Room room = ButterflyEnvironment.GetGame().GetRoomManager().LoadRoom(Id);
            if (room == null)
            {
                this.GetClient().SendPacket(new CloseConnectionComposer());
                return;
            }

            if (this.GetClient().GetHabbo().IsTeleporting && this.GetClient().GetHabbo().TeleportingRoomID != Id)
            {
                this.GetClient().GetHabbo().TeleportingRoomID = 0;
                this.GetClient().GetHabbo().IsTeleporting = false;
                this.GetClient().GetHabbo().TeleporterId = 0;
                this.GetClient().SendPacket(new CloseConnectionComposer());

                return;
            }

            if (!this.GetClient().GetHabbo().HasFuse("fuse_mod") && room.UserIsBanned(this.GetClient().GetHabbo().Id))
            {
                if (room.HasBanExpired(this.GetClient().GetHabbo().Id))
                {
                    room.RemoveBan(this.GetClient().GetHabbo().Id);
                }
                else
                {
                    this.GetClient().SendPacket(new CantConnectComposer(1));

                    this.GetClient().SendPacket(new CloseConnectionComposer());
                    return;
                }
            }

            if (room.RoomData.UsersNow >= room.RoomData.UsersMax && !this.GetClient().GetHabbo().HasFuse("fuse_enter_full_rooms") && !ButterflyEnvironment.GetGame().GetRoleManager().RankHasRight(this.GetClient().GetHabbo().Rank, "fuse_enter_full_rooms"))
            {
                if (room.CloseFullRoom)
                {
                    room.RoomData.State = 1;
                    room.CloseFullRoom = false;
                }

                if (this.GetClient().GetHabbo().Id != room.RoomData.OwnerId)
                {
                    this.GetClient().SendPacket(new CantConnectComposer(1));

                    this.GetClient().SendPacket(new CloseConnectionComposer());
                    return;
                }
            }

            string[] OwnerEnterNotAllowed = { "WibboGame", "LieuPublic", "WorldRunOff", "SeasonRunOff", "WibboParty", "MovieRunOff" };

            if (!this.GetClient().GetHabbo().HasFuse("fuse_mod"))
            {
                if (!(this.GetClient().GetHabbo().HasFuse("fuse_enter_any_room") && !OwnerEnterNotAllowed.Any(x => x == room.RoomData.OwnerName)) && !room.CheckRights(this.GetClient(), true) && !(this.GetClient().GetHabbo().IsTeleporting && this.GetClient().GetHabbo().TeleportingRoomID == room.Id))
                {
                    if (room.RoomData.State == 1 && (!override_doorbell && !room.CheckRights(this.GetClient())))
                    {
                        if (room.UserCount == 0)
                        {
                            ServerPacket message = new ServerPacket(ServerPacketHeader.ROOM_DOORBELL_DENIED);
                            this.GetClient().SendPacket(message);
                        }
                        else
                        {
                            this.GetClient().SendPacket(new DoorbellComposer(""));
                            room.SendPacket(new DoorbellComposer(this.GetClient().GetHabbo().Username), true);
                            this.GetClient().GetHabbo().LoadingRoomId = Id;
                            this.GetClient().GetHabbo().AllowDoorBell = false;
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

            if (room.RoomData.OwnerName == "WibboGame" || room.RoomData.OwnerName == "WibboParty")
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
                this.GetClient().GetHabbo().LoadingRoomId = Id;
                this.GetClient().GetHabbo().AllowDoorBell = true;
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
            Session.SendPacket(new RoomRatingComposer(Room.RoomData.Score, !(Session.GetHabbo().RatedRooms.Contains(Room.Id) || Room.RoomData.OwnerId == Session.GetHabbo().Id)));


            return true;
        }

        public void LoadData(UserData.UserData data)
        {
            this.LoadAchievements(data.achievements);
            this.LoadFavorites(data.favouritedRooms);
            this.LoadRoomRights(data.RoomRightsList);
        }

        public bool HasFuse(string Fuse)
        {
            if (ButterflyEnvironment.GetGame().GetRoleManager().RankHasRight(this.Rank, Fuse))
            {
                return true;
            }

            return false;
        }

        public void LoadRoomRights(List<int> roomID)
        {
            this.RoomRightsList = new List<RoomData>();
            foreach (int num in roomID)
            {
                RoomData roomdata = ButterflyEnvironment.GetGame().GetRoomManager().GenerateRoomData(num);
                this.RoomRightsList.Add(roomdata);
            }
        }

        public void LoadFavorites(List<int> roomID)
        {
            this.FavoriteRooms = new List<RoomData>();
            foreach (int num in roomID)
            {
                RoomData roomdata = ButterflyEnvironment.GetGame().GetRoomManager().GenerateRoomData(num);
                this.FavoriteRooms.Add(roomdata);
            }
        }

        public void LoadAchievements(Dictionary<string, UserAchievement> achievements)
        {
            this.Achievements = achievements;
        }

        public void OnDisconnect()
        {
            if (this.Disconnected)
            {
                return;
            }

            this.Disconnected = true;

            ButterflyEnvironment.GetGame().GetClientManager().UnregisterClient(this.Id, this.Username);

            if (this.Langue == Language.FRANCAIS)
            {
                ButterflyEnvironment.GetGame().GetClientManager().OnlineUsersFr--;
            }
            else if (this.Langue == Language.ANGLAIS)
            {
                ButterflyEnvironment.GetGame().GetClientManager().OnlineUsersEn--;
            }
            else if (this.Langue == Language.PORTUGAIS)
            {
                ButterflyEnvironment.GetGame().GetClientManager().OnlineUsersBr--;
            }

            if (this.HasFuse("fuse_mod"))
            {
                ButterflyEnvironment.GetGame().GetClientManager().RemoveUserStaff(this.Id);
            }

            Logging.WriteLine(this.Username + " has logged out.");

            if (!this.HabboinfoSaved)
            {
                this.HabboinfoSaved = true;
                TimeSpan TimeOnline = DateTime.Now - this.OnlineTime;
                int TimeOnlineSec = (int)TimeOnline.TotalSeconds;
                using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    queryreactor.RunQuery("UPDATE users SET online = '0', last_online = '" + ButterflyEnvironment.GetUnixTimestamp() + "', activity_points = " + this.Duckets + ", credits = " + this.Credits + " WHERE id = " + this.Id + " ;");
                    queryreactor.RunQuery("UPDATE user_stats SET group_id = " + this.FavouriteGroupId + ",  online_time = online_time + " + TimeOnlineSec + ", quest_id = '" + this.CurrentQuestId + "', Respect = '" + this.Respect + "', daily_respect_points = '" + this.DailyRespectPoints + "', daily_pet_respect_points = '" + this.DailyPetRespectPoints + "' WHERE id = " + this.Id + " ;");
                }
            }

            if (this.InRoom && this.CurrentRoom != null)
            {
                this.CurrentRoom.GetRoomUserManager().RemoveUserFromRoom(this.mClient, false, false);
            }

            if (this.RolePlayId > 0)
            {
                RolePlayerManager RPManager = ButterflyEnvironment.GetGame().GetRoleplayManager().GetRolePlay(this.RolePlayId);
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
                GameClient requester = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(this.GuideOtherUserId);
                if (requester != null)
                {
                    ServerPacket message = new ServerPacket(ServerPacketHeader.OnGuideSessionEnded);
                    message.WriteInteger(1);
                    requester.SendPacket(message);

                    requester.GetHabbo().GuideOtherUserId = 0;
                }
            }
            if (this.OnDuty)
            {
                ButterflyEnvironment.GetGame().GetGuideManager().RemoveGuide(this.Id);
            }

            if (this.Messenger != null)
            {
                this.Messenger.AppearOffline = true;
                this.Messenger.Destroy();
            }

            if (this.InventoryComponent != null)
            {
                this.InventoryComponent.Destroy();
                this.InventoryComponent = null;
            }

            if (this.BadgeComponent != null)
            {
                this.BadgeComponent.Destroy();
                this.BadgeComponent = null;
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

            this.mClient = null;
        }

        public void UpdateCreditsBalance()
        {
            GameClient client = this.GetClient();
            if (client == null)
            {
                return;
            }

            client.SendPacket(new CreditBalanceComposer(this.Credits));
        }

        public void UpdateWPBalance()
        {
            GameClient client = this.GetClient();
            if (client == null)
            {
                return;
            }

            ServerPacket Message = new ServerPacket(ServerPacketHeader.USER_CURRENCY);
            Message.WriteInteger(1);
            Message.WriteInteger(105);
            Message.WriteInteger(this.WibboPoints);
            client.SendPacket(Message);
        }

        public void UpdateActivityPointsBalance()
        {
            GameClient client = this.GetClient();
            if (client == null)
            {
                return;
            }

            client.SendPacket(new HabboActivityPointNotificationComposer(this.Duckets, 1));
        }

        private GameClient GetClient()
        {
            return ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(this.Id);
        }

        public HabboMessenger GetMessenger()
        {
            return this.Messenger;
        }

        public BadgeComponent GetBadgeComponent()
        {
            return this.BadgeComponent;
        }

        public InventoryComponent GetInventoryComponent()
        {
            return this.InventoryComponent;
        }

        public ChatMessageManager GetChatMessageManager()
        {
            return this.chatMessageManager;
        }

        public int GetQuestProgress(int p)
        {
            this.quests.TryGetValue(p, out int num);
            return num;
        }

        public UserAchievement GetAchievementData(string p)
        {
            this.Achievements.TryGetValue(p, out UserAchievement userAchievement);
            return userAchievement;
        }
    }
}
