using WibboEmulator.Communication.Packets.Incoming;
using WibboEmulator.Core;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Utilities;
using System.Collections.Concurrent;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Security.Authentication;

namespace WibboEmulator.Communication.WebSocket
{
    public class WebSocketManager
    {
        private WebSocketServer _webSocketServer;

        private readonly ConcurrentDictionary<string, int> _ipConnectionsCount;
        private readonly ConcurrentDictionary<string, int> _lastTimeConnection;
        private readonly List<string> _bannedIp;

        public WebSocketManager(int port, bool isSecure, string certificatePassword)
        {
            this._ipConnectionsCount = new ConcurrentDictionary<string, int>();
            this._lastTimeConnection = new ConcurrentDictionary<string, int>();
            this._bannedIp = new List<string>();

            this._webSocketServer = new WebSocketServer(IPAddress.Any, port, isSecure);
            this._webSocketServer.KeepClean = false;
            if (isSecure)
            {
                string pemFile = WibboEnvironment.GetSettings().GetData<string>("game.ssl.pem.file.path");
                string pemKeyFile = WibboEnvironment.GetSettings().GetData<string>("game.ssl.keypem.file.path");

                var certificat = X509Certificate2.CreateFromPemFile(pemFile, pemKeyFile);

                if (certificat != null)
                {
                    this._webSocketServer.SslConfiguration.ServerCertificate = certificat;
                    this._webSocketServer.SslConfiguration.EnabledSslProtocols = SslProtocols.Tls13;
                }
            }
            this._webSocketServer.AddWebSocketService<GameWebSocket>("/", (initializer) => new GameWebSocket() { IgnoreExtensions = true });
            this._webSocketServer.Start();

            #if DEBUG
            this._webSocketServer.Log.Level = LogLevel.Trace;
            #endif
        }

        public void DisposeClient(GameWebSocket connection)
        {
            string ip = connection.GetIp();

            this.AlterIpConnectionCount(ip, (this.GetAmountOfConnectionFromIp(ip) - 1));

            WibboEnvironment.GetGame().GetGameClientManager().DisposeConnection(connection.ID);
        }

        public void CreatedClient(GameWebSocket connection)
        {
            string ip = connection.GetIp();

            if (ip.Contains(',') || this._bannedIp.Contains(ip) || !WibboEnvironment.WebSocketOrigins.Contains(connection.GetOrigin()) || connection.GetUserAgent() == "")
            {
                ExceptionLogger.LogDenial("[IP: " + ip + "] [Origin: " + connection.GetOrigin() + "] [User-Agent: " + connection.GetUserAgent() + "]");
                return;
            }

            this.AlterIpConnectionCount(ip, (this.GetAmountOfConnectionFromIp(ip) + 1));

            int ConnectionCount = this.GetAmountOfConnectionFromIp(ip);
            if (ConnectionCount <= 10)
            {
                WibboEnvironment.GetGame().GetGameClientManager().CreateAndStartClient(connection.ID, connection);
            }
            else
            {
                if (this._lastTimeConnection.ContainsKey(ip))
                {
                    if (!this._lastTimeConnection.TryGetValue(ip, out int lastTime))
                    {
                        return;
                    }

                    int now = GetUnixTimestamp();

                    if (now - lastTime < 2)
                    {
                        this._bannedIp.Add(ip);
                    }

                    this._lastTimeConnection.TryRemove(ip, out lastTime);
                }
                else
                {
                    this._lastTimeConnection.TryAdd(ip, GetUnixTimestamp());
                }
            }
        }

        private static int GetUnixTimestamp()
        {
            return (int)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
        }

        private void AlterIpConnectionCount(string ip, int amount)
        {
            if (ip == "127.0.0.1")
            {
                return;
            }

            if (this._ipConnectionsCount.ContainsKey(ip))
            {
                this._ipConnectionsCount.TryRemove(ip, out int am);
            }

            this._ipConnectionsCount.TryAdd(ip, amount);
        }

