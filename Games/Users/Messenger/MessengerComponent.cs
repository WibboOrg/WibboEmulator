namespace WibboEmulator.Games.Users.Messenger;
using System.Data;
using WibboEmulator.Communication.Packets.Outgoing.Messenger;
using WibboEmulator.Database.Daos.Messenger;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Utilities;

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
        var frienShips = UserDao.GetAllFriendShips(dbClient, this._userInstance.Id);

        var requests = UserDao.GetAllFriendRequests(dbClient, this._userInstance.Id);

        foreach (DataRow dataRow in frienShips.Rows)
        {
            var userId = Convert.ToInt32(dataRow["id"]);
            var pUsername = (string)dataRow["username"];
            var relation = Convert.ToInt32(dataRow["relation"]);
            if (userId != this._userInstance.Id)
            {
                if (!this.Friends.ContainsKey(userId))
                {
                    this.Friends.Add(userId, new MessengerBuddy(userId, pUsername, "", relation));
                    if (relation != 0)
                    {
                        this.Relation.Add(userId, new Relationship(userId, relation));
                    }
                }
            }
        }

        foreach (DataRow dataRow in requests.Rows)
        {
            var fromId = Convert.ToInt32(dataRow["from_id"]);
            var toId = Convert.ToInt32(dataRow["to_id"]);
            var username = (string)dataRow["username"];

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

    public void ClearRequests() => this.Requests.Clear();

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
        var onlineUsers = WibboEnvironment.GetGame().GetGameClientManager().GetClientsById(this.Friends.Keys);

        foreach (var gameClient in onlineUsers)
        {
            if (gameClient != null && gameClient.User != null && gameClient.User.Messenger != null && gameClient.User.Messenger.FriendshipExists(this._userInstance.Id))
            {
                gameClient.User.Messenger.UpdateFriend(this._userInstance.Id, true);
            }
        }

        this.Requests.Clear();
        this.Relation.Clear();
        this.Friends.Clear();

        GC.SuppressFinalize(this);
    }

    public void RelationChanged(int id, int type) => this.Friends[id].UpdateRelation(type);

    public void OnStatusChanged()
    {
        if (this.Friends == null)
        {
            return;
        }

        var onlineUsers = WibboEnvironment.GetGame().GetGameClientManager().GetClientsById(this.Friends.Keys);

        if (onlineUsers == null)
        {
            return;
        }

        foreach (var client in onlineUsers)
        {
            if (client != null && client.User != null && client.User.Messenger != null)
            {
                if (client.User.Messenger.FriendshipExists(this._userInstance.Id))
                {
                    client.User.Messenger.UpdateFriend(this._userInstance.Id, true);
                    this.UpdateFriend(client.User.Id, false);
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

        var friend = this.Friends[userId];

        friend.UpdateUser();

        if (!notification)
        {
            return;
        }

        var client = this.GetClient();
        if (client == null)
        {
            return;
        }

        client.SendPacket(new FriendListUpdateComposer(friend));
    }

    public void HandleAllRequests()
    {
        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            MessengerRequestDao.Delete(dbClient, this._userInstance.Id);
        }

        this.ClearRequests();
    }

    public void HandleRequest(int sender)
    {
        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            MessengerRequestDao.Delete(dbClient, this._userInstance.Id, sender);
        }

        _ = this.Requests.Remove(sender);
    }

    public void CreateFriendship(int friendID)
    {
        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            MessengerFriendshipDao.Replace(dbClient, this._userInstance.Id, friendID);
            MessengerFriendshipDao.Replace(dbClient, friendID, this._userInstance.Id);
        }

        this.OnNewFriendship(friendID);
        var clientByUserId = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(friendID);
        if (clientByUserId == null || clientByUserId.User.Messenger == null)
        {
            return;
        }

        clientByUserId.User.Messenger.OnNewFriendship(this._userInstance.Id);
    }

    public void DestroyFriendship(int friendID)
    {
        if (!this.Friends.ContainsKey(friendID))
        {
            return;
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            MessengerFriendshipDao.Delete(dbClient, this._userInstance.Id, friendID);
        }

        this.OnDestroyFriendship(friendID);
        var clientByUserId = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(friendID);
        if (clientByUserId == null || clientByUserId.User.Messenger == null)
        {
            return;
        }

        clientByUserId.User.Messenger.OnDestroyFriendship(this._userInstance.Id);
    }

    public void OnNewFriendship(int friendID)
    {
        var clientByUserId = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(friendID);
        MessengerBuddy friend;
        if (clientByUserId == null || clientByUserId.User == null)
        {
            string username;
            using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
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
            var user = clientByUserId.User;
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

        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        return MessengerFriendshipDao.HaveFriend(dbClient, this._userInstance.Id, requestID);
    }

    public bool FriendshipExists(int friendID) => this.Friends.ContainsKey(friendID);

    public void OnDestroyFriendship(int friendId)
    {
        _ = this.Friends.Remove(friendId);
        _ = this.Relation.Remove(friendId);

        this.GetClient().SendPacket(new FriendListUpdateComposer(null, friendId));
    }

    public bool RequestBuddy(string userQuery)
    {
        var clientByUsername = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(userQuery);
        int sender;
        bool flag;
        if (clientByUsername == null)
        {
            DataRow dataRow = null;
            using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dataRow = UserDao.GetOneIdAndBlockNewFriend(dbClient, userQuery.ToLower());
            }

            if (dataRow == null)
            {
                return false;
            }

            sender = Convert.ToInt32(dataRow["id"]);
            flag = WibboEnvironment.EnumToBool(dataRow["block_newfriends"].ToString());
        }
        else
        {
            if (clientByUsername.User != null)
            {
                sender = clientByUsername.User.Id;
                flag = clientByUsername.User.HasFriendRequestsDisabled;
            }
            else
            {
                return false;
            }
        }

        if (flag)
        {
            this.GetClient().SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.textamigo.error", this.GetClient().Langue));
            return false;
        }
        else
        {
            if (this.RequestExists(sender))
            {
                return false;
            }

            using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                MessengerRequestDao.Replace(dbClient, this._userInstance.Id, sender);
            }

            var clientByUserId = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(sender);
            if (clientByUserId == null || clientByUserId.User == null)
            {
                return false;
            }

            var request = new MessengerRequest(sender, this._userInstance.Id, WibboEnvironment.GetGame().GetGameClientManager().GetNameById(this._userInstance.Id));
            clientByUserId.User.Messenger.OnNewRequest(this._userInstance.Id);

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

        this.Requests.Add(friendID, new MessengerRequest(this._userInstance.Id, friendID, WibboEnvironment.GetGame().GetGameClientManager().GetNameById(friendID)));
    }

    public void SendInstantMessage(int toId, string message)
    {
        if (!this.FriendshipExists(toId))
        {
            this.GetClient().SendPacket(new InstantMessageErrorComposer(6, toId));
            return;
        }

        var client = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(toId);
        if (client == null || client.User == null || client.User.Messenger == null)
        {
            using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
            MessengerOfflineMessageDao.Insert(dbClient, toId, this.GetClient().User.Id, message);

            return;
        }

        client.SendPacket(new NewConsoleComposer(this._userInstance.Id, message));
    }

    public void ProcessOfflineMessages()
    {
        DataTable getMessages = null;
        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        getMessages = MessengerOfflineMessageDao.GetAll(dbClient, this._userInstance.Id);

        if (getMessages != null)
        {
            var client = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(this._userInstance.Id);
            if (client == null)
            {
                return;
            }

            var packetList = new ServerPacketList();

            foreach (DataRow row in getMessages.Rows)
            {
                packetList.Add(new NewConsoleComposer(Convert.ToInt32(row["from_id"]), Convert.ToString(row["message"]), WibboEnvironment.GetUnixTimestamp() - Convert.ToInt32(row["timestamp"])));
            }

            client.SendPacket(packetList);

            MessengerOfflineMessageDao.Delete(dbClient, this._userInstance.Id);
        }
    }

    public List<Relationship> GetRelationships() => this.Friends.Values.Select(c => new Relationship(c.UserId, c.Relation)).ToList();

    private GameClient GetClient() => WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(this._userInstance.Id);
}
