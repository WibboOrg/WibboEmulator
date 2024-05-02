namespace WibboEmulator.Communication.WebSocket;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using WebSocketSharp;
using WebSocketSharp.Server;
using WibboEmulator.Core;
using WibboEmulator.Core.Settings;
using WibboEmulator.Games.GameClients;

public static class WebSocketManager
{
    private static WebSocketServer _webSocketServer;

    private static readonly ConcurrentDictionary<string, int> IpConnectionsCount = new();
    private static readonly ConcurrentDictionary<string, int> LastTimeConnection = new();
    private static readonly List<string> BannedIp = new();
    private static List<string> _webSocketOrigins;

    public static void Initialize(int port, bool isSecure, List<string> webSocketOrigins)
    {
        _webSocketOrigins = webSocketOrigins;

        _webSocketServer = new WebSocketServer(IPAddress.Any, port, isSecure)
        {
            AllowForwardedRequest = true
        };

        if (isSecure)
        {
            var pemFile = SettingsManager.GetData<string>("game.ssl.pem.file.path");
            var pemKeyFile = SettingsManager.GetData<string>("game.ssl.keypem.file.path");

            var certificat = X509Certificate2.CreateFromPemFile(pemFile, pemKeyFile);

            if (certificat != null)
            {
                _webSocketServer.SslConfiguration.ServerCertificate = certificat;
                _webSocketServer.SslConfiguration.EnabledSslProtocols = SslProtocols.Tls12;
            }
        }
        _webSocketServer.AddWebSocketService<GameWebSocket>("/", (initializer) => _ = new GameWebSocket() { IgnoreExtensions = true });
        _webSocketServer.Start();

        _webSocketServer.Log.Output = (LogData data, string path) => ExceptionLogger.LogWebSocket(data.ToString());

        if (Debugger.IsAttached)
        {
            _webSocketServer.Log.Level = LogLevel.Trace;
        }
        else
        {
            _webSocketServer.Log.Level = LogLevel.Debug;
        }
    }

    public static void DisposeClient(GameWebSocket connection)
    {
        var ip = connection.Ip;

        AlterIpConnectionCount(ip, GetAmountOfConnectionFromIp(ip) - 1);

        GameClientManager.DisconnectConnection(connection.ID);
    }

    public static void CreatedClient(GameWebSocket connection)
    {
        var ip = connection.Ip;

        if (ip.Contains(',') || BannedIp.Contains(ip) || !_webSocketOrigins.Contains(connection.Origin) || connection.UserAgent == "")
        {
            ExceptionLogger.LogDenial("[IP: " + ip + "] [Origin: " + connection.Origin + "] [User-Agent: " + connection.UserAgent + "]");
            return;
        }

        AlterIpConnectionCount(ip, GetAmountOfConnectionFromIp(ip) + 1);

        var connectionCount = GetAmountOfConnectionFromIp(ip);
        if (connectionCount <= 10)
        {
            GameClientManager.CreateAndStartClient(connection.ID, connection);
        }
        else
        {
            if (LastTimeConnection.ContainsKey(ip))
            {
                if (!LastTimeConnection.TryGetValue(ip, out var lastTime))
                {
                    return;
                }

                var now = GetUnixTimestamp();

                if (now - lastTime < 2)
                {
                    BannedIp.Add(ip);
                }

                _ = LastTimeConnection.TryRemove(ip, out _);
            }
            else
            {
                _ = LastTimeConnection.TryAdd(ip, GetUnixTimestamp());
            }
        }
    }

    private static int GetUnixTimestamp() => (int)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

    private static void AlterIpConnectionCount(string ip, int amount)
    {
        if (ip == "127.0.0.1")
        {
            return;
        }

        if (IpConnectionsCount.ContainsKey(ip))
        {
            _ = IpConnectionsCount.TryRemove(ip, out _);
        }

        _ = IpConnectionsCount.TryAdd(ip, amount);
    }

    private static int GetAmountOfConnectionFromIp(string ip)
    {
        if (ip == "127.0.0.1")
        {
            return 0;
        }

        if (IpConnectionsCount.ContainsKey(ip))
        {
            _ = IpConnectionsCount.TryGetValue(ip, out var count);
            return count;
        }
        else
        {
            return 0;
        }
    }

    public static void ResetBan()
    {
        BannedIp.Clear();
        LastTimeConnection.Clear();
        IpConnectionsCount.Clear();
    }

    public static void Destroy() => _webSocketServer.Stop();
}
