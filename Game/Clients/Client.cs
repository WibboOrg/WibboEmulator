using Wibbo.Communication.Interfaces;
using Wibbo.Communication.Packets.Outgoing;
using Wibbo.Communication.Packets.Outgoing.BuildersClub;
using Wibbo.Communication.Packets.Outgoing.Handshake;
using Wibbo.Communication.Packets.Outgoing.Help;
using Wibbo.Communication.Packets.Outgoing.Inventory.Achievements;
using Wibbo.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using Wibbo.Communication.Packets.Outgoing.Inventory.Purse;
using Wibbo.Communication.Packets.Outgoing.Misc;
using Wibbo.Communication.Packets.Outgoing.Moderation;
using Wibbo.Communication.Packets.Outgoing.Navigator;
using Wibbo.Communication.Packets.Outgoing.Notifications;
using Wibbo.Communication.Packets.Outgoing.Rooms.Chat;
using Wibbo.Communication.Packets.Outgoing.Settings;
using Wibbo.Communication.Packets.Outgoing.WibboTool;
using Wibbo.Communication.WebSocket;
using Wibbo.Core;
using Wibbo.Database.Daos;
using Wibbo.Database.Interfaces;
using Wibbo.Game.Help;
using Wibbo.Game.Rooms;
using Wibbo.Game.Users;
using Wibbo.Game.Users.Authenticator;
using Wibbo.Utilities;

namespace Wibbo.Game.Clients
{
    public class Client
    {
        private GameWebSocket _connection;
        private User _user;

        private Dictionary<int, double> _packetTimeout;
        private int _packetCount;
        private double _packetLastTimestamp;

        public string MachineId;
        public Language Langue;

        public string ConnectionID;
        public bool ShowGameAlert;

        public Client(string ClientId, GameWebSocket connection)
        {
            this.ConnectionID = ClientId;
            this.Langue = Language.FRANCAIS;
            this._connection = connection;

            this._packetTimeout = new Dictionary<int, double>();
            this._packetCount = 0;
            this._packetLastTimestamp = UnixTimestamp.GetNow();
        }

