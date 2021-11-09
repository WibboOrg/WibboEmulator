using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using Butterfly.Communication.Packets.Outgoing.Rooms.Avatar;
using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Communication.Packets.Outgoing.Rooms.Session;

using Butterfly.Core;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.Rooms.Chat.Logs;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Items;
using Butterfly.HabboHotel.Pets;
using Butterfly.HabboHotel.Roleplay;
using Butterfly.HabboHotel.Rooms.Games;
using Butterfly.HabboHotel.Rooms.Janken;
using Butterfly.HabboHotel.Rooms.Projectile;
using Butterfly.HabboHotel.Rooms.AI;
using Butterfly.HabboHotel.Rooms.Wired;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Butterfly.Database.Daos;

namespace Butterfly.HabboHotel.Rooms
{
    public delegate void RoomEventDelegate(object sender, EventArgs e);
    public delegate void RoomUserSaysDelegate(object sender, UserSaysArgs e, ref bool messageHandled);
    public delegate void TriggerUserDelegate(RoomUser user, string ActionType);
    public delegate void BotCollisionDelegate(RoomUser user, string BotName);

    public class Room
    {
        public bool RoomMuted;
        public bool isCycling;
        public int IsLagging;
        public bool mCycleEnded;
        public int IdleTime;

        public bool IsRoleplay;
        public bool Pvp;
        public int RpHour;
        public int RpMinute;
        public int RpIntensity;
        public bool RpCycleHourEffect;
        public bool RpTimeSpeed;

        private TeamManager _teamManager;

        public List<int> UsersWithRights;
        public bool EveryoneGotRights;
        private readonly Dictionary<int, double> _bans;
        private readonly Dictionary<int, double> _mutes;
        public bool HeightMapLoaded;
        public DateTime lastTimerReset;
        public GameManager game;
        private readonly Gamemap gamemap;
        private readonly RoomItemHandling roomItemHandling;
        private readonly RoomUserManager roomUserManager;
        private Soccer soccer;
        private BattleBanzai banzai;
        private Freeze freeze;
        private JankenManager jankan;
        private GameItemHandler gameItemHandler;
        private WiredHandler wiredHandler;
        public MoodlightData MoodlightData;
        public List<Trade> ActiveTrades;
        private readonly ChatlogManager chatMessageManager;
        public RoomData RoomData;
        public bool Disposed;
        public bool RoomMutePets;
        public bool FreezeRoom;
        public bool PushPullAllowed;
        public bool CloseFullRoom;
        public bool OldFoot;
        public bool RoomIngameChat;

        private ProjectileManager projectileManager;
        private int SaveTimer;

        //Question
        public int VotedYesCount;
        public int VotedNoCount;
        private List<CancellationTokenSource> cancellationTokenSources;

        public int UserCount => this.roomUserManager.GetRoomUserCount();

        public int Id => this.RoomData.Id;
        public string Name => this.RoomData.Name;

        public event TriggerUserDelegate TriggerUser;
        public event RoomUserSaysDelegate OnUserSays;
        public event RoomEventDelegate OnTrigger;
        public event RoomEventDelegate OnTriggerSelf;
        public event RoomEventDelegate OnUserCls;

