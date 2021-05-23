using Buttefly.Communication.Encryption.Crypto.Prng;
using Butterfly.Communication.Packets.Incoming;
using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.Communication.Packets.Outgoing.WebSocket;
using Butterfly.Core;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.Users;
using Butterfly.HabboHotel.Users.UserData;
using Butterfly.Net;
using ConnectionManager;
using System;
using System.Text;

namespace Butterfly.HabboHotel.GameClients
{
    public class GameClient
    {
        private ConnectionInformation Connection;
        private GamePacketParser packetParser;
        private Habbo Habbo;

        public string MachineId;
        public Language Langue;

        public ARC4 RC4Client = null;

        public int ConnectionID;

        public GameClient(int ClientId, ConnectionInformation pConnection)
        {
            this.ConnectionID = ClientId;
            this.Langue = Language.FRANCAIS;
            this.Connection = pConnection;
            this.packetParser = new GamePacketParser(this);
        }

        private void SwitchParserRequest()
        {
            this.packetParser.OnNewPacket += new GamePacketParser.HandlePacket(this.onNewPacket);

            byte[] packet = (this.Connection.Parser as InitialPacketParser).CurrentData;
            this.Connection.Parser.Dispose();
            this.Connection.Parser = this.packetParser;
            this.Connection.Parser.HandlePacketData(packet);
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
                    ButterflyEnvironment.GetGame().GetClientManager().LogClonesOut(userData.userID);
                    this.Habbo = userData.user;
                    this.Langue = this.Habbo.Langue;

                    ButterflyEnvironment.GetGame().GetClientManager().RegisterClient(this, userData.userID, this.Habbo.Username);

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

                    if(this.Connection.IsWebSocket)
                    {
                        ButterflyEnvironment.GetGame().GetClientManager().OnlineNitroUsers++;
                    }

                    if (this.Habbo.MachineId != this.MachineId)
                    {
                        using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            queryreactor.SetQuery("UPDATE users SET machine_id = @machineid WHERE id = '" + this.Habbo.Id + "'");
                            queryreactor.AddParameter("machineid", (this.MachineId != null) ? this.MachineId : "");
                            queryreactor.RunQuery();
                        }
                    }

                    this.Habbo.Init(this, userData);
                    this.Habbo.LoadData(userData);

                    this.IsNewUser();

                    this.SendPacket(new AuthenticationOKComposer());
                    this.SendPacket(new NavigatorSettingsComposer(this.Habbo.HomeRoom));
                    this.SendPacket(new FavouritesComposer(this.Habbo.FavoriteRooms));
                    this.SendPacket(new FigureSetIdsComposer());
                    this.SendPacket(new UserRightsComposer(this.Habbo.Rank < 2 ? 2 : this.GetHabbo().Rank));
                    this.SendPacket(new AvailabilityStatusComposer());
                    this.SendPacket(new AchievementScoreComposer(this.Habbo.AchievementPoints));
                    this.SendPacket(new BuildersClubMembershipComposer());
                    this.SendPacket(new ActivityPointsComposer(this.Habbo.Duckets, this.Habbo.WibboPoints));
                    this.SendPacket(new CfhTopicsInitComposer(ButterflyEnvironment.GetGame().GetModerationManager().UserActionPresets));
                    this.SendPacket(new SoundSettingsComposer(this.Habbo.ClientVolume, false, false, false, 1));
                    this.SendPacket(new AvatarEffectsComposer(ButterflyEnvironment.GetGame().GetEffectsInventoryManager().GetEffects()));

                    this.Habbo.UpdateActivityPointsBalance();
                    this.Habbo.UpdateCreditsBalance();
                    this.Habbo.UpdateWPBalance();

                    if (this.Habbo.HasFuse("fuse_mod"))
                    {
                        ButterflyEnvironment.GetGame().GetClientManager().AddUserStaff(this.Habbo.Id);
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
                    dbClient.SetQuery("INSERT INTO rooms (caption,description,owner,model_name,category,state, wallpaper, floor, landscape, allow_hidewall, wallthick, floorthick) SELECT @caption, @desc, @username, @model, category, state, wallpaper, floor, landscape, allow_hidewall, wallthick, floorthick FROM rooms WHERE id = '5328079'");
                    dbClient.AddParameter("caption", this.GetHabbo().Username);
                    dbClient.AddParameter("desc", ButterflyEnvironment.GetLanguageManager().TryGetValue("room.welcome.desc", this.Langue));
                    dbClient.AddParameter("username", this.GetHabbo().Username);
                    dbClient.AddParameter("model", "model_welcome");
                    RoomId = Convert.ToInt32(dbClient.InsertQuery());

                    dbClient.RunQuery("UPDATE users SET nux_enable = '0', home_room = '" + RoomId + "' WHERE id = '" + this.GetHabbo().Id + "';");
                    if (RoomId == 0)
                    {
                        return;
                    }

                    dbClient.RunQuery("INSERT INTO items (user_id, room_id, base_item, extra_data, x, y, z, rot) SELECT '" + this.GetHabbo().Id + "', '" + RoomId + "', base_item, extra_data, x, y, z, rot FROM items WHERE room_id = '5328079'");
                }

                this.GetHabbo().UsersRooms.Add(ButterflyEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomId));
                this.GetHabbo().HomeRoom = RoomId;

                ServerPacket nuxStatus = new ServerPacket(ServerPacketHeader.NuxAlertComposer);
                nuxStatus.WriteInteger(2);
                this.SendPacket(nuxStatus);