        public void TryAuthenticate(string AuthTicket)
        {
            if (string.IsNullOrEmpty(AuthTicket))
            {
                return;
            }

            try
            {
                string ip = this.GetConnection().GetIp();
                User user = UserFactory.GetUserData(AuthTicket, ip, this.MachineId);

                if (user == null)
                {
                    return;
                }
                else
                {
                    WibboEnvironment.GetGame().GetClientManager().LogClonesOut(user.Id);
                    this._user = user;
                    this.Langue = user.Langue;

                    WibboEnvironment.GetGame().GetClientManager().RegisterClient(this, user.Id, user.Username);

                    if (this.Langue == Language.FRANCAIS)
                    {
                        WibboEnvironment.GetGame().GetClientManager().OnlineUsersFr++;
                    }
                    else if (this.Langue == Language.ANGLAIS)
                    {
                        WibboEnvironment.GetGame().GetClientManager().OnlineUsersEn++;
                    }
                    else if (this.Langue == Language.PORTUGAIS)
                    {
                        WibboEnvironment.GetGame().GetClientManager().OnlineUsersBr++;
                    }

                    if (this._user.MachineId != this.MachineId && this.MachineId != null)
                    {
                        using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            UserDao.UpdateMachineId(dbClient, this._user.Id, this.MachineId);
                        }
                    }

                    this._user.Init(this);


                    this.SendPacket(new AuthenticationOKComposer());

                    ServerPacketList packetList = new ServerPacketList();
                    packetList.Add(new NavigatorHomeRoomComposer(this._user.HomeRoom, this._user.HomeRoom));
                    packetList.Add(new FavouritesComposer(this._user.FavoriteRooms));
                    packetList.Add(new FigureSetIdsComposer());
                    packetList.Add(new UserRightsComposer(this._user.Rank < 2 ? 2 : this.GetUser().Rank));
                    packetList.Add(new AvailabilityStatusComposer());
                    packetList.Add(new AchievementScoreComposer(this._user.AchievementPoints));
                    packetList.Add(new BuildersClubMembershipComposer());
                    packetList.Add(new CfhTopicsInitComposer(WibboEnvironment.GetGame().GetModerationManager().UserActionPresets));
                    packetList.Add(new UserSettingsComposer(this._user.ClientVolume, this._user.OldChat, this._user.IgnoreRoomInvites, this._user.CameraFollowDisabled, 1, 0));
                    packetList.Add(new AvatarEffectsComposer(WibboEnvironment.GetGame().GetEffectManager().GetEffects()));

                    packetList.Add(new ActivityPointNotificationComposer(this._user.Duckets, 1));
                    packetList.Add(new CreditBalanceComposer(this._user.Credits));

                    /*int day = (int)DateTime.Now.Day;
                    int days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

                    List<int> missDays = new List<int>();
                    for (int i = 0; i < day; i++)
                        missDays.Add(i);

                    packetList.Add(new CampaignCalendarDataComposer("", "", day, days, new List<int>(), missDays));
                    packetList.Add(new InClientLinkComposer("openView/calendar"));*/

                    if (this.IsNewUser())
                    {
                        packetList.Add(new NuxAlertComposer(2));
                        packetList.Add(new InClientLinkComposer("nux/lobbyoffer/hide"));
                    }

                    if (this._user.HasFuse("fuse_mod"))
                    {
                        WibboEnvironment.GetGame().GetClientManager().AddUserStaff(this._user.Id);
                        packetList.Add(new ModeratorInitComposer(
                            WibboEnvironment.GetGame().GetModerationManager().UserMessagePresets(),
                            WibboEnvironment.GetGame().GetModerationManager().RoomMessagePresets(),
                            WibboEnvironment.GetGame().GetModerationManager().Tickets()));
                    }

                    if (this._user.HasExactFuse("fuse_helptool"))
                    {
                        HelpManager guideManager = WibboEnvironment.GetGame().GetHelpManager();
                        guideManager.AddGuide(this._user.Id);
                        this._user.OnDuty = true;

                        packetList.Add(new HelperToolComposer(this._user.OnDuty, guideManager.GuidesCount));
                    }

                    this.SendPacket(packetList);

                    return;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException("Invalid Dario bug duing user login: " + (ex).ToString());
            }
        }

        private bool IsNewUser()
        {
            if (!this.GetUser().NewUser)
                return false;

            this.GetUser().NewUser = false;

            int RoomId = 0;
            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                RoomId = RoomDao.InsertDuplicate(dbClient, this.GetUser().Username, WibboEnvironment.GetLanguageManager().TryGetValue("room.welcome.desc", this.Langue));

                UserDao.UpdateNuxEnable(dbClient, this.GetUser().Id, RoomId);
                if (RoomId == 0)
                {
                    return false;
                }

                ItemDao.InsertDuplicate(dbClient, this.GetUser().Id, RoomId);
            }

            if (!this.GetUser().UsersRooms.Contains(RoomId))
                this.GetUser().UsersRooms.Add(RoomId);

            this.GetUser().HomeRoom = RoomId;

            return true;
        }

        public GameWebSocket GetConnection()
        {
            return this._connection;
        }

        public User GetUser()
        {
            return this._user;
        }

        public bool Antipub(string Message, string type, int RoomId = 0)
        {
            if (this.GetUser() == null)
            {
                return false;
            }

            if (this.GetUser().HasFuse("fuse_sysadmin"))
            {
                return false;
            }

            if (Message.Length <= 3)
            {
                return false;
            }

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                LogChatDao.Insert(dbClient, this.GetUser().Id, RoomId, Message, type, this.GetUser().Username);
            }

            if (!WibboEnvironment.GetGame().GetChatManager().GetFilter().Ispub(Message))
            {
                if (WibboEnvironment.GetGame().GetChatManager().GetFilter().CheckMessageWord(Message))
                {
                    using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        LogChatPubDao.Insert(dbClient, this.GetUser().Id, "A vérifié: " + type + Message, this.GetUser().Username);
                    }

                    foreach (Client Client in WibboEnvironment.GetGame().GetClientManager().GetStaffUsers())
                    {
                        if (Client == null || Client.GetUser() == null)
                        {
                            continue;
                        }

                        Client.SendPacket(new AddChatlogsComposer(this._user.Id, this._user.Username, type + Message));
                    }

                    return false;
                }

