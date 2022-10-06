namespace WibboEmulator.Games.GameClients;
using System.Collections.Concurrent;
using System.Text;
using WibboEmulator.Communication.Interfaces;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Notifications;
using WibboEmulator.Communication.WebSocket;
using WibboEmulator.Core;
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
        var Users = new List<GameClient>();

        foreach (var UserId in this._userStaff)
        {
            var Client = this.GetClientByUserID(UserId);
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
        _ = this.TryGetClient(clientID, out var client);

        return client;
    }

    public GameClient GetClientByUserID(int userID)
    {
        if (this._userIDRegister.ContainsKey(userID))
        {
            if (!this.TryGetClient(this._userIDRegister[userID], out var Client))
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
            if (!this.TryGetClient(this._usernameRegister[username.ToLower()], out var Client))
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

        _ = this._usernameRegister.TryRemove(OldUsername.ToLower(), out ClientId);
        _ = this._usernameRegister.TryAdd(NewUsername.ToLower(), ClientId);
        return true;
    }

    public bool TryGetClient(string ClientId, out GameClient Client) => this._clients.TryGetValue(ClientId, out Client);

    public string GetNameById(int Id)
    {
        var clientByUserId = this.GetClientByUserID(Id);

        if (clientByUserId != null)
        {
            return clientByUserId.GetUser().Username;
        }

        var username = "";
        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            username = UserDao.GetNameById(dbClient, Id);
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
        foreach (var UserId in this._userStaff)
        {
            var Client = this.GetClientByUserID(UserId);
            if (Client == null || Client.GetUser() == null)
            {
                continue;
            }

            Client.SendPacket(packet);
        }
    }

    public void SendMessage(IServerPacket packet)
    {
        foreach (var Client in this._clients.Values.ToList())
        {
            if (Client == null || Client.GetUser() == null)
            {
                continue;
            }

            Client.SendPacket(packet);
        }
    }

    public void CreateAndStartClient(string clientID, GameWebSocket connection)
    {
        var Client = new GameClient(clientID, connection);
        if (!this._clients.TryAdd(clientID, Client))
        {
            connection.Disconnect();
        }
    }

    public void DisposeConnection(string clientID)
    {
        if (!this.TryGetClient(clientID, out var Client))
        {
            return;
        }

        if (Client != null)
        {
            Client.Dispose();
        }

        _ = this._clients.TryRemove(clientID, out Client);
    }

    public void LogClonesOut(int UserID)
    {
        var clientByUserId = this.GetClientByUserID(UserID);
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

    public void UnregisterClient(int userid, string username)
    {
        _ = this._userIDRegister.TryRemove(userid, out var Client);
        _ = this._usernameRegister.TryRemove(username.ToLower(), out Client);
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
            _ = this._userStaff.Remove(UserId);
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

            if (client.GetUser() != null)
            {
                try
                {
                    var TimeOnline = DateTime.Now - client.GetUser().OnlineTime;
                    var TimeOnlineSec = (int)TimeOnline.TotalSeconds;

                    _ = stringBuilder.Append(UserDao.BuildUpdateQuery(client.GetUser().Id, client.GetUser().Duckets, client.GetUser().Credits));
                    _ = stringBuilder.Append(UserStatsDao.BuildUpdateQuery(client.GetUser().Id, client.GetUser().FavouriteGroupId, TimeOnlineSec, client.GetUser().CurrentQuestId, client.GetUser().Respect, client.GetUser().DailyRespectPoints, client.GetUser().DailyPetRespectPoints));

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
                using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
                dbClient.RunQuery(stringBuilder.ToString());
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
            foreach (var client in this.GetClients.ToList())
            {

                if (client == null || client.GetConnection() == null)
                {
                    continue;
                }

                try
                {
                    client.GetConnection().Disconnect();
                }
                catch
                {
                }
            }
        }
        catch (Exception ex)
        {
            ExceptionLogger.LogCriticalException(ex.ToString());
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

        var Variable = Client.GetUser().Username.ToLower();
        var str = "user";
        var Expire = WibboEnvironment.GetUnixTimestamp() + LengthSeconds;
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

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
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

    public void SendSuperNotif(string Title, string Notice, string Picture, string Link, string LinkTitle, bool Broadcast, bool Event) => this.SendMessage(new RoomNotificationComposer(Title, Notice, Picture, LinkTitle, Link));

    public ICollection<GameClient> GetClients => this._clients.Values;
}
