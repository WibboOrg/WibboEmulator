namespace WibboEmulator.Games.GameClients;
using WibboEmulator.Communication.Interfaces;
using WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Communication.Packets.Outgoing.Notifications;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Chat;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Notifications;
using WibboEmulator.Communication.Packets.Outgoing.WibboTool;
using WibboEmulator.Communication.WebSocket;
using WibboEmulator.Core.Language;
using WibboEmulator.Database.Daos.Log;
using WibboEmulator.Games.Users;
using WibboEmulator.Utilities;

public class GameClient
{
    private Dictionary<int, double> _packetTimeout;
    private int _packetCount;
    private double _packetLastTimestamp;

    public string MachineId { get; set; }
    public Language Langue { get; set; }
    public string ConnectionID { get; set; }
    public bool ShowGameAlert { get; set; }
    public GameWebSocket Connection { get; private set; }
    public User User { get; set; }

    public GameClient(string clientId, GameWebSocket connection)
    {
        this.ConnectionID = clientId;
        this.Langue = Language.French;
        this.Connection = connection;

        this._packetTimeout = new Dictionary<int, double>();
        this._packetCount = 0;
        this._packetLastTimestamp = UnixTimestamp.GetNow();
    }

    public bool Antipub(string message, string type, int roomId = 0)
    {
        if (this.User == null)
        {
            return false;
        }

        if (this.User.HasPermission("perm_god"))
        {
            return false;
        }

        if (message.Length <= 3)
        {
            return false;
        }

        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        
        LogChatDao.Insert(dbClient, this.User.Id, roomId, message, type, this.User.Username);

        if (!WibboEnvironment.GetGame().GetChatManager().GetFilter().Ispub(message))
        {
            if (WibboEnvironment.GetGame().GetChatManager().GetFilter().CheckMessageWord(message))
            {
                LogChatPubDao.Insert(dbClient, this.User.Id, "A vérifié: " + type + message, this.User.Username);

                foreach (var client in WibboEnvironment.GetGame().GetGameClientManager().GetStaffUsers())
                {
                    if (client == null || client.User == null)
                    {
                        continue;
                    }

                    client.SendPacket(new AddChatlogsComposer(this.User.Id, this.User.Username, type + message));
                }

                return false;
            }

            return false;
        }

        var pubCount = this.User.PubDetectCount++;

        if (type == "<CMD>")
        {
            pubCount = 4;
        }

        LogChatPubDao.Insert(dbClient, this.User.Id, "Pub numero " + pubCount + ": " + type + message, this.User.Username);

        if (pubCount is < 3 and > 0)
        {
            this.SendNotification(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("notif.antipub.warn.1", this.Langue), pubCount));
        }
        else if (pubCount == 3)
        {
            this.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.antipub.warn.2", this.Langue));
        }
        else if (pubCount == 4)
        {
            WibboEnvironment.GetGame().GetGameClientManager().BanUser(this, "Robot", 86400, "Notre Robot a detecte de la pub pour sur le compte " + this.User.Username, true, false);
        }

        foreach (var client in WibboEnvironment.GetGame().GetGameClientManager().GetStaffUsers())
        {
            if (client == null || client.User == null)
            {
                continue;
            }

            client.SendPacket(RoomNotificationComposer.SendBubble("mention", "Detection d'un message suspect sur le compte " + this.User.Username));
            client.SendPacket(new AddChatlogsComposer(this.User.Id, this.User.Username, type + message));
        }
        return true;
    }
    public void SendWhisper(string message, bool info = true)
    {
        if (this.User == null || this.User.CurrentRoom == null)
        {
            return;
        }

        var user = this.User.CurrentRoom.RoomUserManager.GetRoomUserByName(this.User.Username);
        if (user == null)
        {
            return;
        }

        this.SendPacket(new WhisperComposer(user.VirtualId, message, info ? 34 : 0));
    }
    public void SendNotification(string message) => this.SendPacket(new BroadcastMessageAlertComposer(message));

    public void SendHugeNotif(string message) => this.SendPacket(new MOTDNotificationComposer(message));

    public bool PacketTimeout(int packetId, double delay)
    {
        var timestampNow = UnixTimestamp.GetNow();

        if (this._packetLastTimestamp > timestampNow)
        {
            this._packetCount++;
        }
        else
        {
            this._packetCount = 0;
            this._packetLastTimestamp = timestampNow + 1;
        }

        if (this._packetCount >= 20)
        {
            return true;
        }

        if (delay <= 0)
        {
            return false;
        }

        if (this._packetTimeout.TryGetValue(packetId, out var timestamp))
        {
            if (timestamp > timestampNow)
            {
                return true;
            }

            _ = this._packetTimeout.Remove(packetId);
        }

        this._packetTimeout.Add(packetId, timestampNow + (delay / 1000));

        return false;
    }

    public void Dispose()
    {
        if (this.Langue == Language.French)
        {
            WibboEnvironment.GetGame().GetGameClientManager().OnlineUsersFr--;
        }
        else if (this.Langue == Language.English)
        {
            WibboEnvironment.GetGame().GetGameClientManager().OnlineUsersEn--;
        }
        else if (this.Langue == Language.Portuguese)
        {
            WibboEnvironment.GetGame().GetGameClientManager().OnlineUsersBr--;
        }

        if (this.User != null)
        {
            this.User.Dispose();
        }

        if (this._packetTimeout != null)
        {
            this._packetTimeout.Clear();
        }
    }

    public void Disconnect()
    {
        if (this.Connection != null)
        {
            this.Connection.Disconnect();
        }
    }

    public void SendPacket(ServerPacketList packets)
    {
        if (packets == null)
        {
            return;
        }

        if (packets.Count == 0)
        {
            return;
        }

        this.Connection.SendData(packets.GetBytes);
    }

    public void SendPacket(IServerPacket packet)
    {
        if (packet == null || this.Connection == null)
        {
            return;
        }

        this.Connection.SendData(packet.GetBytes());
    }
}
