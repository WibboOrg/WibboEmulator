using WibboEmulator.Communication.Interfaces;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Notifications;
using WibboEmulator.Communication.WebSocket;
using WibboEmulator.Core;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.GameClients.Messenger;
using System.Collections.Concurrent;
using System.Text;

namespace WibboEmulator.Games.GameClients
{
    public class ClientManager
    {
        public ConcurrentDictionary<string, GameClient> _clients;
        public ConcurrentDictionary<string, string> _usernameRegister;
        public ConcurrentDictionary<int, string> _userIDRegister;

        public int OnlineUsersFr;
        public int OnlineUsersEn;
        public int OnlineUsersBr;

        private readonly List<int> _userStaff;

        public int Count => this._userIDRegister.Count;

        public ClientManager()
        {
            this._clients = new ConcurrentDictionary<string, GameClient>();
            this._usernameRegister = new ConcurrentDictionary<string, string>();
            this._userIDRegister = new ConcurrentDictionary<int, string>();
            this._userStaff = new List<int>();
        }

        public List<GameClient> GetStaffUsers()
        {
            List<GameClient> Users = new List<GameClient>();

            foreach (int UserId in this._userStaff)
            {
                GameClient Client = this.GetClientByUserID(UserId);
                if (Client == null || Client.GetUser() == null)
                {
                    continue;
                }

                Users.Add(Client);
            }

            return Users;
        }

        public GameClient GetClientById(string clientID)
        {
            this.TryGetClient(clientID, out GameClient client);

            return client;
        }

        public GameClient GetClientByUserID(int userID)
        {
            if (this._userIDRegister.ContainsKey(userID))
            {
                if (!this.TryGetClient(this._userIDRegister[userID], out GameClient Client))
                {
                    return null;
                }

                return Client;
            }
            else
            {
                return null;
            }
        }

        public GameClient GetClientByUsername(string username)
        {
            if (this._usernameRegister.ContainsKey(username.ToLower()))
            {
                if (!this.TryGetClient(this._usernameRegister[username.ToLower()], out GameClient Client))
                {
                    return null;
                }

                return Client;
            }
            return null;
        }

        public bool UpdateClientUsername(string ClientId, string OldUsername, string NewUsername)
        {
            if (!this._usernameRegister.ContainsKey(OldUsername.ToLower()))
            {
                return false;
            }

            this._usernameRegister.TryRemove(OldUsername.ToLower(), out ClientId);
            this._usernameRegister.TryAdd(NewUsername.ToLower(), ClientId);
            return true;
        }

        public bool TryGetClient(string ClientId, out GameClient Client)
        {
            return this._clients.TryGetValue(ClientId, out Client);
        }

        public string GetNameById(int Id)
        {
            GameClient clientByUserId = this.GetClientByUserID(Id);

            if (clientByUserId != null)
            {
                return clientByUserId.GetUser().Username;
            }

            string username = "";
            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                username = UserDao.GetNameById(dbClient, Id);
            }

            return username;
        }

        public List<GameClient> GetClientsById(Dictionary<int, MessengerBuddy>.KeyCollection users)
        {
            List<GameClient> ClientOnline = new List<GameClient>();
            foreach (int userID in users)
            {
                GameClient client = this.GetClientByUserID(userID);
                if (client != null)
                {
                    ClientOnline.Add(client);
                }
            }

            return ClientOnline;
        }

        public void SendMessageStaff(IServerPacket Packet)
        {
            foreach (int UserId in this._userStaff)
            {
                GameClient Client = this.GetClientByUserID(UserId);
                if (Client == null || Client.GetUser() == null)
                {
                    continue;
                }

                Client.SendPacket(Packet);
            }
        }

        public void SendMessage(IServerPacket Packet)
        {
            foreach (GameClient Client in this._clients.Values.ToList())
            {
                if (Client == null || Client.GetUser() == null)
                {
                    continue;
                }

                Client.SendPacket(Packet);
            }
        }

        public void CreateAndStartClient(string clientID, GameWebSocket connection)
        {
            GameClient Client = new GameClient(clientID, connection);
            if (!this._clients.TryAdd(clientID, Client))
                connection.Dispose();
        }

        public void DisposeConnection(string clientID)
        {
            if (!this.TryGetClient(clientID, out GameClient Client))
            {
                return;
            }

            if (Client != null)
            {
                Client.Dispose();
            }

            this._clients.TryRemove(clientID, out Client);
        }