                return false;
            }

            int PubCount = this.GetUser().PubDectectCount++;

            if (type == "<CMD>")
            {
                PubCount = 4;
            }

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                LogChatPubDao.Insert(dbClient, this.GetUser().Id, "Pub numero " + PubCount + ": " + type + Message, this.GetUser().Username);

            if (PubCount < 3 && PubCount > 0)
            {
                this.SendNotification(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("notif.antipub.warn.1", this.Langue), PubCount));
            }
            else if (PubCount == 3)
            {
                this.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.antipub.warn.2", this.Langue));
            }
            else if (PubCount == 4)
            {
                WibboEnvironment.GetGame().GetClientManager().BanUser(this, "Robot", 86400, "Notre Robot a detecte de la pub pour sur le compte " + this.GetUser().Username, true, false);
            }

            foreach (Client Client in WibboEnvironment.GetGame().GetClientManager().GetStaffUsers())
            {
                if (Client == null || Client.GetUser() == null)
                {
                    continue;
                }

                Client.SendPacket(new AddChatlogsComposer(this._user.Id, this._user.Username, type + Message));
            }

            return true;
        }
        public void SendWhisper(string message, bool Info = true)
        {
            if (GetUser() == null || GetUser().CurrentRoom == null)
                return;

            RoomUser user = GetUser().CurrentRoom.GetRoomUserManager().GetRoomUserByName(GetUser().Username);
            if (user == null)
                return;


            SendPacket(new WhisperComposer(user.VirtualId, message, (Info) ? 34 : 0));
        }
        public void SendNotification(string Message)
        {
            SendPacket(new BroadcastMessageAlertComposer(Message));
        }

        public void SendHugeNotif(string Message)
        {
            SendPacket(new MOTDNotificationComposer(Message));
        }

        public bool PacketTimeout(int packetId, double delay)
        {
            double timestampNow = UnixTimestamp.GetNow();

            if (this._packetLastTimestamp > timestampNow)
            {
                this._packetCount++;
            }
            else
            {
                this._packetCount = 0;
                this._packetLastTimestamp = timestampNow + 1;
            }

            if (this._packetCount >= 20)
                return true;

            if (delay <= 0)
                return false;

            if (this._packetTimeout.TryGetValue(packetId, out double timestamp))
            {
                if (timestamp > timestampNow)
                    return true;

                this._packetTimeout.Remove(packetId);
            }

            this._packetTimeout.Add(packetId, timestampNow + (delay / 1000));

            return false;
        }

        public void Dispose()
        {
            if (this.Langue == Language.FRANCAIS)
            {
                WibboEnvironment.GetGame().GetClientManager().OnlineUsersFr--;
            }
            else if (this.Langue == Language.ANGLAIS)
            {
                WibboEnvironment.GetGame().GetClientManager().OnlineUsersEn--;
            }
            else if (this.Langue == Language.PORTUGAIS)
            {
                WibboEnvironment.GetGame().GetClientManager().OnlineUsersBr--;
            }

            if (this.GetUser() != null)
            {
                this._user.OnDisconnect();
            }

            if (this._packetTimeout != null)
            {
                this._packetTimeout.Clear();
            }

            this._packetTimeout = null;
            this._user = null;
            this._connection = null;
        }

        public void Disconnect()
        {
            if (this._connection != null)
            {
                this._connection.Dispose();
            }
        }

        public void SendPacket(ServerPacketList packets)
        {
            if (packets == null)
                return;

            if (packets.Count == 0)
                return;

            this.GetConnection().SendData(packets.GetBytes);
        }

        public void SendPacket(IServerPacket packet)
        {
            if (packet == null || this.GetConnection() == null)
            {
                return;
            }

            this.GetConnection().SendData(packet.GetBytes());
        }
    }
}