        public Room(RoomData Data)
        {
            RolePlayerManager RPManager = ButterflyEnvironment.GetGame().GetRoleplayManager().GetRolePlay(Data.OwnerId);
            if (RPManager != null)
            {
                this.IsRoleplay = true;
                this.Pvp = true;
                this.RpCycleHourEffect = true;
                this.RpTimeSpeed = false;
                this.RpHour = -1;
            }

            this.SaveTimer = 0;
            this.Disposed = false;
            this._bans = new Dictionary<int, double>();
            this._mutes = new Dictionary<int, double>();
            this.ActiveTrades = new List<Trade>();
            this.mCycleEnded = false;
            this.HeightMapLoaded = false;
            this.RoomData = Data;
            this.EveryoneGotRights = Data.AllowRightsOverride;
            this.IdleTime = 0;
            this.RoomMuted = false;
            this.PushPullAllowed = true;
            this.RoomIngameChat = false;
            this.gamemap = new Gamemap(this);
            this.roomItemHandling = new RoomItemHandling(this);
            this.roomUserManager = new RoomUserManager(this);
            this.wiredHandler = new WiredHandler(this);
            this.projectileManager = new ProjectileManager(this);
            this.chatMessageManager = new ChatlogManager();
            this.chatMessageManager.LoadRoomChatlogs(this.Id);
            this.LoadRights();
            this.GetRoomItemHandler().LoadFurniture();
            if (this.RoomData.OwnerName == "WibboGame")
            {
                this.GetRoomItemHandler().LoadFurniture(5400713);
            }

            this.GetGameMap().GenerateMaps(true);
            this.LoadBots();
            this.InitPets();
            this.lastTimerReset = DateTime.Now;
            this.cancellationTokenSources = new List<CancellationTokenSource>();
        }

        public Gamemap GetGameMap()
        {
            return this.gamemap;
        }

        public RoomItemHandling GetRoomItemHandler()
        {
            return this.roomItemHandling;
        }

        public RoomUserManager GetRoomUserManager()
        {
            return this.roomUserManager;
        }

        public Soccer GetSoccer()
        {
            if (this.soccer == null)
            {
                this.soccer = new Soccer(this);
            }

            return this.soccer;
        }

        public TeamManager GetTeamManager()
        {
            if (this._teamManager == null)
            {
                this._teamManager = new TeamManager();
            }

            return this._teamManager;
        }

        public BattleBanzai GetBanzai()
        {
            if (this.banzai == null)
            {
                this.banzai = new BattleBanzai(this);
            }

            return this.banzai;
        }

        public Freeze GetFreeze()
        {
            if (this.freeze == null)
            {
                this.freeze = new Freeze(this);
            }

            return this.freeze;
        }

        public JankenManager GetJanken()
        {
            if (this.jankan == null)
            {
                this.jankan = new JankenManager(this);
            }

            return this.jankan;
        }

        public GameManager GetGameManager()
        {
            if (this.game == null)
            {
                this.game = new GameManager(this);
            }

            return this.game;
        }

        public GameItemHandler GetGameItemHandler()
        {
            if (this.gameItemHandler == null)
            {
                this.gameItemHandler = new GameItemHandler(this);
            }

            return this.gameItemHandler;
        }

        public WiredHandler GetWiredHandler()
        {
            if (this.wiredHandler == null)
            {
                this.wiredHandler = new WiredHandler(this);
            }

            return this.wiredHandler;
        }

        public ProjectileManager GetProjectileManager()
        {
            if (this.projectileManager == null)
            {
                this.projectileManager = new ProjectileManager(this);
            }

            return this.projectileManager;
        }

        public bool GotSoccer()
        {
            return this.soccer != null;
        }

        public bool GotBanzai()
        {
            return this.banzai != null;
        }

        public bool GotFreeze()
        {
            return this.freeze != null;
        }

        public bool GotJanken()
        {
            return this.jankan != null;
        }

        public bool GotWired()
        {
            return this.wiredHandler != null;
        }

        public ChatlogManager GetChatMessageManager()
        {
            return this.chatMessageManager;
        }

        public bool AllowsShous(RoomUser user, string message)
        {
            bool messageHandled = false;
            if (this.OnUserSays != null)
            {
                this.OnUserSays(null, new UserSaysArgs(user, message), ref messageHandled);
            }

            return messageHandled;
        }