        public void LogClonesOut(int UserID)
        {
            GameClient clientByUserId = this.GetClientByUserID(UserID);
            if (clientByUserId == null)
            {
                return;
            }

            clientByUserId.Disconnect();
        }

        public void RegisterClient(GameClient client, int userID, string username)
        {
            if (this._usernameRegister.ContainsKey(username.ToLower()))
            {
                this._usernameRegister[username.ToLower()] = client.ConnectionID;
            }
            else
            {
                this._usernameRegister.TryAdd(username.ToLower(), client.ConnectionID);
            }

            if (this._userIDRegister.ContainsKey(userID))
            {
                this._userIDRegister[userID] = client.ConnectionID;
            }
            else
            {
                this._userIDRegister.TryAdd(userID, client.ConnectionID);
            }
        }

        public void UnregisterClient(int userid, string username)
        {
            this._userIDRegister.TryRemove(userid, out string Client);
            this._usernameRegister.TryRemove(username.ToLower(), out Client);
        }

        public void AddUserStaff(int UserId)
        {
            if (!this._userStaff.Contains(UserId))
            {
                this._userStaff.Add(UserId);
            }
        }

        public void RemoveUserStaff(int UserId)
        {
            if (this._userStaff.Contains(UserId))
            {
                this._userStaff.Remove(UserId);
            }
        }

        public void CloseAll()
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (GameClient client in this.GetClients.ToList())
            {
                if (client == null)
                {
                    continue;
                }

                if (client.GetUser() != null)
                {
                    try
                    {
                        TimeSpan TimeOnline = DateTime.Now - client.GetUser().OnlineTime;
                        int TimeOnlineSec = (int)TimeOnline.TotalSeconds;

                        stringBuilder.Append(UserDao.BuildUpdateQuery(client.GetUser().Id, client.GetUser().Duckets, client.GetUser().Credits));
                        stringBuilder.Append(UserStatsDao.BuildUpdateQuery(client.GetUser().Id, client.GetUser().FavouriteGroupId, TimeOnlineSec, client.GetUser().CurrentQuestId, client.GetUser().Respect, client.GetUser().DailyRespectPoints, client.GetUser().DailyPetRespectPoints));

                    }
                    catch
                    {
                    }
                }
            }
            try
            {
                if (stringBuilder.Length > 0)
                {
                    using IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
                    dbClient.RunQuery((stringBuilder).ToString());
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.HandleException(ex, "GameClientManager.CloseAll()");
            }
            Console.WriteLine("Done saving users inventory!");
            Console.WriteLine("Closing server connections...");
            try
            {
                foreach (GameClient client in this.GetClients.ToList())
                {

                    if (client == null || client.GetConnection() == null)
                    {
                        continue;
                    }

                    try
                    {
                        client.GetConnection().Dispose();
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogCriticalException((ex).ToString());
            }
            this._clients.Clear();
            Console.WriteLine("Connections closed!");
        }

        public void BanUser(GameClient Client, string Moderator, double LengthSeconds, string Reason, bool IpBan, bool MachineBan)
        {
            if (string.IsNullOrEmpty(Reason))
            {
                Reason = "Non respect des règles de conditions générales d'utilisations ainsi que la Wibbo Attitude";
            }

            string Variable = Client.GetUser().Username.ToLower();
            string str = "user";
            double Expire = WibboEnvironment.GetUnixTimestamp() + LengthSeconds;
            if (IpBan)
            {
                //Variable = Client.GetConnection().getIp();
                Variable = Client.GetUser().IP;
                str = "ip";
            }

            if (MachineBan)
            {
                Variable = Client.MachineId;
                str = "machine";
            }

            if (Variable == "")
            {
                return;
            }

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                if (str == "user")
                {
                    UserDao.UpdateIsBanned(dbClient, Client.GetUser().Id);
                }

                BanDao.InsertBan(dbClient, Expire, str, Variable, Reason, Moderator);
            }

            if (MachineBan)
            {
                this.BanUser(Client, Moderator, LengthSeconds, Reason, true, false);
            }
            else if (IpBan)
            {
                this.BanUser(Client, Moderator, LengthSeconds, Reason, false, false);
            }
            else
            {
                Client.Disconnect();
            }
        }

        public void SendSuperNotif(string Title, string Notice, string Picture, string Link, string LinkTitle, bool Broadcast, bool Event)
        {
            this.SendMessage(new RoomNotificationComposer(Title, Notice, Picture, LinkTitle, Link));
        }

        public ICollection<GameClient> GetClients => this._clients.Values;
    }
}
