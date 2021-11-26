using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.Rooms.Notifications;

using Butterfly.Core;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Users.Messenger;
using ConnectionManager;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Butterfly.Game.Clients
{
    public class ClientManager
    {
        public ConcurrentDictionary<int, Client> _clients;
        public ConcurrentDictionary<string, int> _usernameRegister;
        public ConcurrentDictionary<int, int> _userIDRegister;

        public int OnlineNitroUsers;
        public int OnlineUsersFr;
        public int OnlineUsersEn;
        public int OnlineUsersBr;

        private readonly List<int> _userStaff;

        public int Count => this._userIDRegister.Count;

        public ClientManager()
        {
            this._clients = new ConcurrentDictionary<int, Client>();
            this._usernameRegister = new ConcurrentDictionary<string, int>();
            this._userIDRegister = new ConcurrentDictionary<int, int>();
            this._userStaff = new List<int>();
        }

        public List<Client> GetStaffUsers()
        {
            List<Client> Users = new List<Client>();

            foreach (int UserId in this._userStaff)
            {
                Client Client = this.GetClientByUserID(UserId);
                if (Client == null || Client.GetHabbo() == null)
                {
                    continue;
                }

                Users.Add(Client);
            }

            return Users;
        }

        public Client GetClientByUserID(int userID)
        {
            if (this._userIDRegister.ContainsKey(userID))
            {
                if (!this.TryGetClient(this._userIDRegister[userID], out Client Client))
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

        public Client GetClientByUsername(string username)
        {
            if (this._usernameRegister.ContainsKey(username.ToLower()))
            {
                if (!this.TryGetClient(this._usernameRegister[username.ToLower()], out Client Client))
                {
                    return null;
                }

                return Client;
            }
            return null;
        }

        public bool UpdateClientUsername(int ClientId, string OldUsername, string NewUsername)
        {
            if (!this._usernameRegister.ContainsKey(OldUsername.ToLower()))
            {
                return false;
            }

            this._usernameRegister.TryRemove(OldUsername.ToLower(), out ClientId);
            this._usernameRegister.TryAdd(NewUsername.ToLower(), ClientId);
            return true;
        }

        public bool TryGetClient(int ClientId, out Client Client)
        {
            return this._clients.TryGetValue(ClientId, out Client);
        }

        public string GetNameById(int Id)
        {
            Client clientByUserId = this.GetClientByUserID(Id);

            if (clientByUserId != null)
            {
                return clientByUserId.GetHabbo().Username;
            }

            string username = "";
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                username = UserDao.GetNameById(dbClient, Id);
            }

            return username;
        }

        public List<Client> GetClientsById(Dictionary<int, MessengerBuddy>.KeyCollection users)
        {
            List<Client> ClientOnline = new List<Client>();
            foreach (int userID in users)
            {
                Client client = this.GetClientByUserID(userID);
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
                Client Client = this.GetClientByUserID(UserId);
                if (Client == null || Client.GetHabbo() == null)
                {
                    continue;
                }

                Client.SendPacket(Packet);
            }
        }

        public void SendMessage(IServerPacket Packet)
        {
            foreach (Client Client in this._clients.Values.ToList())
            {
                if (Client == null || Client.GetHabbo() == null)
                {
                    continue;
                }

                Client.SendPacket(Packet);
            }
        }

        public void CreateAndStartClient(int clientID, ConnectionInformation connection)
        {
            Client Client = new Client(clientID, connection);
            if (this._clients.TryAdd(Client.ConnectionID, Client))
            {
                Client.StartConnection();
            }
            else
            {
                connection.Dispose();
            }
        }

        public void DisposeConnection(int clientID)
        {
            if (!this.TryGetClient(clientID, out Client Client))
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
            Client clientByUserId = this.GetClientByUserID(UserID);
            if (clientByUserId == null)
            {
                return;
            }

            clientByUserId.Disconnect();
        }

        public void RegisterClient(Client client, int userID, string username)
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
            this._userIDRegister.TryRemove(userid, out int Client);
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

            foreach (Client client in this.GetClients.ToList())
            {
                if (client == null)
                {
                    continue;
                }

                if (client.GetHabbo() != null)
                {
                    try
                    {
                        TimeSpan TimeOnline = DateTime.Now - client.GetHabbo().OnlineTime;
                        int TimeOnlineSec = (int)TimeOnline.TotalSeconds;

                        stringBuilder.Append(UserDao.BuildUpdateQuery(client.GetHabbo().Id, client.GetHabbo().Duckets, client.GetHabbo().Credits));
                        stringBuilder.Append(UserStatsDao.BuildUpdateQuery(client.GetHabbo().Id, client.GetHabbo().FavouriteGroupId, TimeOnlineSec, client.GetHabbo().CurrentQuestId, client.GetHabbo().Respect, client.GetHabbo().DailyRespectPoints, client.GetHabbo().DailyPetRespectPoints));

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
                    using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery((stringBuilder).ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.HandleException(ex, "GameClientManager.CloseAll()");
            }
            Console.WriteLine("Done saving users inventory!");
            Console.WriteLine("Closing server connections...");
            try
            {
                foreach (Client client in this.GetClients.ToList())
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
                Logging.LogCriticalException((ex).ToString());
            }
            this._clients.Clear();
            Console.WriteLine("Connections closed!");
        }

        public void BanUser(Client Client, string Moderator, double LengthSeconds, string Reason, bool IpBan, bool MachineBan)
        {
            if (string.IsNullOrEmpty(Reason))
            {
                Reason = "Ne respect pas les régles";
            }

            string Variable = Client.GetHabbo().Username.ToLower();
            string str = "user";
            double Expire = ButterflyEnvironment.GetUnixTimestamp() + LengthSeconds;
            if (IpBan)
            {
                //Variable = Client.GetConnection().getIp();
                Variable = Client.GetHabbo().IP;
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

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                if (str == "user")
                {
                    UserDao.UpdateIsBanned(dbClient, Client.GetHabbo().Id);
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

        public ICollection<Client> GetClients => this._clients.Values;
    }
}
