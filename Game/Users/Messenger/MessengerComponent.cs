using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.Messenger;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using System;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.Game.Users.Messenger
{
    public class MessengerComponent
    {
        private readonly int _userId;

        public Dictionary<int, MessengerRequest> Requests { get; private set; }
        public Dictionary<int, MessengerBuddy> Friends { get; private set; }
        public Dictionary<int, Relationship> Relation { get; private set; }
        public bool AppearOffline { get; set; }

        public int Count => this.Friends.Count;

        public MessengerComponent(int UserId)
        {
            this._userId = UserId;

            this.Requests = new Dictionary<int, MessengerRequest>();
            this.Friends = new Dictionary<int, MessengerBuddy>();
            this.Relation = new Dictionary<int, Relationship>();
        }

        public void Init(Dictionary<int, MessengerBuddy> friends, Dictionary<int, MessengerRequest> requests, Dictionary<int, Relationship> relationships, bool appearOffline)
        {
            this.Requests = requests;
            this.Friends = friends;
            this.Relation = relationships;
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

        public void Destroy()
        {
            List<Client> onlineUsers = ButterflyEnvironment.GetGame().GetClientManager().GetClientsById(this.Friends.Keys);

            foreach (Client gameClient in onlineUsers)
            {
                if (gameClient != null && gameClient.GetHabbo() != null && gameClient.GetHabbo().GetMessenger() != null && gameClient.GetHabbo().GetMessenger().FriendshipExists(this._userId))
                {
                    gameClient.GetHabbo().GetMessenger().UpdateFriend(this._userId, true);
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
                if (client != null && client.GetHabbo() != null && client.GetHabbo().GetMessenger() != null)
                {
                    if (client.GetHabbo().GetMessenger().FriendshipExists(this._userId))
                    {
                        client.GetHabbo().GetMessenger().UpdateFriend(this._userId, true);
                        this.UpdateFriend(client.GetHabbo().Id, false);
                    }
                }
            }
        }

        public void UpdateFriend(int userid, bool notification)
        {
            if (!this.Friends.ContainsKey(userid))
            {
                return;
            }

            this.Friends[userid].UpdateUser();

            if (!notification)
            {
                return;
            }

            Client client1 = this.GetClient();
            if (client1 == null)
            {
                return;
            }

            client1.SendPacket(this.SerializeUpdate(this.Friends[userid]));
        }

        public void HandleAllRequests()
        {
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                MessengerRequestDao.Delete(dbClient, this._userId);
            }

            this.ClearRequests();
        }

        public void HandleRequest(int sender)
        {
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                MessengerRequestDao.Delete(dbClient, this._userId, sender);
            }

            this.Requests.Remove(sender);
        }

        public void CreateFriendship(int friendID)
        {
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                MessengerFriendshipDao.Replace(dbClient, this._userId, friendID);
                MessengerFriendshipDao.Replace(dbClient, friendID, this._userId);
            }

            this.OnNewFriendship(friendID);
            Client clientByUserId = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(friendID);
            if (clientByUserId == null || clientByUserId.GetHabbo().GetMessenger() == null)
            {
                return;
            }

            clientByUserId.GetHabbo().GetMessenger().OnNewFriendship(this._userId);
        }

        public void DestroyFriendship(int friendID)
        {
            if (!this.Friends.ContainsKey(friendID))
            {
                return;
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                MessengerFriendshipDao.Delete(dbClient, this._userId, friendID);
            }

            this.OnDestroyFriendship(friendID);
            Client clientByUserId = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(friendID);
            if (clientByUserId == null || clientByUserId.GetHabbo().GetMessenger() == null)
            {
                return;
            }

            clientByUserId.GetHabbo().GetMessenger().OnDestroyFriendship(this._userId);
        }

        public void OnNewFriendship(int friendID)
        {
            Client clientByUserId = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(friendID);
            MessengerBuddy friend;
            if (clientByUserId == null || clientByUserId.GetHabbo() == null)
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
                User habbo = clientByUserId.GetHabbo();
                friend = new MessengerBuddy(friendID, habbo.Username, habbo.Look, 0);
                friend.UpdateUser();
            }
            if (!this.Friends.ContainsKey(friendID))
            {
                this.Friends.Add(friendID, friend);
            }

            this.GetClient().SendPacket(this.SerializeUpdate(friend));
        }

        public bool RequestExists(int requestID)
        {
            if (this.Requests.ContainsKey(requestID))
            {
                return true;
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                return MessengerFriendshipDao.haveFriend(dbClient, this._userId, requestID);
            }
        }

        public bool FriendshipExists(int friendID)
        {
            return this.Friends.ContainsKey(friendID);
        }

        public void OnDestroyFriendship(int Friend)
        {
            this.Friends.Remove(Friend);
            this.Relation.Remove(Friend);

            ServerPacket Response = new ServerPacket(ServerPacketHeader.MESSENGER_UPDATE);
            Response.WriteInteger(0);
            Response.WriteInteger(1);
            Response.WriteInteger(-1);
            Response.WriteInteger(Friend);
            this.GetClient().SendPacket(Response);
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
                if (clientByUsername.GetHabbo() != null)
                {
                    sender = clientByUsername.GetHabbo().Id;
                    flag = clientByUsername.GetHabbo().HasFriendRequestsDisabled;
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
                    MessengerRequestDao.Replace(dbClient, this._userId, sender);
                }

                Client clientByUserId = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(sender);
                if (clientByUserId == null || clientByUserId.GetHabbo() == null)
                {
                    return false;
                }

                MessengerRequest messengerRequest = new MessengerRequest(sender, this._userId, ButterflyEnvironment.GetGame().GetClientManager().GetNameById(this._userId));
                clientByUserId.GetHabbo().GetMessenger().OnNewRequest(this._userId);

                ServerPacket serverMessage = new ServerPacket(ServerPacketHeader.MESSENGER_REQUEST);
                messengerRequest.Serialize(serverMessage);
                clientByUserId.SendPacket(serverMessage);

                if (!this.Requests.ContainsKey(sender))
                {
                    this.Requests.Add(sender, messengerRequest);
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

            this.Requests.Add(friendID, new MessengerRequest(this._userId, friendID, ButterflyEnvironment.GetGame().GetClientManager().GetNameById(friendID)));
        }

        public void SendInstantMessage(int ToId, string Message)
        {
            if (!this.FriendshipExists(ToId))
            {
                this.GetClient().SendPacket(new InstantMessageErrorComposer(6, ToId));
                return;
            }

            Client Client = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(ToId);
            if (Client == null || Client.GetHabbo() == null || Client.GetHabbo().GetMessenger() == null)
            {
                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    MessengerOfflineMessageDao.Insert(dbClient, ToId, this.GetClient().GetHabbo().Id, Message);
                }

                return;
            }

            Client.SendPacket(new NewConsoleComposer(this._userId, Message));
        }

        public ServerPacket SerializeCategories()
        {
            return new MessengerInitComposer();
        }

        public ServerPacket SerializeFriends()
        {
            ServerPacket reply = new ServerPacket(ServerPacketHeader.MESSENGER_FRIENDS);
            reply.WriteInteger(1);
            reply.WriteInteger(0);
            reply.WriteInteger(this.Friends.Count);
            foreach (MessengerBuddy messengerBuddy in this.Friends.Values)
            {
                messengerBuddy.Serialize(reply);
            }

            return reply;
        }

        public ServerPacket SerializeUpdate(MessengerBuddy friend)
        {
            ServerPacket reply = new ServerPacket(ServerPacketHeader.MESSENGER_UPDATE);
            reply.WriteInteger(0);
            reply.WriteInteger(1);
            reply.WriteInteger(0);
            friend.Serialize(reply);
            reply.WriteBoolean(false);
            return reply;
        }

        public ServerPacket SerializeRequests()
        {
            ServerPacket Request = new ServerPacket(ServerPacketHeader.MESSENGER_REQUESTS);
            Request.WriteInteger(this.Requests.Count);
            Request.WriteInteger(this.Requests.Count);

            foreach (MessengerRequest messengerRequest in this.Requests.Values)
            {
                messengerRequest.Serialize(Request);
            }
            return Request;
        }

        public ServerPacket PerformSearch(string query)
        {
            List<SearchResult> searchResult = SearchResultFactory.GetSearchResult(query);
            List<SearchResult> list1 = new List<SearchResult>();
            List<SearchResult> list2 = new List<SearchResult>();

            foreach (SearchResult searchResult2 in searchResult)
            {
                if (searchResult2.UserId != this.GetClient().GetHabbo().Id)
                {
                    if (this.FriendshipExists(searchResult2.UserId))
                    {
                        list1.Add(searchResult2);
                    }
                    else
                    {
                        list2.Add(searchResult2);
                    }
                }
            }

            ServerPacket reply = new ServerPacket(ServerPacketHeader.MESSENGER_SEARCH);
            reply.WriteInteger(list1.Count);
            foreach (SearchResult searchResult2 in list1)
            {
                searchResult2.Searialize(reply);
            }

            reply.WriteInteger(list2.Count);
            foreach (SearchResult searchResult2 in list2)
            {
                searchResult2.Searialize(reply);
            }

            return reply;
        }

        public void ProcessOfflineMessages()
        {
            DataTable GetMessages = null;
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                GetMessages = MessengerOfflineMessageDao.GetAll(dbClient, this._userId);

                if (GetMessages != null)
                {
                    Client Client = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(this._userId);
                    if (Client == null)
                    {
                        return;
                    }

                    foreach (DataRow Row in GetMessages.Rows)
                    {
                        Client.SendPacket(new NewConsoleComposer(Convert.ToInt32(Row["from_id"]), Convert.ToString(Row["message"]), (ButterflyEnvironment.GetUnixTimestamp() - Convert.ToInt32(Row["timestamp"]))));
                    }

                    MessengerOfflineMessageDao.Delete(dbClient, this._userId);
                }
            }
        }

        private Client GetClient()
        {
            return ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(this._userId);
        }
    }
}
