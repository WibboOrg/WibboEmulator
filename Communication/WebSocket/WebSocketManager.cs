namespace WibboEmulator.Communication.WebSocket;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using WebSocketSharp;
using WebSocketSharp.Server;
using WibboEmulator.Communication.Packets.Incoming;
using WibboEmulator.Core;
using WibboEmulator.Utilities;

public class WebSocketManager
{
    private readonly WebSocketServer _webSocketServer;

    private readonly ConcurrentDictionary<string, int> _ipConnectionsCount;
    private readonly ConcurrentDictionary<string, int> _lastTimeConnection;
    private readonly List<string> _bannedIp;
    private readonly List<string> _webSocketOrigins;

    public WebSocketManager(int port, bool isSecure, List<string> webSocketOrigins)
    {
        this._ipConnectionsCount = new ConcurrentDictionary<string, int>();
        this._lastTimeConnection = new ConcurrentDictionary<string, int>();
        this._bannedIp = new List<string>();
        this._webSocketOrigins = webSocketOrigins;

        this._webSocketServer = new WebSocketServer(IPAddress.Any, port, isSecure);
        if (isSecure)
        {
            var pemFile = WibboEnvironment.GetSettings().GetData<string>("game.ssl.pem.file.path");
            var pemKeyFile = WibboEnvironment.GetSettings().GetData<string>("game.ssl.keypem.file.path");

            var certificat = X509Certificate2.CreateFromPemFile(pemFile, pemKeyFile);

            if (certificat != null)
            {
                this._webSocketServer.SslConfiguration.ServerCertificate = certificat;
                this._webSocketServer.SslConfiguration.EnabledSslProtocols = SslProtocols.Tls12;
            }
        }
        this._webSocketServer.AddWebSocketService<GameWebSocket>("/", (initializer) => _ = new GameWebSocket() { IgnoreExtensions = true });
        this._webSocketServer.Start();

        this._webSocketServer.Log.File = WibboEnvironment.PatchDir + "/logs/websocketSharp.txt";
        if (Debugger.IsAttached)
        {
            this._webSocketServer.Log.Level = LogLevel.Debug;
        }
        else
        {
            this._webSocketServer.Log.Level = LogLevel.Debug;
        }
    }

    public void DisposeClient(GameWebSocket connection)
    {
        var ip = connection.GetIp();

        this.AlterIpConnectionCount(ip, this.GetAmountOfConnectionFromIp(ip) - 1);

        WibboEnvironment.GetGame().GetGameClientManager().DisposeConnection(connection.ID);
    }

    public void CreatedClient(GameWebSocket connection)
    {
        var ip = connection.GetIp();

        if (ip.Contains(',') || this._bannedIp.Contains(ip) || !this._webSocketOrigins.Contains(connection.GetOrigin()) || connection.GetUserAgent() == "")
        {
            ExceptionLogger.LogDenial("[IP: " + ip + "] [Origin: " + connection.GetOrigin() + "] [User-Agent: " + connection.GetUserAgent() + "]");
            return;
        }

        this.AlterIpConnectionCount(ip, this.GetAmountOfConnectionFromIp(ip) + 1);

        var connectionCount = this.GetAmountOfConnectionFromIp(ip);
        if (connectionCount <= 10)
        {
            WibboEnvironment.GetGame().GetGameClientManager().CreateAndStartClient(connection.ID, connection);
        }
        else
        {
            if (this._lastTimeConnection.ContainsKey(ip))
            {
                if (!this._lastTimeConnection.TryGetValue(ip, out var lastTime))
                {
                    return;
                }

                var now = GetUnixTimestamp();

                if (now - lastTime < 2)
                {
                    this._bannedIp.Add(ip);
                }

                _ = this._lastTimeConnection.TryRemove(ip, out _);
            }
            else
            {
                _ = this._lastTimeConnection.TryAdd(ip, GetUnixTimestamp());
            }
        }
    }

    private static int GetUnixTimestamp() => (int)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

    private void AlterIpConnectionCount(string ip, int amount)
    {
        if (ip == "127.0.0.1")
        {
            return;
        }

        if (this._ipConnectionsCount.ContainsKey(ip))
        {
            _ = this._ipConnectionsCount.TryRemove(ip, out _);
        }

        _ = this._ipConnectionsCount.TryAdd(ip, amount);
    }