                this.SendPacket(new NuxAlertComposer("nux/lobbyoffer/hide"));
            }
        }

        private void onNewPacket(ClientPacket Message)
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
            return this.Connection;
        }

        public Habbo GetHabbo()
        {
            return this.Habbo;
        }

        public void StartConnection()
        {
            if (this.Connection == null)
            {
                return;
            } (this.Connection.Parser as InitialPacketParser).SwitchParserRequest += new InitialPacketParser.NoParamDelegate(this.SwitchParserRequest);

            this.Connection.StartPacketProcessing();
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

            using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("INSERT INTO chatlogs (user_id, room_id, user_name, timestamp, message, type) VALUES ('" + this.GetHabbo().Id + "', '" + RoomId + "', @username, UNIX_TIMESTAMP(), @message, @type)");
                queryreactor.AddParameter("message", Message);
                queryreactor.AddParameter("type", type);
                queryreactor.AddParameter("username", this.GetHabbo().Username);
                queryreactor.RunQuery();
            }

            if (!ButterflyEnvironment.GetGame().GetChatManager().GetFilter().Ispub(Message))
            {
                if (ButterflyEnvironment.GetGame().GetChatManager().GetFilter().CheckMessageWord(Message))
                {
                    using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        queryreactor.SetQuery("INSERT INTO chatlogs_pub (user_id, user_name, timestamp, message) VALUES ('" + this.GetHabbo().Id + "', @pseudo, UNIX_TIMESTAMP(), @message)");
                        queryreactor.AddParameter("message", "A vérifié: " + type + Message);
                        queryreactor.AddParameter("pseudo", this.GetHabbo().Username);
                        queryreactor.RunQuery();
                    }

                    foreach (GameClient Client in ButterflyEnvironment.GetGame().GetClientManager().GetStaffUsers())
                    {
                        if (Client == null || Client.GetHabbo() == null)
                        {
                            continue;
                        }

                        Client.GetHabbo().SendWebPacket(new AddChatlogsComposer(this.Habbo.Id, this.Habbo.Username, type + Message));
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

            if (PubCount < 3 && PubCount > 0)
            {
                this.SendNotification(string.Format(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.antipub.warn.1", this.Langue), PubCount));
                using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    queryreactor.SetQuery("INSERT INTO chatlogs_pub (user_id,user_name,timestamp,message) VALUES ('" + this.GetHabbo().Id + "',@pseudo,UNIX_TIMESTAMP(),@message)");
                    queryreactor.AddParameter("message", "Pub numero " + PubCount + ": " + type + Message);
                    queryreactor.AddParameter("pseudo", this.GetHabbo().Username);
                    queryreactor.RunQuery();
                }
            }
            else if (PubCount == 3)
            {
                this.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.antipub.warn.2", this.Langue));
                using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    queryreactor.SetQuery("INSERT INTO chatlogs_pub (user_id,user_name,timestamp,message) VALUES ('" + this.GetHabbo().Id + "',@pseudo,UNIX_TIMESTAMP(),@message)");
                    queryreactor.AddParameter("message", "Pub numero " + PubCount + ": " + type + Message);
                    queryreactor.AddParameter("pseudo", this.GetHabbo().Username);
                    queryreactor.RunQuery();
                }
            }
            else if (PubCount == 4)
            {
                using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    queryreactor.SetQuery("INSERT INTO chatlogs_pub (user_id,user_name,timestamp,message) VALUES ('" + this.GetHabbo().Id + "',@pseudo,UNIX_TIMESTAMP(),@message)");
                    queryreactor.AddParameter("message", "Pub numero " + PubCount + " bannisement: " + type + Message);
                    queryreactor.AddParameter("pseudo", this.GetHabbo().Username);
                    queryreactor.RunQuery();
                }

                ButterflyEnvironment.GetGame().GetClientManager().BanUser(this, "Robot", 86400, "Notre Robot a detecte de la pub pour sur le compte " + this.GetHabbo().Username, true, false);
            }
            else
            {
                using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    queryreactor.SetQuery("INSERT INTO chatlogs_pub (user_id,user_name,timestamp,message) VALUES ('" + this.GetHabbo().Id + "',@pseudo,UNIX_TIMESTAMP(),@message)");
                    queryreactor.AddParameter("message", "Pub numero " + PubCount + ": " + type + Message);
                    queryreactor.AddParameter("pseudo", this.GetHabbo().Username);
                    queryreactor.RunQuery();
                }
            }

            foreach (GameClient Client in ButterflyEnvironment.GetGame().GetClientManager().GetStaffUsers())
            {
                if (Client == null || Client.GetHabbo() == null)
                {
                    continue;
                }

                Client.GetHabbo().SendWebPacket(new AddChatlogsComposer(this.Habbo.Id, this.Habbo.Username, type + Message));
            }

            return true;
        }

        public void SendNotification(string Message)
        {
            this.SendPacket(new BroadcastMessageAlertComposer(Message));
        }

        public void SendHugeNotif(string Message)
        {
            ServerPacket MessageNotif = new ServerPacket(ServerPacketHeader.GENERIC_ALERT_MESSAGES);
            MessageNotif.WriteInteger(1);
            MessageNotif.WriteString(Message);
            this.SendPacket(MessageNotif);
        }

        public void Dispose()
        {
            if (this.GetHabbo() != null)
            {
                this.Habbo.OnDisconnect();
            }

            this.Habbo = null;
            this.Connection = null;
            this.packetParser = null;
            this.RC4Client = null;
        }

        public void Disconnect()
        {
            if (this.Connection != null)
            {
                this.Connection.Dispose();
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
