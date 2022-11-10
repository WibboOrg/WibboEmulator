namespace WibboEmulator.Games.GameClients;
using System.Collections.Concurrent;
using System.Text;
using WibboEmulator.Communication.Interfaces;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Notifications;
using WibboEmulator.Communication.WebSocket;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.Users.Messenger;

public class GameClientManager
{
    private readonly ConcurrentDictionary<string, GameClient> _clients;
    private readonly ConcurrentDictionary<string, string> _usernameRegister;
    private readonly ConcurrentDictionary<int, string> _userIDRegister;

    public int OnlineUsersFr { get; set; }
    public int OnlineUsersEn { get; set; }
    public int OnlineUsersBr { get; set; }

    private readonly List<int> _userStaff;

    public int Count => this._userIDRegister.Count;

    public GameClientManager()
    {
        this._clients = new ConcurrentDictionary<string, GameClient>();
        this._usernameRegister = new ConcurrentDictionary<string, string>();
        this._userIDRegister = new ConcurrentDictionary<int, string>();
        this._userStaff = new List<int>();
    }

    public List<GameClient> GetStaffUsers()
    {
        var users = new List<GameClient>();

        foreach (var userId in this._userStaff)
        {
            var client = this.GetClientByUserID(userId);
            if (client == null || client.User == null)
            {
                continue;
            }

            users.Add(client);
        }

        return users;
    }

    public GameClient GetClientById(string clientID)
    {
        _ = this.TryGetClient(clientID, out var client);

        return client;
    }

    public GameClient GetClientByUserID(int userID)
    {
        if (this._userIDRegister.ContainsKey(userID))
        {
            if (!this.TryGetClient(this._userIDRegister[userID], out var client))
            {
                return null;
            }

            return client;
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
            if (!this.TryGetClient(this._usernameRegister[username.ToLower()], out var client))
            {
                return null;
            }

            return client;
        }
        return null;
    }

    public bool UpdateClientUsername(string clientId, string oldUsername, string newUsername)
    {
        if (!this._usernameRegister.ContainsKey(oldUsername.ToLower()))
        {
            return false;
        }

        _ = this._usernameRegister.TryRemove(oldUsername.ToLower(), out _);
        _ = this._usernameRegister.TryAdd(newUsername.ToLower(), clientId);
        return true;
    }

    public bool TryGetClient(string clientId, out GameClient client) => this._clients.TryGetValue(clientId, out client);

    public string GetNameById(int id)
    {
        var clientByUserId = this.GetClientByUserID(id);

        if (clientByUserId != null)
        {
            return clientByUserId.User.Username;
        }

        var username = "";
        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            username = UserDao.GetNameById(dbClient, id);
        }

