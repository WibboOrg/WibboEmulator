namespace WibboEmulator.Games.GameClients;

using System.Collections.Concurrent;
using System.Diagnostics;
using WibboEmulator.Communication.Interfaces;
using WibboEmulator.Communication.Packets.Outgoing.Handshake;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Notifications;
using WibboEmulator.Communication.WebSocket;
using WibboEmulator.Core.Settings;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.Users.Messenger;

public static class GameClientManager
{
    private static readonly ConcurrentDictionary<string, GameClient> ClientRegister = new();
    private static readonly ConcurrentDictionary<string, string> UsernameRegister = new();
    private static readonly ConcurrentDictionary<int, string> UserIDRegister = new();
    private static readonly ConcurrentDictionary<string, string> SsoTicketRegister = new();
    private static readonly ConcurrentDictionary<string, DateTime> PendingDisconnect = new();
    private static readonly List<int> StaffIds = [];

    public static int Count => UserIDRegister.Count;

    public static void Initialize()
    {
        GameClientCycleStopwatch.Start();
        DisconnectCycleStopwatch.Start();
    }

    public static List<GameClient> StaffUsers
    {
        get
        {
            var users = new List<GameClient>();

            foreach (var userId in StaffIds)
            {
                var client = GetClientByUserID(userId);
                if (client == null || client.User == null)
                {
                    continue;
                }

                users.Add(client);
            }

            return users;
        }
    }

    public static GameClient GetClientById(string clientID)
    {
        _ = TryGetClient(clientID, out var client);

        return client;
    }

