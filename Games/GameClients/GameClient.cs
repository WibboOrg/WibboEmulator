namespace WibboEmulator.Games.GameClients;

using WibboEmulator.Communication.Interfaces;
using WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Communication.Packets.Outgoing.Notifications;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Chat;
using WibboEmulator.Communication.WebSocket;
using WibboEmulator.Core.Language;
using WibboEmulator.Games.Users;
using WibboEmulator.Utilities;

public class GameClient
{
    private readonly Dictionary<int, double> _packetTimeout;
    private int _packetCount;
    private double _packetLastTimestamp;

    public string SSOTicket { get; set; }
    public Language Language { get; set; }
    public string ConnectionID { get; set; }
    public bool IsDisconnected { get; set; }
    public GameWebSocket Connection { get; private set; }
    public User User { get; set; }

    public GameClient(string clientId, GameWebSocket connection)
    {
        this.ConnectionID = clientId;
        this.Connection = connection;
        this.Language = Language.French;

        this._packetTimeout = [];
        this._packetCount = 0;
        this._packetLastTimestamp = UnixTimestamp.GetNow();
    }

    public void UpdateClient(GameClient oldClient)
    {
        this.ConnectionID = oldClient.ConnectionID;
        this.Connection = oldClient.Connection;
    }

    public void SendWhisper(string message, bool info = true)
    {
        if (this.User == null || this.User.Room == null)
        {
            return;
        }

        var user = this.User.Room.RoomUserManager.GetRoomUserByName(this.User.Username);
        if (user == null)
        {
            return;
        }

        this.SendPacket(new WhisperComposer(user.VirtualId, message, info ? 34 : 0));
    }

    public void SendNotification(string message) => this.SendPacket(new BroadcastMessageAlertComposer(message));

    public void SendHugeNotification(string message) => this.SendPacket(new MOTDNotificationComposer(message));

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

        if (this._packetCount >= 100)
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
        this.IsDisconnected = true;

        this.User?.Dispose();

        this._packetTimeout?.Clear();
    }

    public void Disconnect()
    {
        this.IsDisconnected = true;

        this.Connection.Disconnect();
    }

    public void SendPacket(ServerPacketList packets)
    {
        if (packets == null || this.Connection == null)
        {
            return;
        }

        if (packets.Count == 0)
        {
            return;
        }

        this.Connection.SendData(packets.Bytes);
    }

    public void SendPacket(IServerPacket packet)
    {
        if (packet == null || this.Connection == null)
        {
            return;
        }

        this.Connection.SendData(packet.Bytes);
    }
}