        return username;
    }

    public List<GameClient> GetClientsById(Dictionary<int, MessengerBuddy>.KeyCollection users)
    {
        var clientOnline = new List<GameClient>();
        foreach (var userID in users)
        {
            var client = this.GetClientByUserID(userID);
            if (client != null)
            {
                clientOnline.Add(client);
            }
        }

        return clientOnline;
    }

    public void SendMessageStaff(IServerPacket packet)
    {
        foreach (var userId in this._userStaff)
        {
            var client = this.GetClientByUserID(userId);
            if (client == null || client.User == null)
            {
                continue;
            }

            client.SendPacket(packet);
        }
    }

    public void SendMessage(IServerPacket packet)
    {
        foreach (var client in this._clients.Values.ToList())
        {
            if (client == null || client.User == null)
            {
                continue;
            }

            client.SendPacket(packet);
        }
    }

    public void CreateAndStartClient(string clientID, GameWebSocket connection)
    {
        var client = new GameClient(clientID, connection);
        if (!this._clients.TryAdd(clientID, client))
        {
            connection.Disconnect();
        }
    }

    public void DisposeConnection(string clientID)
    {
        if (!this.TryGetClient(clientID, out var client))
        {
            return;
        }

        client?.Dispose();

        _ = this._clients.TryRemove(clientID, out _);
    }

    public void LogClonesOut(int userID)
    {
        var clientByUserId = this.GetClientByUserID(userID);
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
            _ = this._usernameRegister.TryAdd(username.ToLower(), client.ConnectionID);
        }

        if (this._userIDRegister.ContainsKey(userID))
        {
            this._userIDRegister[userID] = client.ConnectionID;
        }
        else
        {
            _ = this._userIDRegister.TryAdd(userID, client.ConnectionID);
        }
    }

    public void UnregisterClient(int userId, string username)
    {
        _ = this._userIDRegister.TryRemove(userId, out var _);
        _ = this._usernameRegister.TryRemove(username.ToLower(), out _);
    }

    public void AddUserStaff(int userId)
    {
        if (!this._userStaff.Contains(userId))
        {
            this._userStaff.Add(userId);
        }
    }

    public void RemoveUserStaff(int userId)
    {
        if (this._userStaff.Contains(userId))
        {
            _ = this._userStaff.Remove(userId);
        }
    }

    public void CloseAll()
    {
        var stringBuilder = new StringBuilder();

        foreach (var client in this.GetClients.ToList())
        {
            if (client == null)
            {
                continue;
            }

            if (client.User != null)
            {
                try
                {
                    var timeOnline = DateTime.Now - client.User.OnlineTime;
                    var timeOnlineSec = (int)timeOnline.TotalSeconds;

                    _ = stringBuilder.Append(UserDao.BuildUpdateQuery(client.User.Id, client.User.Duckets, client.User.Credits));
                    _ = stringBuilder.Append(UserStatsDao.BuildUpdateQuery(client.User.Id, client.User.FavouriteGroupId, timeOnlineSec, client.User.CurrentQuestId, client.User.Respect, client.User.DailyRespectPoints, client.User.DailyPetRespectPoints));
                }
                catch
                {
                }
            }
        }

        if (stringBuilder.Length > 0)
        {
            using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
            dbClient.RunQuery(stringBuilder.ToString());
        }

        Console.WriteLine("Done saving users inventory!");
        Console.WriteLine("Closing server connections...");

        foreach (var client in this.GetClients.ToList())
        {

            if (client == null || client.Connection == null)
            {
                continue;
            }

            try
            {
                client.Connection.Disconnect();
            }
            catch
            {
            }
        }
        this._clients.Clear();
        Console.WriteLine("Connections closed!");
    }

    public void BanUser(GameClient client, string moderator, double lengthSeconds, string reason, bool ipBan, bool machineBan)
    {
        if (string.IsNullOrEmpty(reason))
        {
            reason = "Non respect des règles de conditions générales d'utilisations ainsi que la Wibbo Attitude";
        }

        var variable = client.User.Username.ToLower();
        var str = "user";
        var expire = WibboEnvironment.GetUnixTimestamp() + lengthSeconds;
        if (ipBan)
        {
            //Variable = Client.GetConnection().getIp();
            variable = client.User.IP;
            str = "ip";
        }

        if (machineBan)
        {
            variable = client.MachineId;
            str = "machine";
        }

        if (variable == "")
        {
            return;
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            if (str == "user")
            {
                UserDao.UpdateIsBanned(dbClient, client.User.Id);
            }

            BanDao.InsertBan(dbClient, expire, str, variable, reason, moderator);
        }

        if (machineBan)
        {
            this.BanUser(client, moderator, lengthSeconds, reason, true, false);
        }
        else if (ipBan)
        {
            this.BanUser(client, moderator, lengthSeconds, reason, false, false);
        }
        else
        {
            client.Disconnect();
        }
    }

    public void SendSuperNotif(string title, string notice, string picture, string link, string linkTitle) => this.SendMessage(new RoomNotificationComposer(title, notice, picture, linkTitle, link));

    public ICollection<GameClient> GetClients => this._clients.Values;
}
