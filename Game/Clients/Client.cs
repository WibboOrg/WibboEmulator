using Buttefly.Communication.Encryption.Crypto.Prng;
using Butterfly.Communication.Packets.Incoming;
using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.BuildersClub;
using Butterfly.Communication.Packets.Outgoing.Campaign;
using Butterfly.Communication.Packets.Outgoing.Handshake;
using Butterfly.Communication.Packets.Outgoing.Inventory.Achievements;
using Butterfly.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using Butterfly.Communication.Packets.Outgoing.Inventory.Purse;
using Butterfly.Communication.Packets.Outgoing.Misc;
using Butterfly.Communication.Packets.Outgoing.Moderation;
using Butterfly.Communication.Packets.Outgoing.Navigator;
using Butterfly.Communication.Packets.Outgoing.Notifications;
using Butterfly.Communication.Packets.Outgoing.Sound;
using Butterfly.Communication.Packets.Outgoing.WebSocket;
using Butterfly.Core;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Users;
using Butterfly.Game.Users.Data;
using Butterfly.Net;
using Butterfly.Utilities;
using ConnectionManager;
using System;
using System.Collections.Generic;
using System.Text;

namespace Butterfly.Game.Clients
{
    public class Client
    {
        private ConnectionInformation _connection;
        private GamePacketParser _packetParser;
        private User _user;

        private Dictionary<int, double> _packetTimeout;
        private int _packetCount;
        private double _packetLastTimestamp;

        public string MachineId;
        public Language Langue;
        public bool IsWebSocket;

        public ARC4 RC4Client = null;

        public int ConnectionID;

        public bool ShowGameAlert;

        public Client(int ClientId, ConnectionInformation connection)
        {
            this.ConnectionID = ClientId;
            this.Langue = Language.FRANCAIS;
            this._connection = connection;

            this._packetTimeout = new Dictionary<int, double>();
            this._packetCount = 0;
            this._packetLastTimestamp = UnixTimestamp.GetNow();

            this._packetParser = new GamePacketParser(this);
        }

        private void SwitchParserRequest()
        {
            this._packetParser.OnNewPacket += new GamePacketParser.HandlePacket(this.OnNewPacket);

            byte[] packet = (this._connection.Parser as InitialPacketParser).CurrentData;
            this._connection.Parser.Dispose();
            this._connection.Parser = this._packetParser;
            this._connection.Parser.HandlePacketData(packet);
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
                    ButterflyEnvironment.GetGame().GetClientManager().LogClonesOut(user.Id);
                    this._user = user;
                    this.Langue = user.Langue;
                    this.IsWebSocket = this._connection.IsWebSocket;

                    ButterflyEnvironment.GetGame().GetClientManager().RegisterClient(this, user.Id, user.Username);

                    if (this.Langue == Language.FRANCAIS)
                    {
                        ButterflyEnvironment.GetGame().GetClientManager().OnlineUsersFr++;
                    }
                    else if (this.Langue == Language.ANGLAIS)
                    {
                        ButterflyEnvironment.GetGame().GetClientManager().OnlineUsersEn++;
                    }
                    else if (this.Langue == Language.PORTUGAIS)
                    {
                        ButterflyEnvironment.GetGame().GetClientManager().OnlineUsersBr++;
                    }

                    if (this.IsWebSocket)
                    {
                        ButterflyEnvironment.GetGame().GetClientManager().OnlineNitroUsers++;
                    }

                    if (this._user.MachineId != this.MachineId && this.MachineId != null)
                    {
                        using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            UserDao.UpdateMachineId(dbClient, this._user.Id, this.MachineId);
                        }
                    }

                    this._user.Init(this);


                    this.SendPacket(new AuthenticationOKComposer());