        public void CollisionUser(RoomUser User)
        {
            if (this.OnUserCls == null)
            {
                return;
            }

            int Lenght = 1;
            int GoalX = User.X;
            int GoalY = User.Y;

            switch (User.RotBody)
            {
                case 0:
                    GoalX = User.X;
                    GoalY = User.Y - Lenght;
                    break;
                case 1:
                    GoalX = User.X + Lenght;
                    GoalY = User.Y - Lenght;
                    break;
                case 2:
                    GoalX = User.X + Lenght;
                    GoalY = User.Y;
                    break;
                case 3:
                    GoalX = User.X + Lenght;
                    GoalY = User.Y + Lenght;
                    break;
                case 4:
                    GoalX = User.X;
                    GoalY = User.Y + Lenght;
                    break;
                case 5:
                    GoalX = User.X - Lenght;
                    GoalY = User.Y + Lenght;
                    break;
                case 6:
                    GoalX = User.X - Lenght;
                    GoalY = User.Y;
                    break;
                case 7:
                    GoalX = User.X - Lenght;
                    GoalY = User.Y - Lenght;
                    break;
            }

            RoomUser UserGoal = this.GetRoomUserManager().GetUserForSquare(GoalX, GoalY);
            if (UserGoal == null)
            {
                return;
            }

            if (UserGoal.Team == User.Team && User.Team != Team.none)
            {
                return;
            }

            this.OnUserCls(UserGoal, null);
        }

        public void onTriggerUser(RoomUser roomUser, bool isTarget)
        {
            if (isTarget)
            {
                if (this.OnTrigger != null)
                {
                    this.OnTrigger(roomUser, null);
                }
            }
            else
            {
                if (this.OnTriggerSelf != null)
                {
                    this.OnTriggerSelf(roomUser, null);
                }
            }
        }

        public void ClearTags()
        {
            this.RoomData.Tags.Clear();
        }

        public void AddTagRange(List<string> tags)
        {
            this.RoomData.Tags.AddRange(tags);
        }

