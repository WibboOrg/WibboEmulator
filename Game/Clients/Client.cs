using Buttefly.Communication.Encryption.Crypto.Prng;
using Butterfly.Communication.Packets.Incoming;
using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.BuildersClub;
using Butterfly.Communication.Packets.Outgoing.Handshake;
using Butterfly.Communication.Packets.Outgoing.Inventory.Achievements;
using Butterfly.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using Butterfly.Communication.Packets.Outgoing.Inventory.Purse;
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
using ConnectionManager;
using System;
using System.Text;

namespace Butterfly.Game.Clients
{
    public class Client
    {
        private ConnectionInformation _connection;
        private GamePacketParser _packetParser;
        private User _habbo;

        public string MachineId;
        public Language Langue;
        public bool IsWebSocket;

        public ARC4 RC4Client = null;

        public int ConnectionID;

        public Client(int ClientId, ConnectionInformation pConnection)
        {
            this.ConnectionID = ClientId;
            this.Langue = Language.FRANCAIS;
            this._connection = pConnection;
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
                UserData userData = UserDataFactory.GetUserData(AuthTicket, ip, this.MachineId);

                if (userData == null)
                {
                    return;
                }
                else
                {
                    ButterflyEnvironment.GetGame().GetClientManager().LogClonesOut(userData.Id);
                    this._habbo = userData.User;
                    this.Langue = this._habbo.Langue;
                    this.IsWebSocket = this._connection.IsWebSocket;

                    ButterflyEnvironment.GetGame().GetClientManager().RegisterClient(this, userData.Id, this._habbo.Username);

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

                    if (this._habbo.MachineId != this.MachineId && this.MachineId != null)
                    {
                        using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            UserDao.UpdateMachineId(dbClient, this._habbo.Id, this.MachineId);
                        }
                    }

                    this._habbo.Init(this, userData);
                    this._habbo.LoadData(userData);

                    this.IsNewUser();

                    this.SendPacket(new AuthenticationOKComposer());
                    this.SendPacket(new NavigatorSettingsComposer(this._habbo.HomeRoom));
                    this.SendPacket(new FavouritesComposer(this._habbo.FavoriteRooms));
                    this.SendPacket(new FigureSetIdsComposer());
                    this.SendPacket(new UserRightsComposer(this._habbo.Rank < 2 ? 2 : this.GetHabbo().Rank));
                    this.SendPacket(new AvailabilityStatusComposer());
                    this.SendPacket(new AchievementScoreComposer(this._habbo.AchievementPoints));
                    this.SendPacket(new BuildersClubMembershipComposer());
                    this.SendPacket(new ActivityPointsComposer(this._habbo.WibboPoints));
                    this.SendPacket(new CfhTopicsInitComposer(ButterflyEnvironment.GetGame().GetModerationManager().UserActionPresets));
                    this.SendPacket(new SoundSettingsComposer(this._habbo.ClientVolume, false, false, false, 1));
                    this.SendPacket(new AvatarEffectsComposer(ButterflyEnvironment.GetGame().GetEffectManager().GetEffects()));

                    this._habbo.UpdateActivityPointsBalance();
                    this._habbo.UpdateCreditsBalance();

                    if (this._habbo.HasFuse("fuse_mod"))
                    {
                        ButterflyEnvironment.GetGame().GetClientManager().AddUserStaff(this._habbo.Id);
                        this.SendPacket(ButterflyEnvironment.GetGame().GetModerationManager().SerializeTool());
                    }

                    return;
                }
            }
            catch (Exception ex)
            {
                Logging.LogException("Invalid Dario bug duing user login: " + (ex).ToString());
            }
        }

        private void IsNewUser()
        {
            if (this.GetHabbo().NewUser)
            {
                this.GetHabbo().NewUser = false;

                int RoomId = 0;
                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    RoomId = RoomDao.InsertDuplicate(dbClient, this.GetHabbo().Username, ButterflyEnvironment.GetLanguageManager().TryGetValue("room.welcome.desc", this.Langue));

                    UserDao.UpdateNuxEnable(dbClient, this.GetHabbo().Id, RoomId);
                    if (RoomId == 0)
                    {
                        return;
                    }

                    ItemDao.InsertDuplicate(dbClient, this.GetHabbo().Id, RoomId);
                }

                this.GetHabbo().UsersRooms.Add(ButterflyEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomId));
                this.GetHabbo().HomeRoom = RoomId;

                ServerPacket nuxStatus = new ServerPacket(ServerPacketHeader.NuxAlertComposer);
                nuxStatus.WriteInteger(2);
                this.SendPacket(nuxStatus);

                this.SendPacket(new NuxAlertComposer("nux/lobbyoffer/hide"));
            }
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

        public User GetHabbo()
        {
            return this._habbo;
        }

        public void StartConnection()
        {
            if (this._connection == null)
            {
                return;
            } (this._connection.Parser as InitialPacketParser).SwitchParserRequest += new InitialPacketParser.NoParamDelegate(this.SwitchParserRequest);

            this._connection.StartPacketProcessing();
        }

        public bool Antipub(string Message, string type, int RoomId = 0)
        {
            if (this.GetHabbo() == null)
            {
                return false;
            }

            if (this.GetHabbo().HasFuse("fuse_sysadmin"))
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
                LogChatDao.Insert(dbClient, this.GetHabbo().Id, RoomId, Message, type, this.GetHabbo().Username);
            }

            if (!ButterflyEnvironment.GetGame().GetChatManager().GetFilter().Ispub(Message))
            {
                if (ButterflyEnvironment.GetGame().GetChatManager().GetFilter().CheckMessageWord(Message))
                {
                    using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        LogChatPubDao.Insert(dbClient, this.GetHabbo().Id, "A vérifié: " + type + Message, this.GetHabbo().Username);
                    }

                    foreach (Client Client in ButterflyEnvironment.GetGame().GetClientManager().GetStaffUsers())
                    {
                        if (Client == null || Client.GetHabbo() == null)
                        {
                            continue;
                        }

                        Client.GetHabbo().SendWebPacket(new AddChatlogsComposer(this._habbo.Id, this._habbo.Username, type + Message));
                    }

                    return false;
                }

                return false;
            }

            int PubCount = this.GetHabbo().PubDectectCount++;

            if (type == "<CMD>")
            {
                PubCount = 4;
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                LogChatPubDao.Insert(dbClient, this.GetHabbo().Id, "Pub numero " + PubCount + ": " + type + Message, this.GetHabbo().Username);

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
                ButterflyEnvironment.GetGame().GetClientManager().BanUser(this, "Robot", 86400, "Notre Robot a detecte de la pub pour sur le compte " + this.GetHabbo().Username, true, false);
            }

            foreach (Client Client in ButterflyEnvironment.GetGame().GetClientManager().GetStaffUsers())
            {
                if (Client == null || Client.GetHabbo() == null)
                {
                    continue;
                }

                Client.GetHabbo().SendWebPacket(new AddChatlogsComposer(this._habbo.Id, this._habbo.Username, type + Message));
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

            if (this.GetHabbo() != null)
            {
                this._habbo.OnDisconnect();
            }

            this._habbo = null;
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

        public void SendPacket(IServerPacket Message)
        {
            if (Message == null || this.GetConnection() == null)
            {
                return;
            }

            this.GetConnection().SendData(Message.GetBytes());
        }
    }
}