    public static GameClient GetClientBySSOTicket(string ssoTicket)
    {
        if (SsoTicketRegister.TryGetValue(ssoTicket.ToLower(), out var clientId))
        {
            if (!TryGetClient(clientId, out var client))
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

    public static GameClient GetClientByUserID(int userID)
    {
        if (UserIDRegister.TryGetValue(userID, out var clientId))
        {
            if (!TryGetClient(clientId, out var client))
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

    public static GameClient GetClientByUsername(string username)
    {
        if (UsernameRegister.TryGetValue(username.ToLower(), out var clientId))
        {
            if (!TryGetClient(clientId, out var client))
            {
                return null;
            }

            return client;
        }
        return null;
    }

    public static bool UpdateClientUsername(string clientId, string oldUsername, string newUsername)
    {
        if (!UsernameRegister.ContainsKey(oldUsername.ToLower()))
        {
            return false;
        }

        _ = UsernameRegister.TryRemove(oldUsername.ToLower(), out _);
        _ = UsernameRegister.TryAdd(newUsername.ToLower(), clientId);
        return true;
    }

    public static bool TryGetClient(string clientId, out GameClient client) => ClientRegister.TryGetValue(clientId, out client);

    public static List<GameClient> GetClientsById(Dictionary<int, MessengerBuddy>.KeyCollection users)
    {
        var clientOnline = new List<GameClient>();
        foreach (var userID in users)
        {
            var client = GetClientByUserID(userID);
            if (client != null)
            {
                clientOnline.Add(client);
            }
        }

        return clientOnline;
    }

    public static void SendMessageStaff(IServerPacket packet)
    {
        foreach (var userId in StaffIds)
        {
            var client = GetClientByUserID(userId);
            if (client == null || client.User == null)
            {
                continue;
            }

            client.SendPacket(packet);
        }
    }

    public static void SendMessage(IServerPacket packet)
    {
        foreach (var client in ClientRegister.Values.ToList())
        {
            if (client == null || client.User == null)
            {
                continue;
            }

            client.SendPacket(packet);
        }
    }

    public static void CreateAndStartClient(string clientID, GameWebSocket connection)
    {
        var client = new GameClient(clientID, connection);
        if (!ClientRegister.TryAdd(clientID, client))
        {
            connection.Disconnect();
        }
    }

    public static void OnCycle()
    {
        PendingDisconnectCycle();
        GameClientCycle();
    }

    private static readonly Stopwatch GameClientCycleStopwatch = new();
    private static void GameClientCycle()
    {
        if (GameClientCycleStopwatch.ElapsedMilliseconds >= 60000)
        {
            GameClientCycleStopwatch.Restart();

            foreach (var client in ClientRegister.Values.ToList())
            {
                if (client == null || client.User == null)
                {
                    continue;
                }

                client.SendPacket(new PingComposer());

                if (client.User.Premium != null && client.User.Rank >= 2)
                {
                    client.User.Premium.CheckPremiumTimeout();
                }

                var creditsTime = DateTime.Now - client.User.LastCreditsTime;

                if (creditsTime > TimeSpan.FromMinutes(20))
                {
                    client.User.LastCreditsTime = DateTime.Now;

                    var amountCredits = SettingsManager.GetData<int>("user.amount.credits.hours");

                    if (amountCredits > 0)
                    {
                        client.User.Credits += amountCredits;
                        client.SendPacket(new CreditBalanceComposer(client.User.Credits));
                    }
                }
            }
        }
    }

    private static readonly Stopwatch DisconnectCycleStopwatch = new();
    private static void PendingDisconnectCycle()
    {
        if (PendingDisconnect.IsEmpty)
        {
            return;
        }

        if (DisconnectCycleStopwatch.ElapsedMilliseconds >= 1000)
        {
            DisconnectCycleStopwatch.Restart();

            var removeIds = new List<string>();
            foreach (var pending in PendingDisconnect)
            {
                var timeExecution = DateTime.Now - pending.Value;

                if (timeExecution <= TimeSpan.FromSeconds(5))
                {
                    continue;
                }

                removeIds.Add(pending.Key);

                DisposeConnection(pending.Key);
            }

            foreach (var id in removeIds)
            {
                _ = PendingDisconnect.TryRemove(id, out _);
            }

            removeIds.Clear();
        }
    }

    public static bool TryReconnection(ref GameClient newClient, string ssoTicket)
    {
        var oldClient = GetClientBySSOTicket(ssoTicket);

        if (oldClient == null)
        {
            return false;
        }

        if (!PendingDisconnect.TryGetValue(oldClient.ConnectionID, out _))
        {
            newClient.IsDisconnected = true;
            return false;
        }

        if (oldClient.IsDisconnected || oldClient.User == null)
        {
            newClient.IsDisconnected = true;
            return false;
        }

        /*if (oldClient.Connection.GetIp() != newClient.Connection.GetIp())
        {
            newClient.IsDisconnected = true;
            return false;
        }*/

        _ = PendingDisconnect.TryRemove(oldClient.ConnectionID, out _);

        _ = ClientRegister.TryRemove(oldClient.ConnectionID, out _);

        //Update oldClient with new connectionId
        oldClient.UpdateClient(newClient);

        //Change the connectionId
        UnregisterClient(oldClient.User.Id, oldClient.User.Username, oldClient.SSOTicket);
        RegisterClient(oldClient, oldClient.User.Id, oldClient.User.Username, oldClient.SSOTicket);

        //Replace newClient per the oldClient
        ClientRegister[newClient.ConnectionID] = oldClient;

        newClient = oldClient;

        return true;
    }

    public static void DisconnectConnection(string clientID)
    {
        if (!TryGetClient(clientID, out var client))
        {
            return;
        }

        if (client.User == null || client.IsDisconnected)
        {
            DisposeConnection(clientID);
            return;
        }

        _ = PendingDisconnect.TryAdd(clientID, DateTime.Now);
    }

    public static void DisposeConnection(string clientID)
    {
        if (!TryGetClient(clientID, out var client))
        {
            return;
        }

        client.Dispose();

        _ = ClientRegister.TryRemove(clientID, out _);
    }

    public static void LogClonesOut(int userId)
    {
        var clientByUserId = GetClientByUserID(userId);
        if (clientByUserId == null)
        {
            return;
        }

        clientByUserId.Disconnect();
    }

    public static void RegisterClient(GameClient client, int userID, string username, string ssoTicket)
    {
        if (UsernameRegister.ContainsKey(username.ToLower()))
        {
            UsernameRegister[username.ToLower()] = client.ConnectionID;
        }
        else
        {
            _ = UsernameRegister.TryAdd(username.ToLower(), client.ConnectionID);
        }

        if (UserIDRegister.ContainsKey(userID))
        {
            UserIDRegister[userID] = client.ConnectionID;
        }
        else
        {
            _ = UserIDRegister.TryAdd(userID, client.ConnectionID);
        }

        if (SsoTicketRegister.ContainsKey(ssoTicket.ToLower()))
        {
            SsoTicketRegister[ssoTicket.ToLower()] = client.ConnectionID;
        }
        else
        {
            _ = SsoTicketRegister.TryAdd(ssoTicket.ToLower(), client.ConnectionID);
        }
    }

    public static void UnregisterClient(int userId, string username, string ssoTicket)
    {
        _ = UserIDRegister.TryRemove(userId, out var _);
        _ = UsernameRegister.TryRemove(username.ToLower(), out _);
        _ = SsoTicketRegister.TryRemove(ssoTicket.ToLower(), out _);
    }

    public static void AddUserStaff(int userId)
    {
        if (!StaffIds.Contains(userId))
        {
            StaffIds.Add(userId);
        }
    }

    public static void RemoveUserStaff(int userId)
    {
        if (StaffIds.Contains(userId))
        {
            _ = StaffIds.Remove(userId);
        }
    }

    public static void CloseAll()
    {
        using var dbClient = DatabaseManager.Connection;

        foreach (var client in Clients.ToList())
        {
            if (client == null)
            {
                continue;
            }

            var user = client.User;

            if (user != null)
            {
                try
                {
                    user.SaveInfo(dbClient);
                }
                catch
                {
                }
            }
        }

        Console.WriteLine("Done saving users inventory!");
        Console.WriteLine("Closing server connections...");

        foreach (var client in Clients.ToList())
        {
            if (client == null || client.Connection == null)
            {
                continue;
            }

            try
            {
                client.Disconnect();
            }
            catch
            {
            }
        }
        ClientRegister.Clear();
        Console.WriteLine("Connections closed!");
    }

    public static void BanUser(GameClient client, string moderator, int lengthSeconds, string reason, bool ipBan)
    {
        if (string.IsNullOrEmpty(reason))
        {
            reason = "Non respect des règles de conditions générales d'utilisations";
        }

        var variable = client.User.Username.ToLower();
        var str = "user";
        var expire = lengthSeconds == -1 ? int.MaxValue : WibboEnvironment.GetUnixTimestamp() + lengthSeconds;
        if (ipBan)
        {
            variable = client.User.IP;
            str = "ip";
        }

        if (variable == "")
        {
            return;
        }

        using (var dbClient = DatabaseManager.Connection)
        {
            if (str == "user")
            {
                UserDao.UpdateIsBanned(dbClient, client.User.Id);
            }

            BanDao.InsertBan(dbClient, expire, str, variable, reason, moderator);
        }

        if (ipBan)
        {
            BanUser(client, moderator, lengthSeconds, reason, false);
        }
        else
        {
            client.Disconnect();
        }
    }

    public static void SendSuperNotification(string title, string notice, string picture, string link, string linkTitle) => SendMessage(new RoomNotificationComposer(title, notice, picture, linkTitle, link));

    public static ICollection<GameClient> Clients => ClientRegister.Values;
}
