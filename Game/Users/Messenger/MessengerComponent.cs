using Butterfly.Communication.Packets.Outgoing.Messenger;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Users.Relationships;
using Butterfly.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Butterfly.Game.Users.Messenger
{
    public class MessengerComponent : IDisposable
    {
        private readonly User _userInstance;

        public Dictionary<int, MessengerRequest> Requests { get; private set; }
        public Dictionary<int, MessengerBuddy> Friends { get; private set; }
        public Dictionary<int, Relationship> Relation { get; private set; }
        public bool AppearOffline { get; set; }

        public int Count => this.Friends.Count;

        public MessengerComponent(User user)
        {
            this._userInstance = user;

            this.Requests = new Dictionary<int, MessengerRequest>();
            this.Friends = new Dictionary<int, MessengerBuddy>();
            this.Relation = new Dictionary<int, Relationship>();
        }

        public void Init(IQueryAdapter dbClient, bool appearOffline)
        {
            DataTable dFrienShips = UserDao.GetAllFriendShips(dbClient, this._userInstance.Id);

            DataTable Requests = UserDao.GetAllFriendRequests(dbClient, this._userInstance.Id);

            foreach (DataRow dataRow in dFrienShips.Rows)
            {
                int userId = Convert.ToInt32(dataRow["id"]);
                string pUsername = (string)dataRow["username"];
                int Relation = Convert.ToInt32(dataRow["relation"]);
                if (userId != this._userInstance.Id)
                {
                    if (!this.Friends.ContainsKey(userId))
                    {
                        this.Friends.Add(userId, new MessengerBuddy(userId, pUsername, "", Relation));
                        if (Relation != 0)
                        {
                            this.Relation.Add(userId, new Relationship(userId, Relation));
                        }
                    }
                }
            }

            foreach (DataRow dataRow in Requests.Rows)
            {
                int fromId = Convert.ToInt32(dataRow["from_id"]);
                int toId = Convert.ToInt32(dataRow["to_id"]);
                string username = (string)dataRow["username"];

                if (fromId != this._userInstance.Id)
                {
                    if (!this.Requests.ContainsKey(fromId))
                    {
                        this.Requests.Add(fromId, new MessengerRequest(this._userInstance.Id, fromId, username));
                    }
                }
                else if (!this.Requests.ContainsKey(toId))
                {
                    this.Requests.Add(toId, new MessengerRequest(this._userInstance.Id, toId, username));
                }
            }

            this.AppearOffline = appearOffline;
        }

        public void ClearRequests()
        {
            this.Requests.Clear();
        }

        public MessengerRequest GetRequest(int senderID)
        {
            if (this.Requests.ContainsKey(senderID))
            {
                return this.Requests[senderID];
            }
            else
            {
                return null;
            }
        }

        public void Dispose()
        {
            List<Client> onlineUsers = ButterflyEnvironment.GetGame().GetClientManager().GetClientsById(this.Friends.Keys);

            foreach (Client gameClient in onlineUsers)
            {
                if (gameClient != null && gameClient.GetUser() != null && gameClient.GetUser().GetMessenger() != null && gameClient.GetUser().GetMessenger().FriendshipExists(this._userInstance.Id))
                {
                    gameClient.GetUser().GetMessenger().UpdateFriend(this._userInstance.Id, true);
                }
            }

            this.Requests.Clear();
            this.Relation.Clear();
            this.Friends.Clear();
        }

        public void RelationChanged(int Id, int Type)
        {
            this.Friends[Id].UpdateRelation(Type);
        }

        public void OnStatusChanged()
        {
            if (this.Friends == null)
            {
                return;
            }

            List<Client> onlineUsers = ButterflyEnvironment.GetGame().GetClientManager().GetClientsById(this.Friends.Keys);

            if (onlineUsers == null)
            {
                return;
            }

            foreach (Client client in onlineUsers)
            {
                if (client != null && client.GetUser() != null && client.GetUser().GetMessenger() != null)
                {
                    if (client.GetUser().GetMessenger().FriendshipExists(this._userInstance.Id))
                    {
                        client.GetUser().GetMessenger().UpdateFriend(this._userInstance.Id, true);
                        this.UpdateFriend(client.GetUser().Id, false);
                    }
                }
            }
        }

        public void UpdateFriend(int userId, bool notification)
        {
            if (!this.Friends.ContainsKey(userId))
            {
                return;
            }

            MessengerBuddy friend = this.Friends[userId];

            friend.UpdateUser();

            if (!notification)
            {
                return;
            }

            Client client = this.GetClient();
            if (client == null)
            {
                return;
            }

            client.SendPacket(new FriendListUpdateComposer(friend));
        }

        public void HandleAllRequests()
        {
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                MessengerRequestDao.Delete(dbClient, this._userInstance.Id);
            }

            this.ClearRequests();
        }

        public void HandleRequest(int sender)
        {
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                MessengerRequestDao.Delete(dbClient, this._userInstance.Id, sender);
            }

            this.Requests.Remove(sender);
        }

        public void CreateFriendship(int friendID)
        {
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                MessengerFriendshipDao.Replace(dbClient, this._userInstance.Id, friendID);
                MessengerFriendshipDao.Replace(dbClient, friendID, this._userInstance.Id);
            }

            this.OnNewFriendship(friendID);
            Client clientByUserId = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(friendID);
            if (clientByUserId == null || clientByUserId.GetUser().GetMessenger() == null)
            {
                return;
            }

            clientByUserId.GetUser().GetMessenger().OnNewFriendship(this._userInstance.Id);
        }

        public void DestroyFriendship(int friendID)
        {
            if (!this.Friends.ContainsKey(friendID))
            {
                return;
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                MessengerFriendshipDao.Delete(dbClient, this._userInstance.Id, friendID);
            }

            this.OnDestroyFriendship(friendID);
            Client clientByUserId = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(friendID);
            if (clientByUserId == null || clientByUserId.GetUser().GetMessenger() == null)
            {
                return;
            }

            clientByUserId.GetUser().GetMessenger().OnDestroyFriendship(this._userInstance.Id);
        }

        public void OnNewFriendship(int friendID)
        {
            Client clientByUserId = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(friendID);
            MessengerBuddy friend;
            if (clientByUserId == null || clientByUserId.GetUser() == null)
            {
                string username;
                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    username = UserDao.GetNameById(dbClient, friendID);
                }

                if (username == "")
                {
                    return;
                }

                friend = new MessengerBuddy(friendID, username, "", 0);
            }
            else
            {
                User user = clientByUserId.GetUser();
                friend = new MessengerBuddy(friendID, user.Username, user.Look, 0);
                friend.UpdateUser();
            }
            if (!this.Friends.ContainsKey(friendID))
            {
                this.Friends.Add(friendID, friend);
            }

            this.GetClient().SendPacket(new FriendListUpdateComposer(friend));
        }

        public bool RequestExists(int requestID)
        {
            if (this.Requests.ContainsKey(requestID))
            {
                return true;
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                return MessengerFriendshipDao.haveFriend(dbClient, this._userInstance.Id, requestID);
            }
        }

        public bool FriendshipExists(int friendID)
        {
            return this.Friends.ContainsKey(friendID);
        }

        public void OnDestroyFriendship(int friendId)
        {
            this.Friends.Remove(friendId);
            this.Relation.Remove(friendId);

            this.GetClient().SendPacket(new FriendListUpdateComposer(null, friendId));
        }

        public bool RequestBuddy(string UserQuery)
        {
            Client clientByUsername = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(UserQuery);
            int sender;
            bool flag;
            if (clientByUsername == null)
            {
                DataRow dataRow = null;
                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dataRow = UserDao.GetOneIdAndBlockNewFriend(dbClient, UserQuery.ToLower());
                }

                if (dataRow == null)
                {
                    return false;
                }

                sender = Convert.ToInt32(dataRow["id"]);
                flag = ButterflyEnvironment.EnumToBool(dataRow["block_newfriends"].ToString());
            }
            else
            {
                if (clientByUsername.GetUser() != null)
                {
                    sender = clientByUsername.GetUser().Id;
                    flag = clientByUsername.GetUser().HasFriendRequestsDisabled;
                }
                else
                {
                    return false;
                }
            }

            if (flag)
            {
                this.GetClient().SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.textamigo.error", this.GetClient().Langue));
                return false;
            }
            else
            {
                if (this.RequestExists(sender))
                {
                    return false;
                }

                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    MessengerRequestDao.Replace(dbClient, this._userInstance.Id, sender);
                }

                Client clientByUserId = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(sender);
                if (clientByUserId == null || clientByUserId.GetUser() == null)
                {
                    return false;
                }

                MessengerRequest request = new MessengerRequest(sender, this._userInstance.Id, ButterflyEnvironment.GetGame().GetClientManager().GetNameById(this._userInstance.Id));
                clientByUserId.GetUser().GetMessenger().OnNewRequest(this._userInstance.Id);

                clientByUserId.SendPacket(new NewBuddyRequestComposer(request));

                if (!this.Requests.ContainsKey(sender))
                {
                    this.Requests.Add(sender, request);
                }

                return true;
            }
        }

        public void OnNewRequest(int friendID)
        {
            if (this.Requests.ContainsKey(friendID))
            {
                return;
            }

            this.Requests.Add(friendID, new MessengerRequest(this._userInstance.Id, friendID, ButterflyEnvironment.GetGame().GetClientManager().GetNameById(friendID)));
        }

        public void SendInstantMessage(int ToId, string Message)
        {
            if (!this.FriendshipExists(ToId))
            {
                this.GetClient().SendPacket(new InstantMessageErrorComposer(6, ToId));
                return;
            }

            Client Client = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(ToId);
            if (Client == null || Client.GetUser() == null || Client.GetUser().GetMessenger() == null)
            {
                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    MessengerOfflineMessageDao.Insert(dbClient, ToId, this.GetClient().GetUser().Id, Message);
                }

                return;
            }

            if (Client.GetUser().IgnoreRoomInvites)
            {
                this.GetClient().SendPacket(new InstantMessageErrorComposer(7, ToId));
                return;
            }

            if (Client.GetUser().FloodCount > 0)
            {
                this.GetClient().SendPacket(new InstantMessageErrorComposer(4, ToId));
            }

            Client.SendPacket(new NewConsoleComposer(this._userInstance.Id, Message));
        }

        public void ProcessOfflineMessages()
        {
            DataTable GetMessages = null;
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                GetMessages = MessengerOfflineMessageDao.GetAll(dbClient, this._userInstance.Id);

                if (GetMessages != null)
                {
                    Client Client = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(this._userInstance.Id);
                    if (Client == null)
                    {
                        return;
                    }

                    ServerPacketList packetList = new ServerPacketList();

                    foreach (DataRow Row in GetMessages.Rows)
                    {
                        packetList.Add(new NewConsoleComposer(Convert.ToInt32(Row["from_id"]), Convert.ToString(Row["message"]), (ButterflyEnvironment.GetUnixTimestamp() - Convert.ToInt32(Row["timestamp"]))));
                    }

                    Client.SendPacket(packetList);

                    MessengerOfflineMessageDao.Delete(dbClient, this._userInstance.Id);
                }
            }
        }

        public List<Relationship> GetRelationships()
        {
            return Friends.Values.Select(c => new Relationship(c.UserId, c.Relation)).ToList();
        }

        private Client GetClient()
        {
            return ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(this._userInstance.Id);
        }
    }
}