    private int GetAmountOfConnectionFromIp(string ip)
    {
        if (ip == "127.0.0.1")
        {
            return 0;
        }

        if (this._ipConnectionsCount.ContainsKey(ip))
        {
            _ = this._ipConnectionsCount.TryGetValue(ip, out var count);
            return count;
        }
        else
        {
            return 0;
        }
    }

    public void ResetBan()
    {
        this._bannedIp.Clear();
        this._lastTimeConnection.Clear();
        this._ipConnectionsCount.Clear();
    }

    public void Destroy() => this._webSocketServer.Stop();
}

public class GameWebSocket : WebSocketBehavior
{
    private string _ip;

    protected override void OnError(WebSocketSharp.ErrorEventArgs e) => ExceptionLogger.LogException(e.Message);

    protected override void OnClose(CloseEventArgs e) => WibboEnvironment.GetWebSocketManager().DisposeClient(this);

    protected override void OnMessage(MessageEventArgs e)
    {
        try
        {
            if (this.ConnectionState != WebSocketState.Open)
            {
                return;
            }

            if (!e.IsBinary)
            {
                return;
            }

            var dataDecoded = e.RawData;

            if (dataDecoded.Length < 4)
            {
                return;
            }

            var client = WibboEnvironment.GetGame().GetGameClientManager().GetClientById(this.ID);

            if (client == null)
            {
                return;
            }

            using var reader = new BinaryReader(new MemoryStream(dataDecoded));
            var msgLen = IntEncoding.DecodeInt32(reader.ReadBytes(4));

            if (msgLen is < 2 or > 1024000)
            {
                return;
            }
            else if ((reader.BaseStream.Length - 4) < msgLen)
            {
                return;
            }

            var packet = reader.ReadBytes(msgLen);

            using var r = new BinaryReader(new MemoryStream(packet));
            int header = IntEncoding.DecodeInt16(r.ReadBytes(2));

            var content = new byte[packet.Length - 2];
            Buffer.BlockCopy(packet, 2, content, 0, packet.Length - 2);

            var message = new ClientPacket(header, content);

            try
            {
                WibboEnvironment.GetGame().GetPacketManager().TryExecutePacket(client, message);
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogPacketException(message.ToString(), ex.ToString());
            }
        }
        catch (Exception ex)
        {
            ExceptionLogger.LogException(ex.ToString());
        }
    }

    protected override void OnOpen() => WibboEnvironment.GetWebSocketManager().CreatedClient(this);

    public void SendData(byte[] bytes)
    {
        if (this.ConnectionState != WebSocketState.Open)
        {
            return;
        }

        this.Send(bytes);
    }

    public void Disconnect() => this.Close();

    public string GetUserAgent() => this.Headers["User-Agent"] ?? "";

    public string GetOrigin() => this.Headers["Origin"] ?? "";

    public string GetIp()
    {
        if (this._ip != null)
        {
            return this._ip;
        }

        this._ip = this.GetRealIP();

        return this._ip;
    }

    private string GetRealIP()
    {
        if (IPAddress.TryParse(this.Headers["CF-Connecting-IP"], out var realIP))
        {
            string[] cloudlareIPs = {
                "173.245.48.0/20", "103.21.244.0/22", "103.22.200.0/22", "103.31.4.0/22", "141.101.64.0/18", "108.162.192.0/18", "190.93.240.0/20",
                "188.114.96.0/20", "197.234.240.0/22", "198.41.128.0/17", "162.158.0.0/15", "104.16.0.0/13", "104.24.0.0/14", "172.64.0.0/13",
                "131.0.72.0/22", "2400:cb00::/32", "2606:4700::/32", "2803:f800::/32", "2405:b500::/32", "2405:8100::/32", "2a06:98c0::/29",
                "2c0f:f248::/32"
            };

            foreach (var rangeIP in cloudlareIPs)
            {
                if (IPRange.IsInSubnet(this.Context.UserEndPoint.Address, rangeIP))
                {
                    return realIP.ToString();
                }
            }
        }

        return this.Context.UserEndPoint.Address.ToString();
    }
}
