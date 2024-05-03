namespace WibboEmulator.Games.Users.Messenger;
using System.Data;
using WibboEmulator.Communication.Packets.Outgoing.Messenger;
using WibboEmulator.Core.Language;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Messenger;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Utilities;

public class MessengerComponent(User user) : IDisposable
{

    public Dictionary<int, MessengerRequest> Requests { get; private set; } = [];
    public Dictionary<int, MessengerBuddy> Friends { get; private set; } = [];
    public Dictionary<int, Relationship> Relation { get; private set; } = [];
    public bool AppearOffline { get; set; }

    public int Count => this._profilFriendCount > -1 ? this._profilFriendCount : this.Friends.Count;

    private int _profilFriendCount;

    public void Initialize(IDbConnection dbClient, bool appearOffline, bool onlyProfil = false)
    {
        var frienShips = onlyProfil ? MessengerFriendshipDao.GetAllFriendRelations(dbClient, user.Id) : MessengerFriendshipDao.GetAllFriendShips(dbClient, user.Id);

        var requests = onlyProfil ? [] : MessengerRequestDao.GetAllFriendRequests(dbClient, user.Id);

        foreach (var friend in frienShips)
        {
            if (friend.Id != user.Id)
            {
                if (!this.Friends.ContainsKey(friend.Id))
                {
                    this.Friends.Add(friend.Id, new MessengerBuddy(friend.Id, friend.UserName, "", friend.Relation));
                    if (friend.Relation != 0)
                    {
                        this.Relation.Add(friend.Id, new Relationship(friend.Id, friend.Relation));
                    }
                }
            }
        }

        foreach (var request in requests)
        {
            if (request.FromId != user.Id)
            {
                if (!this.Requests.ContainsKey(request.FromId))
                {
                    this.Requests.Add(request.FromId, new MessengerRequest(user.Id, request.FromId, request.UserName));
                }
            }
            else if (!this.Requests.ContainsKey(request.ToId))
            {
                this.Requests.Add(request.ToId, new MessengerRequest(user.Id, request.ToId, request.UserName));
            }
        }

        this._profilFriendCount = onlyProfil ? MessengerFriendshipDao.GetCount(dbClient, user.Id) : -1;
        this.AppearOffline = appearOffline;
    }

    public void ClearRequests() => this.Requests.Clear();

    public MessengerRequest GetRequest(int senderID)
    {
        if (this.Requests.TryGetValue(senderID, out var value))
        {
            return value;
        }
        else
        {
            return null;
        }
    }

