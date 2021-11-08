using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.Messenger;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.HabboHotel.Users.Messenger
{
    public class HabboMessenger
    {
        private readonly int UserId;
        public Dictionary<int, MessengerRequest> requests;
        private Dictionary<int, MessengerBuddy> friends;
        public Dictionary<int, Relationship> relation;
        public bool AppearOffline;

        public int Count => this.friends.Count;

        public HabboMessenger(int UserId)
        {
            this.requests = new Dictionary<int, MessengerRequest>();
            this.friends = new Dictionary<int, MessengerBuddy>();
            this.relation = new Dictionary<int, Relationship>();
            this.UserId = UserId;
        }

        public void Init(Dictionary<int, MessengerBuddy> friends, Dictionary<int, MessengerRequest> requests, Dictionary<int, Relationship> Relationships)
        {
            this.requests = new Dictionary<int, MessengerRequest>(requests);
            this.friends = new Dictionary<int, MessengerBuddy>(friends);
            this.relation = new Dictionary<int, Relationship>(Relationships);
        }

        public void ClearRequests()
        {
            this.requests.Clear();
        }

        public MessengerRequest GetRequest(int senderID)
        {
            if (this.requests.ContainsKey(senderID))
            {
                return this.requests[senderID];
            }
            else
            {
                return null;
            }
        }

        public void Destroy()
        {
            List<GameClient> onlineUsers = ButterflyEnvironment.GetGame().GetClientManager().GetClientsById(this.friends.Keys);

            foreach (GameClient gameClient in onlineUsers)
            {
                if (gameClient != null && gameClient.GetHabbo() != null && gameClient.GetHabbo().GetMessenger() != null && gameClient.GetHabbo().GetMessenger().FriendshipExists(this.UserId))
                {
                    gameClient.GetHabbo().GetMessenger().UpdateFriend(this.UserId, true);
                }
            }

            this.requests.Clear();
            this.relation.Clear();
            this.friends.Clear();
        }

        public void RelationChanged(int Id, int Type)
        {
            this.friends[Id].UpdateRelation(Type);
        }

        public void OnStatusChanged()
        {
            if (this.friends == null)
            {
                return;
            }

            List<GameClient> onlineUsers = ButterflyEnvironment.GetGame().GetClientManager().GetClientsById(this.friends.Keys);

            if (onlineUsers == null)
            {
                return;
            }

            foreach (GameClient client in onlineUsers)
            {
                if (client != null && client.GetHabbo() != null && client.GetHabbo().GetMessenger() != null)
                {
                    if (client.GetHabbo().GetMessenger().FriendshipExists(this.UserId))
                    {
                        client.GetHabbo().GetMessenger().UpdateFriend(this.UserId, true);
                        this.UpdateFriend(client.GetHabbo().Id, false);
                    }
                }
            }
        }

        public void UpdateFriend(int userid, bool notification)
        {
            if (!this.friends.ContainsKey(userid))
            {
                return;
            }

            this.friends[userid].UpdateUser();

            if (!notification)
            {
                return;
            }

            GameClient client1 = this.GetClient();
            if (client1 == null)
            {
                return;
            }

            client1.SendPacket(this.SerializeUpdate(this.friends[userid]));
        }

        public void HandleAllRequests()
        {
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                MessengerRequestDao.Delete(dbClient, this.UserId);
            }

            this.ClearRequests();
        }

        public void HandleRequest(int sender)
        {
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                MessengerRequestDao.Delete(dbClient, this.UserId, sender);
            }

            this.requests.Remove(sender);
        }

        public void CreateFriendship(int friendID)
        {
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                MessengerFriendshipDao.Replace(dbClient, this.UserId, friendID);
                MessengerFriendshipDao.Replace(dbClient, friendID, this.UserId);
            }

            this.OnNewFriendship(friendID);
            GameClient clientByUserId = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(friendID);
            if (clientByUserId == null || clientByUserId.GetHabbo().GetMessenger() == null)
            {
                return;
            }

            clientByUserId.GetHabbo().GetMessenger().OnNewFriendship(this.UserId);
        }

        public void DestroyFriendship(int friendID)
        {
            if (!this.friends.ContainsKey(friendID))
            {
                return;
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                MessengerFriendshipDao.Delete(dbClient, this.UserId, friendID);
            }

            this.OnDestroyFriendship(friendID);
            GameClient clientByUserId = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(friendID);
            if (clientByUserId == null || clientByUserId.GetHabbo().GetMessenger() == null)
            {
                return;
            }

            clientByUserId.GetHabbo().GetMessenger().OnDestroyFriendship(this.UserId);
        }

        public void OnNewFriendship(int friendID)
        {
            GameClient clientByUserId = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(friendID);
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
                Habbo habbo = clientByUserId.GetHabbo();
                friend = new MessengerBuddy(friendID, habbo.Username, habbo.Look, 0);
                friend.UpdateUser();
            }
            if (!this.friends.ContainsKey(friendID))
            {
                this.friends.Add(friendID, friend);
            }

            this.GetClient().SendPacket(this.SerializeUpdate(friend));
        }

        public bool RequestExists(int requestID)
        {
            if (this.requests.ContainsKey(requestID))
            {
                return true;
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                return MessengerFriendshipDao.haveFriend(dbClient, this.UserId, requestID);
            }
        }

        public bool FriendshipExists(int friendID)
        {
            return this.friends.ContainsKey(friendID);
        }

        public void OnDestroyFriendship(int Friend)
        {
            this.friends.Remove(Friend);
            this.relation.Remove(Friend);

            ServerPacket Response = new ServerPacket(ServerPacketHeader.MESSENGER_UPDATE);
            Response.WriteInteger(0);
            Response.WriteInteger(1);
            Response.WriteInteger(-1);
            Response.WriteInteger(Friend);
            this.GetClient().SendPacket(Response);
        }

        public bool RequestBuddy(string UserQuery)
        {
            GameClient clientByUsername = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(UserQuery);
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
                    MessengerRequestDao.Replace(dbClient, this.UserId, sender);
                }

                GameClient clientByUserId = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(sender);
                if (clientByUserId == null || clientByUserId.GetHabbo() == null)
                {
                    return false;
                }

                MessengerRequest messengerRequest = new MessengerRequest(sender, this.UserId, ButterflyEnvironment.GetGame().GetClientManager().GetNameById(this.UserId));
                clientByUserId.GetHabbo().GetMessenger().OnNewRequest(this.UserId);

                ServerPacket serverMessage = new ServerPacket(ServerPacketHeader.MESSENGER_REQUEST);
                messengerRequest.Serialize(serverMessage);
                clientByUserId.SendPacket(serverMessage);

                if (!this.requests.ContainsKey(sender))
                {
                    this.requests.Add(sender, messengerRequest);
                }

                return true;
            }
        }

        public void OnNewRequest(int friendID)
        {
            if (this.requests.ContainsKey(friendID))
            {
                return;
            }

            this.requests.Add(friendID, new MessengerRequest(this.UserId, friendID, ButterflyEnvironment.GetGame().GetClientManager().GetNameById(friendID)));
        }

        public void SendInstantMessage(int ToId, string Message)
        {
            if (!this.FriendshipExists(ToId))
            {
                this.GetClient().SendPacket(new InstantMessageErrorComposer(6, ToId));
                return;
            }

            GameClient Client = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(ToId);
            if (Client == null || Client.GetHabbo() == null || Client.GetHabbo().GetMessenger() == null)
            {
                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    MessengerOfflineMessageDao.Insert(dbClient, ToId, this.GetClient().GetHabbo().Id, Message);
                }

                return;
            }

            Client.SendPacket(new NewConsoleMessageComposer(this.UserId, Message));
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
            reply.WriteInteger(this.friends.Count);
            foreach (MessengerBuddy messengerBuddy in this.friends.Values)
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
            Request.WriteInteger(this.requests.Count);
            Request.WriteInteger(this.requests.Count);

            foreach (MessengerRequest messengerRequest in this.requests.Values)
            {
                messengerRequest.Serialize(Request);
            }
            return Request;
        }

        public ServerPacket PerformSearch(string query)
        {
            List<SearchResult> searchResult1 = SearchResultFactory.GetSearchResult(query);
            List<SearchResult> list1 = new List<SearchResult>();
            List<SearchResult> list2 = new List<SearchResult>();
            foreach (SearchResult searchResult2 in searchResult1)
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
                GetMessages = MessengerOfflineMessageDao.GetAll(dbClient, this.UserId);

                if (GetMessages != null)
                {
                    GameClient Client = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(this.UserId);
                    if (Client == null)
                    {
                        return;
                    }

                    foreach (DataRow Row in GetMessages.Rows)
                    {
                        Client.SendPacket(new NewConsoleMessageComposer(Convert.ToInt32(Row["from_id"]), Convert.ToString(Row["message"]), (ButterflyEnvironment.GetUnixTimestamp() - Convert.ToInt32(Row["timestamp"]))));
                    }

                    MessengerOfflineMessageDao.Delete(dbClient, this.UserId);
                }
            }
        }

        private GameClient GetClient()
        {
            return ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(this.UserId);
        }


        public Dictionary<int, MessengerBuddy> GetFriends()
        {
            return this.friends;
        }
    }
}