        private void LoadBots()
        {
            DataTable table;
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                table = BotDao.GetOneByRoomId(dbClient, this.Id);
                if (table == null)
                {
                    return;
                }

                foreach (DataRow Row in table.Rows)
                {
                    RoomBot roomBot = new RoomBot(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["user_id"]), Convert.ToInt32(Row["room_id"]), (this.IsRoleplay) ? AIType.RolePlayBot : AIType.Generic, (string)Row["walk_enabled"] == "1", (string)Row["name"], (string)Row["motto"], (string)Row["gender"], (string)Row["look"], Convert.ToInt32(Row["x"]), Convert.ToInt32(Row["y"]), Convert.ToInt32(Row["z"]), Convert.ToInt32(Row["rotation"]), (string)Row["chat_enabled"] == "1", (string)Row["chat_text"], Convert.ToInt32(Row["chat_seconds"]), (string)Row["is_dancing"] == "1", Convert.ToInt32(Row["enable"]), Convert.ToInt32(Row["handitem"]), Convert.ToInt32((string)Row["status"]));
                    RoomUser roomUser = this.GetRoomUserManager().DeployBot(roomBot, null);
                    if (roomBot.IsDancing)
                    {
                        roomUser.DanceId = 3;
                    }
                }
            }
        }

        public void InitPets()
        {
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DataTable table = PetDao.GetAllByRoomId(dbClient, this.Id);
                if (table == null)
                {
                    return;
                }

                foreach (DataRow Row in table.Rows)
                {
                    Pet PetData = new Pet(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["user_id"]), Convert.ToInt32(Row["room_id"]), (string)Row["name"], Convert.ToInt32(Row["type"]), (string)Row["race"], (string)Row["color"], Convert.ToInt32(Row["experience"]), Convert.ToInt32(Row["energy"]), Convert.ToInt32(Row["nutrition"]), Convert.ToInt32(Row["respect"]), (double)Row["createstamp"], Convert.ToInt32(Row["x"]), Convert.ToInt32(Row["y"]), (double)Row["z"], Convert.ToInt32(Row["have_saddle"]), Convert.ToInt32(Row["hairdye"]), Convert.ToInt32(Row["pethair"]), (string)(Row["anyone_ride"]) == "1");
                    List<string> list = new List<string>();
                    this.roomUserManager.DeployBot(new RoomBot(PetData.PetId, PetData.OwnerId, this.Id, AIType.Pet, true, PetData.Name, "", "", PetData.Look, PetData.X, PetData.Y, PetData.Z, 0, false, "", 0, false, 0, 0, 0), PetData);
                }
            }
        }

        public void onRoomKick()
        {
            List<RoomUser> list = new List<RoomUser>();
            foreach (RoomUser roomUser in this.roomUserManager.GetUserList().ToList())
            {
                if (!roomUser.IsBot && !roomUser.GetClient().GetHabbo().HasFuse("fuse_no_kick"))
                {
                    this.GetRoomUserManager().RemoveUserFromRoom(roomUser.GetClient(), true, true);
                }
            }
        }

        public void OnUserSay(RoomUser User, string Message, bool Shout)
        {
            foreach (RoomUser roomUser in this.roomUserManager.GetPets().ToList())
            {
                if (Shout)
                {
                    roomUser.BotAI.OnUserShout(User, Message);
                }
                else
                {
                    roomUser.BotAI.OnUserSay(User, Message);
                }
            }
        }

        public void LoadRights()
        {
            this.UsersWithRights = new List<int>();
            DataTable dataTable = new DataTable();
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
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

        public int GetRightsLevel(GameClient Session)
        {
            if (Session == null || Session.GetHabbo() == null)
            {
                return 0;
            }

            if (Session.GetHabbo().Username == this.RoomData.OwnerName || Session.GetHabbo().HasFuse("fuse_any_room_controller"))
            {
                return 4;
            }

            if (Session.GetHabbo().HasFuse("fuse_any_room_rights"))
            {
                return 3;
            }

            if (this.UsersWithRights.Contains(Session.GetHabbo().Id))
            {
                return 1;
            }

            if (this.EveryoneGotRights)
            {
                return 1;
            }

            return 0;
        }

        public bool CheckRights(GameClient Session, bool RequireOwnership = false)
        {
            if (Session == null || Session.GetHabbo() == null)
            {
                return false;
            }

            if (Session.GetHabbo().Username == this.RoomData.OwnerName || Session.GetHabbo().HasFuse("fuse_any_room_controller"))
            {
                return true;
            }

            if (!RequireOwnership)
            {
                if (Session.GetHabbo().HasFuse("fuse_any_room_rights") || this.UsersWithRights.Contains(Session.GetHabbo().Id))
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

                if (this.RoomData.Group.IsAdmin(Session.GetHabbo().Id))
                {
                    return true;
                }

                if (this.RoomData.Group.AdminOnlyDeco == 0)
                {
                    if (this.RoomData.Group.IsMember(Session.GetHabbo().Id))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void SendObjects(GameClient Session)
        {
            Session.SendPacket(this.GetGameMap().Model.SerializeRelativeHeightmap());
            Session.SendPacket(this.GetGameMap().Model.GetHeightmap());

            foreach (RoomUser RoomUser in this.roomUserManager.GetUserList().ToList())
            {
                if (RoomUser == null)
                {
                    continue;
                }

                if (RoomUser.IsSpectator)
                {
                    continue;
                }

                if (!RoomUser.IsBot && RoomUser.GetClient() == null)
                {
                    continue;
                }

                if (!RoomUser.IsBot && RoomUser.GetClient().GetHabbo() == null)
                {
                    continue;
                }

                Session.SendPacket(new UsersComposer(RoomUser));

                if (RoomUser.IsDancing)
                {
                    Session.SendPacket(new DanceComposer(RoomUser, RoomUser.DanceId));
                }

                if (RoomUser.IsAsleep)
                {
                    Session.SendPacket(new SleepComposer(RoomUser, true));
                }

                if (RoomUser.CarryItemID > 0 && RoomUser.CarryTimer > 0)
                {
                    Session.SendPacket(new CarryObjectComposer(RoomUser.VirtualId, RoomUser.CarryItemID));
                }

                if (RoomUser.CurrentEffect > 0)
                {
                    Session.SendPacket(new AvatarEffectComposer(RoomUser.VirtualId, RoomUser.CurrentEffect));
                }
            }

            Session.SendPacket(new UserUpdateComposer(this.roomUserManager.GetUserList().ToList()));
            Session.SendPacket(new ObjectsComposer(this.GetRoomItemHandler().GetFloor.ToArray(), this));
            Session.SendPacket(new ObjectsComposer(this.GetRoomItemHandler().GetTempItems.ToArray(), this));
            Session.SendPacket(new ItemsComposer(this.GetRoomItemHandler().GetWall.ToArray(), this));
        }

        public void ProcessRoom(object pCallback)
        {
            try
            {
                this.isCycling = true;
                if (this.Disposed)
                {
                    return;
                }

                try
                {
                    int idleCount = 0;

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

                    if (!this.mCycleEnded)
                    {
                        if (this.IdleTime >= 60)
                        {
                            ButterflyEnvironment.GetGame().GetRoomManager().UnloadRoom(this);

                            return;
                        }
                        else
                        {
                            this.GetRoomUserManager().SerializeStatusUpdates();
                        }
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

                    if (this.SaveTimer < ((2 * 60) * 2))
                    {
                        this.SaveTimer++;
                    }
                    else
                    {
                        this.SaveTimer = 0;
                        using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            this.GetRoomItemHandler().SaveFurniture(dbClient);
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.OnRoomCrash(ex);
                }
            }
            catch (Exception ex)
            {
                this.isCycling = false;
                Logging.LogCriticalException("Sub crash in room cycle: " + (ex).ToString());
            }
            finally
            {
                this.isCycling = false;
            }
        }

        private void RpCycleHour()
        {
            if (!this.IsRoleplay)
            {
                return;
            }

            DateTime Now = DateTime.Now;

            int RpHourNow = (int)Math.Floor((double)(((Now.Minute * 60) + Now.Second) / 150)); //150sec = 2m30s = 1heure dans le rp

            int RpMinuteNow = (int)Math.Floor((((Now.Minute * 60) + Now.Second) - (RpHourNow * 150)) / 2.5);

            if (RpHourNow >= 16)
            {
                RpHourNow = (RpHourNow + 8) - 24;
            }
            else
            {
                RpHourNow = RpHourNow + 8;
            }

            if (this.RpTimeSpeed)
            {
                RpHourNow = (int)Math.Floor((double)(Now.Second / 2.5));
            }

            if (this.RpMinute != RpMinuteNow)
            {
                this.RpMinute = RpMinuteNow;
            }

            if (this.RpHour == RpHourNow)
            {
                return;
            }

            this.RpHour = RpHourNow;

            if (!this.RpCycleHourEffect)
            {
                return;
            }

            int Intensity = 255;

            if (this.RpHour >= 8 && this.RpHour < 20) //Journée
            {
                Intensity = 255;
            }
            else if (this.RpHour >= 20 && this.RpHour < 21)  //Crépuscule
            {
                Intensity = 200;
            }
            else if (this.RpHour >= 21 && this.RpHour < 22)  //Crépuscule
            {
                Intensity = 150;
            }
            else if (this.RpHour >= 22 && this.RpHour < 23)  //Crépuscule
            {
                Intensity = 100;
            }
            else if (this.RpHour >= 23 && this.RpHour < 24)  //Crépuscule
            {
                Intensity = 75;
            }
            else if (this.RpHour >= 0 && this.RpHour < 4)  //Nuit
            {
                Intensity = 50;
            }
            else if (this.RpHour >= 4 && this.RpHour < 5)  //Aube
            {
                Intensity = 75;
            }
            else if (this.RpHour >= 5 && this.RpHour < 6)  //Aube
            {
                Intensity = 100;
            }
            else if (this.RpHour >= 6 && this.RpHour < 7)  //Aube
            {
                Intensity = 150;
            }
            else if (this.RpHour >= 7 && this.RpHour < 8)  //Aube
            {
                Intensity = 200;
            }

            if (this.RpIntensity == Intensity || this.RoomData.OwnerName == "WibboParty")
            {
                return;
            }

            this.RpIntensity = Intensity;

            this.UpdateRpMoodLight();
            this.UpdateRpToner();
            this.UpdateRpBlock();
        }

        private void UpdateRpBlock()
        {
            List<Item> roomItems = this.GetRoomItemHandler().GetFloor.Where(i => i.GetBaseItem().Id == 99138022).ToList();
            if (roomItems == null)
            {
                return;
            }

            int UseNum = 0;
            if (this.RpIntensity == 50)
            {
                UseNum = 0;
            }
            else if (this.RpIntensity == 75)
            {
                UseNum = 1;
            }
            else if (this.RpIntensity == 100)
            {
                UseNum = 2;
            }
            else if (this.RpIntensity == 150)
            {
                UseNum = 3;
            }
            else if (this.RpIntensity == 200)
            {
                UseNum = 4;
            }
            else if (this.RpIntensity == 255)
            {
                UseNum = 5;
            }

            foreach (Item RoomItem in roomItems)
            {
                RoomItem.ExtraData = UseNum.ToString();
                RoomItem.UpdateState();
            }
        }

        private void UpdateRpMoodLight()
        {
            if (this.MoodlightData == null)
            {
                return;
            }

            Item roomItem = this.GetRoomItemHandler().GetItem(this.MoodlightData.ItemId);
            if (roomItem == null || roomItem.GetBaseItem().InteractionType != InteractionType.MOODLIGHT)
            {
                return;
            }

            this.MoodlightData.Enabled = true;
            this.MoodlightData.CurrentPreset = 1;
            this.MoodlightData.UpdatePreset(1, "#000000", this.RpIntensity, false);
            roomItem.ExtraData = this.MoodlightData.GenerateExtraData();
            roomItem.UpdateState();
        }

        private void UpdateRpToner()
        {
            Item roomItem = Enumerable.FirstOrDefault<Item>(this.GetRoomItemHandler().GetFloor.Where(i => i.GetBaseItem().InteractionType == InteractionType.TONER));
            if (roomItem == null)
            {
                return;
            }

            int Teinte = 135;
            int Saturation = 180;
            int Luminosite = (int)Math.Floor((double)this.RpIntensity / 2);
            roomItem.ExtraData = "on," + Teinte + "," + Saturation + "," + Luminosite;
            roomItem.UpdateState(true, true);
        }

        public void OnRoomCrash(Exception e)
        {
            Logging.LogThreadException((e).ToString(), "Room cycle task for room " + this.Id);
            //ButterflyEnvironment.GetGame().GetRoomManager().UnloadRoom(this);
        }

        public void SendPacketOnChat(IServerPacket Message, RoomUser ThisUser = null, bool UserMutedOnly = false, bool UserNotIngameOnly = false)
        {
            try
            {
                if (Message == null)
                {
                    return;
                }

                if (this == null || this.roomUserManager == null)
                {
                    return;
                }

                List<RoomUser> Users = this.roomUserManager.GetUserList().ToList();
                if (Users == null)
                {
                    return;
                }

                foreach (RoomUser User in Users)
                {
                    if (User == null || User.IsBot)
                    {
                        continue;
                    }

                    if (User.GetClient() == null || User.GetClient().GetConnection() == null || User.GetClient().GetHabbo() == null)
                    {
                        continue;
                    }

                    if (UserMutedOnly && ThisUser != null && User.GetClient().GetHabbo().MutedUsers.Contains(ThisUser.UserId))
                    {
                        continue;
                    }

                    if (ThisUser != null && ThisUser.GetClient() != null && ThisUser.GetClient().GetHabbo() != null && ThisUser.GetClient().GetHabbo().IgnoreAll && ThisUser != User)
                    {
                        continue;
                    }

                    if (!UserMutedOnly && ThisUser == User)
                    {
                        continue;
                    }

                    if (this.RoomIngameChat && (UserNotIngameOnly && User.Team != Team.none))
                    {
                        continue;
                    }

                    if (this.RoomData.ChatMaxDistance > 0 && (Math.Abs(ThisUser.X - User.X) > this.RoomData.ChatMaxDistance || Math.Abs(ThisUser.Y - User.Y) > this.RoomData.ChatMaxDistance))
                    {
                        continue;
                    }

                    User.GetClient().SendPacket(Message);
                }
            }
            catch (Exception ex)
            {
                Logging.HandleException(ex, "Room.SendMessage (" + this.Id + ")");
            }
        }

        public void SendPacketWeb(IServerPacket Message)
        {
            try
            {
                if (Message == null)
                {
                    return;
                }

                if (this == null || this.roomUserManager == null)
                {
                    return;
                }

                List<RoomUser> Users = this.roomUserManager.GetUserList().ToList();
                if (Users == null)
                {
                    return;
                }

                foreach (RoomUser User in Users)
                {
                    if (User == null || User.IsBot)
                    {
                        continue;
                    }

                    if (User.GetClient() == null || User.GetClient().GetConnection() == null)
                    {
                        continue;
                    }

                    User.GetClient().GetHabbo().SendWebPacket(Message);
                }
            }
            catch (Exception ex)
            {
                Logging.HandleException(ex, "Room.SendMessageWeb (" + this.Id + ")");
            }
        }
        public void SendPacket(IServerPacket Message, bool UsersWithRightsOnly = false)
        {
            try
            {
                if (Message == null)
                {
                    return;
                }

                if (this == null || this.roomUserManager == null)
                {
                    return;
                }

                List<RoomUser> Users = this.roomUserManager.GetUserList().ToList();
                if (Users == null)
                {
                    return;
                }

                foreach (RoomUser User in Users)
                {
                    if (User == null || User.IsBot)
                    {
                        continue;
                    }

                    if (User.GetClient() == null || User.GetClient().GetConnection() == null)
                    {
                        continue;
                    }

                    if (UsersWithRightsOnly && !this.CheckRights(User.GetClient()))
                    {
                        continue;
                    }

                    User.GetClient().SendPacket(Message);
                }
            }
            catch (Exception ex)
            {
                Logging.HandleException(ex, "Room.SendMessage (" + this.Id + ")");
            }
        }

        public void SendMessage(List<ServerPacket> Messages)
        {
            if (Messages.Count == 0)
            {
                return;
            }

            try
            {
                byte[] TotalBytes = new byte[0];
                int Current = 0;

                foreach (ServerPacket Packet in Messages.ToList())
                {
                    byte[] ToAdd = Packet.GetBytes();
                    int NewLen = TotalBytes.Length + ToAdd.Length;

                    Array.Resize(ref TotalBytes, NewLen);

                    for (int i = 0; i < ToAdd.Length; i++)
                    {
                        TotalBytes[Current] = ToAdd[i];
                        Current++;
                    }
                }

                this.BroadcastPacket(TotalBytes);
            }
            catch (Exception e)
            {
                Logging.HandleException(e, "Room.SendMessage List<ServerPacket>");
            }
        }

        public void BroadcastPacket(byte[] Packet)
        {
            foreach (RoomUser User in this.roomUserManager.GetUserList().ToList())
            {
                if (User == null || User.IsBot)
                {
                    continue;
                }

                if (User.GetClient() == null || User.GetClient().GetConnection() == null)
                {
                    continue;
                }

                User.GetClient().GetConnection().SendData(Packet);
            }
        }

        public void Destroy()
        {
            this.SendPacket(new CloseConnectionComposer());
            this.Dispose();
        }

        private void Dispose()
        {
            if (this.Disposed)
            {
                return;
            }

            this.Disposed = true;
            this.mCycleEnded = true;

            foreach (CancellationTokenSource tokenSource in this.cancellationTokenSources)
            {
                tokenSource.Cancel();
            }
            this.cancellationTokenSources.Clear();

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                this.GetRoomItemHandler().SaveFurniture(dbClient);
            }
            this.RoomData.Tags.Clear();

            this.UsersWithRights.Clear();
            this._bans.Clear();
            foreach (Item roomItem in this.GetRoomItemHandler().GetWallAndFloor)
            {
                roomItem.Destroy();
            }

            this.GetRoomItemHandler().Destroy();

            this.ActiveTrades.Clear();

            this.GetRoomUserManager().UpdateUserCount(0);
            this.GetRoomUserManager().Destroy();

            this.gamemap.Destroy();
        }

        public Dictionary<int, double> getBans()
        {
            return this._bans;
        }

        public bool UserIsBanned(int pId)
        {
            return this._bans.ContainsKey(pId);
        }

        public void RemoveBan(int pId)
        {
            this._bans.Remove(pId);
        }

        public void AddBan(int pId, int Time)
        {
            if (this._bans.ContainsKey(pId))
            {
                return;
            }

            this._bans.Add(pId, ButterflyEnvironment.GetUnixTimestamp() + Time);
        }

        public bool HasBanExpired(int pId)
        {
            return !this.UserIsBanned(pId) || this._bans[pId] - ButterflyEnvironment.GetUnixTimestamp() <= 0.0;
        }

        public Dictionary<int, double> getMute()
        {
            return this._mutes;
        }

        public bool UserIsMuted(int pId)
        {
            return this._mutes.ContainsKey(pId);
        }

        public void RemoveMute(int pId)
        {
            this._mutes.Remove(pId);
        }

        public void AddMute(int pId, int Time)
        {
            if (this._mutes.ContainsKey(pId))
            {
                return;
            }

            this._mutes.Add(pId, ButterflyEnvironment.GetUnixTimestamp() + Time);
        }

        public bool HasMuteExpired(int pId)
        {
            return !this.UserIsMuted(pId) || this._mutes[pId] - ButterflyEnvironment.GetUnixTimestamp() <= 0.0;
        }

        public bool HasActiveTrade(RoomUser User)
        {
            if (User.IsBot)
            {
                return false;
            }
            else
            {
                return this.HasActiveTrade(User.GetClient().GetHabbo().Id);
            }
        }

        public bool HasActiveTrade(int UserId)
        {
            foreach (Trade trade in this.ActiveTrades)
            {
                if (trade.ContainsUser(UserId))
                {
                    return true;
                }
            }
            return false;
        }

        public Trade GetUserTrade(int UserId)
        {
            foreach (Trade trade in this.ActiveTrades)
            {
                if (trade.ContainsUser(UserId))
                {
                    return trade;
                }
            }
            return null;
        }

        public void TryStartTrade(RoomUser UserOne, RoomUser UserTwo)
        {
            if (UserOne == null || UserTwo == null)
            {
                return;
            }

            if ((UserOne.IsBot || UserTwo.IsBot) || (UserOne.IsTrading || UserTwo.IsTrading || (this.HasActiveTrade(UserOne) || this.HasActiveTrade(UserTwo))))
            {
                return;
            }

            this.ActiveTrades.Add(new Trade(UserOne.GetClient().GetHabbo().Id, UserTwo.GetClient().GetHabbo().Id, this.Id));
        }

        public void TryStopTrade(int UserId)
        {
            Trade userTrade = this.GetUserTrade(UserId);
            if (userTrade == null)
            {
                return;
            }

            userTrade.CloseTrade(UserId);
            this.ActiveTrades.Remove(userTrade);
        }

        public void SetMaxUsers(int MaxUsers)
        {
            this.RoomData.UsersMax = MaxUsers;
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                RoomDao.UpdateUsersMax(dbClient, this.Id, MaxUsers);
            }
        }

        public void SetTimeout(int delay, Action callBack)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            Task.Delay(delay).ContinueWith((t) =>
            {
                if (this.cancellationTokenSources.Contains(cancellationTokenSource))
                    this.cancellationTokenSources.Remove(cancellationTokenSource);

                if (this.Disposed) return;

                callBack();

            }, cancellationToken);


            if(!this.cancellationTokenSources.Contains(cancellationTokenSource))
                this.cancellationTokenSources.Add(cancellationTokenSource);
        }
    }
}