        private int GetAmountOfConnectionFromIp(string ip)
        {
            try
            {
                if (ip == "127.0.0.1")
                {
                    return 0;
                }

                if (this._ipConnectionsCount.ContainsKey(ip))
                {
                    this._ipConnectionsCount.TryGetValue(ip, out int Count);
                    return Count;
                }
                else
                {
                    return 0;
                }
            }
            catch
            {
                return 0;
            }
        }

        public void Destroy()
        {
            this._webSocketServer.Stop();
            this._webSocketServer = null;
        }
    }

    public class GameWebSocket : WebSocketBehavior
    {
        private string _ip;

        protected override void OnError(WebSocketSharp.ErrorEventArgs e)
        {
            ExceptionLogger.LogException(e.Message);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            WibboEnvironment.GetWebSocketManager().DisposeClient(this);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            try
            {
                if (this.ConnectionState != WebSocketState.Open)
                    return;

                if (!e.IsBinary) return;

                byte[] dataDecoded = e.RawData;

                if (dataDecoded.Length < 4)
                {
                    return;
                }

                GameClient client = WibboEnvironment.GetGame().GetGameClientManager().GetClientById(this.ID);

                if (client == null)
                    return;

                using BinaryReader reader = new BinaryReader(new MemoryStream(dataDecoded));
                int msgLen = IntEncoding.DecodeInt32(reader.ReadBytes(4));

                if (msgLen < 2 || msgLen > 1024000)
                {
                    return;
                }
                else if ((reader.BaseStream.Length - 4) < msgLen)
                {
                    return;
                }

                byte[] packet = reader.ReadBytes(msgLen);

                using BinaryReader r = new BinaryReader(new MemoryStream(packet));
                int header = IntEncoding.DecodeInt16(r.ReadBytes(2));

                byte[] content = new byte[packet.Length - 2];
                Buffer.BlockCopy(packet, 2, content, 0, packet.Length - 2);

                ClientPacket message = new ClientPacket(header, content);

                try
                {
                    WibboEnvironment.GetGame().GetPacketManager().TryExecutePacket(client, message);
                }
                catch (Exception ex)
                {
                    ExceptionLogger.LogPacketException(message.ToString(), (ex).ToString());
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException((ex).ToString());
            }
        }

        protected override void OnOpen()
        {
            WibboEnvironment.GetWebSocketManager().CreatedClient(this);
        }

        public void SendData(byte[] bytes)
        {
            if (this.ConnectionState != WebSocketState.Open)
                return;

            this.Send(bytes);
        }

        public void Dispose()
        {
            this.Close();
        }

        public string GetUserAgent()
        {
            return this.Headers["User-Agent"] ?? "";
        }

        public string GetOrigin()
        {
            return this.Headers["Origin"] ?? "";
        }

        public string GetIp()
        {
            if (this._ip != null) 
                return this._ip;

            this._ip = GetRealIP();

            return this._ip;
        }

        private string GetRealIP()
        {
            if (IPAddress.TryParse(this.Headers["CF-Connecting-IP"], out IPAddress realIP))
            {
                string[] cloudlareIPs = { 
                    "173.245.48.0/20", "103.21.244.0/22", "103.22.200.0/22", "103.31.4.0/22", "141.101.64.0/18", "108.162.192.0/18", "190.93.240.0/20", 
                    "188.114.96.0/20", "197.234.240.0/22", "198.41.128.0/17", "162.158.0.0/15", "104.16.0.0/13", "104.24.0.0/14", "172.64.0.0/13",
                    "131.0.72.0/22", "2400:cb00::/32", "2606:4700::/32", "2803:f800::/32", "2405:b500::/32", "2405:8100::/32", "2a06:98c0::/29",
                    "2c0f:f248::/32" 
                };

                foreach (string rangeIP in cloudlareIPs)
                {
                    if (IPRange.IsInSubnet(this.Context.UserEndPoint.Address, rangeIP))
                        return realIP.ToString();
                }
            }

            return this.Context.UserEndPoint.Address.ToString();
        }
    }
}