    public void Dispose()
    {
        var onlineUsers = GameClientManager.GetClientsById(this.Friends.Keys);

        foreach (var gameClient in onlineUsers)
        {
            if (gameClient != null && gameClient.User != null && gameClient.User.Messenger != null && gameClient.User.Messenger.FriendshipExists(user.Id))
            {
                gameClient.User.Messenger.UpdateFriend(user.Id, true);
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

        var onlineUsers = GameClientManager.GetClientsById(this.Friends.Keys);

        if (onlineUsers == null)
        {
            return;
        }

        foreach (var client in onlineUsers)
        {
            if (client != null && client.User != null && client.User.Messenger != null)
            {
                if (client.User.Messenger.FriendshipExists(user.Id))
                {
                    client.User.Messenger.UpdateFriend(user.Id, true);
                    this.UpdateFriend(client.User.Id, false);
                }
            }
        }
    }

    public void UpdateFriend(int userId, bool notification)
    {
        if (!this.Friends.TryGetValue(userId, out var friend))
        {
            return;
        }

        friend.UpdateUser();

        if (!notification)
        {
            return;
        }

        var client = this.Client;
        if (client == null)
        {
            return;
        }

        client.SendPacket(new FriendListUpdateComposer(friend));
    }

    public void HandleAllRequests()
    {
        using (var dbClient = DatabaseManager.Connection)
        {
            MessengerRequestDao.Delete(dbClient, user.Id);
        }

        this.ClearRequests();
    }

    public void HandleRequest(int sender)
    {
        using (var dbClient = DatabaseManager.Connection)
        {
            MessengerRequestDao.Delete(dbClient, user.Id, sender);
        }

        _ = this.Requests.Remove(sender);
    }

    public void CreateFriendship(int friendID)
    {
        using (var dbClient = DatabaseManager.Connection)
        {
            MessengerFriendshipDao.Replace(dbClient, user.Id, friendID);
            MessengerFriendshipDao.Replace(dbClient, friendID, user.Id);
        }

        this.OnNewFriendship(friendID);
        var clientByUserId = GameClientManager.GetClientByUserID(friendID);
        if (clientByUserId == null || clientByUserId.User.Messenger == null)
        {
            return;
        }

        clientByUserId.User.Messenger.OnNewFriendship(user.Id);
    }

    public void DestroyFriendship(int friendID)
    {
        if (!this.Friends.ContainsKey(friendID))
        {
            return;
        }

        using (var dbClient = DatabaseManager.Connection)
        {
            MessengerFriendshipDao.Delete(dbClient, user.Id, friendID);
        }

        this.OnDestroyFriendship(friendID);
        var clientByUserId = GameClientManager.GetClientByUserID(friendID);
        if (clientByUserId == null || clientByUserId.User.Messenger == null)
        {
            return;
        }

        clientByUserId.User.Messenger.OnDestroyFriendship(user.Id);
    }

    public void OnNewFriendship(int friendID)
    {
        var clientByUserId = GameClientManager.GetClientByUserID(friendID);
        MessengerBuddy friend;
        if (clientByUserId == null || clientByUserId.User == null)
        {
            var username = WibboEnvironment.GetNameById(friendID);

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

        _ = this.Friends.TryAdd(friendID, friend);
        this.Client.SendPacket(new FriendListUpdateComposer(friend));
    }

    public bool RequestExists(int requestID)
    {
        if (this.Requests.ContainsKey(requestID))
        {
            return true;
        }

        using var dbClient = DatabaseManager.Connection;
        return MessengerFriendshipDao.HaveFriend(dbClient, user.Id, requestID);
    }

    public bool FriendshipExists(int friendID) => this.Friends.ContainsKey(friendID);

    public void OnDestroyFriendship(int friendId)
    {
        _ = this.Friends.Remove(friendId);
        _ = this.Relation.Remove(friendId);

        this.
        Client.SendPacket(new FriendListUpdateComposer(null, friendId));
    }

    public bool RequestBuddy(string userQuery)
    {
        var clientByUsername = GameClientManager.GetClientByUsername(userQuery);
        int sender;
        bool flag;
        if (clientByUsername == null)
        {
            using var dbClient = DatabaseManager.Connection;
            var userSettings = UserDao.GetOneIdAndBlockNewFriend(dbClient, userQuery.ToLower());

            if (userSettings == null)
            {
                return false;
            }

            sender = userSettings.Id;
            flag = userSettings.BlockNewFriends;
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
            this.Client.SendNotification(LanguageManager.TryGetValue("notif.textamigo.error", this.Client.Language));
            return false;
        }
        else
        {
            if (this.RequestExists(sender))
            {
                return false;
            }

            using (var dbClient = DatabaseManager.Connection)
            {
                MessengerRequestDao.Replace(dbClient, user.Id, sender);
            }

            var clientByUserId = GameClientManager.GetClientByUserID(sender);
            if (clientByUserId == null || clientByUserId.User == null)
            {
                return false;
            }

            var request = new MessengerRequest(sender, user.Id, WibboEnvironment.GetNameById(user.Id));
            clientByUserId.User.Messenger.OnNewRequest(user.Id);

            clientByUserId.SendPacket(new NewBuddyRequestComposer(request));

            _ = this.Requests.TryAdd(sender, request);
            return true;
        }
    }

    public void OnNewRequest(int friendID)
    {
        if (this.Requests.ContainsKey(friendID))
        {
            return;
        }

        this.Requests.Add(friendID, new MessengerRequest(user.Id, friendID, WibboEnvironment.GetNameById(friendID)));
    }

    public void SendInstantMessage(int toId, string message)
    {
        if (!this.FriendshipExists(toId))
        {
            this.Client.SendPacket(new InstantMessageErrorComposer(6, toId));
            return;
        }

        var client = GameClientManager.GetClientByUserID(toId);
        if (client == null || client.User == null || client.User.Messenger == null)
        {
            using var dbClient = DatabaseManager.Connection;
            MessengerOfflineMessageDao.Insert(dbClient, toId, this.Client.User.Id, message);

            return;
        }

        client.SendPacket(new NewConsoleComposer(user.Id, message));
    }

    public void ProcessOfflineMessages()
    {
        using var dbClient = DatabaseManager.Connection;

        var messageList = MessengerOfflineMessageDao.GetAll(dbClient, user.Id);

        if (messageList.Count != 0)
        {
            var client = GameClientManager.GetClientByUserID(user.Id);
            if (client == null)
            {
                return;
            }

            var packetList = new ServerPacketList();

            foreach (var message in messageList)
            {
                packetList.Add(new NewConsoleComposer(message.FromId, message.Message, WibboEnvironment.GetUnixTimestamp() - message.Timestamp));
            }

            client.SendPacket(packetList);

            MessengerOfflineMessageDao.Delete(dbClient, user.Id);
        }
    }

    public List<Relationship> Relationships => [.. this.Relation.Values];

    private GameClient Client => GameClientManager.GetClientByUserID(user.Id);
}