                    ServerPacketList packetList = new ServerPacketList();
                    packetList.Add(new NavigatorSettingsComposer(this._user.HomeRoom));
                    packetList.Add(new FavouritesComposer(this._user.FavoriteRooms));
                    packetList.Add(new FigureSetIdsComposer());
                    packetList.Add(new UserRightsComposer(this._user.Rank < 2 ? 2 : this.GetUser().Rank));
                    packetList.Add(new AvailabilityStatusComposer());
                    packetList.Add(new AchievementScoreComposer(this._user.AchievementPoints));
                    packetList.Add(new BuildersClubMembershipComposer());
                    packetList.Add(new CfhTopicsInitComposer(ButterflyEnvironment.GetGame().GetModerationManager().UserActionPresets));
                    packetList.Add(new SoundSettingsComposer(this._user.ClientVolume, false, false, false, 1));
                    packetList.Add(new AvatarEffectsComposer(ButterflyEnvironment.GetGame().GetEffectManager().GetEffects()));

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
                        ButterflyEnvironment.GetGame().GetClientManager().AddUserStaff(this._user.Id);
                        packetList.Add(new ModeratorInitComposer(
                            ButterflyEnvironment.GetGame().GetModerationManager().UserMessagePresets(),
                            ButterflyEnvironment.GetGame().GetModerationManager().RoomMessagePresets(),
                            ButterflyEnvironment.GetGame().GetModerationManager().Tickets()));
                    }

                    this.SendPacket(packetList);

                    return;
                }
            }
            catch (Exception ex)
            {
                Logging.LogException("Invalid Dario bug duing user login: " + (ex).ToString());
            }
        }

        private bool IsNewUser()
        {
            if (!this.GetUser().NewUser)
                return false;

            this.GetUser().NewUser = false;

            int RoomId = 0;
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                RoomId = RoomDao.InsertDuplicate(dbClient, this.GetUser().Username, ButterflyEnvironment.GetLanguageManager().TryGetValue("room.welcome.desc", this.Langue));

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

        private void OnNewPacket(ClientPacket Message)
        {
            try
            {
                ButterflyEnvironment.GetGame().GetPacketManager().TryExecutePacket(this, Message);
            }
            catch (Exception ex)
            {
                Logging.LogPacketException(Message.ToString(), (ex).ToString());
            }
        }

        public ConnectionInformation GetConnection()
        {
            return this._connection;
        }

        public User GetUser()
        {
            return this._user;
        }

        public void StartConnection()
        {
            if (this._connection == null)
            {
                return;
            }

            (this._connection.Parser as InitialPacketParser).SwitchParserRequest += new InitialPacketParser.NoParamDelegate(this.SwitchParserRequest);

            this._connection.StartPacketProcessing();
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

            Message = Encoding.GetEncoding("UTF-8").GetString(Encoding.GetEncoding("Windows-1252").GetBytes(Message));

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                LogChatDao.Insert(dbClient, this.GetUser().Id, RoomId, Message, type, this.GetUser().Username);
            }

            if (!ButterflyEnvironment.GetGame().GetChatManager().GetFilter().Ispub(Message))
            {
                if (ButterflyEnvironment.GetGame().GetChatManager().GetFilter().CheckMessageWord(Message))
                {
                    using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        LogChatPubDao.Insert(dbClient, this.GetUser().Id, "A vérifié: " + type + Message, this.GetUser().Username);
                    }

                    foreach (Client Client in ButterflyEnvironment.GetGame().GetClientManager().GetStaffUsers())
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

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                LogChatPubDao.Insert(dbClient, this.GetUser().Id, "Pub numero " + PubCount + ": " + type + Message, this.GetUser().Username);

            if (PubCount < 3 && PubCount > 0)
            {
                this.SendNotification(string.Format(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.antipub.warn.1", this.Langue), PubCount));
            }
            else if (PubCount == 3)
            {
                this.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.antipub.warn.2", this.Langue));
            }
            else if (PubCount == 4)
            {
                ButterflyEnvironment.GetGame().GetClientManager().BanUser(this, "Robot", 86400, "Notre Robot a detecte de la pub pour sur le compte " + this.GetUser().Username, true, false);
            }

            foreach (Client Client in ButterflyEnvironment.GetGame().GetClientManager().GetStaffUsers())
            {
                if (Client == null || Client.GetUser() == null)
                {
                    continue;
                }

                Client.SendPacket(new AddChatlogsComposer(this._user.Id, this._user.Username, type + Message));
            }

            return true;
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

            if (this._packetCount >= 15)
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

            if (this.IsWebSocket)
            {
                ButterflyEnvironment.GetGame().GetClientManager().OnlineNitroUsers--;
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
            this._packetParser = null;
            this.RC4Client = null;
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
