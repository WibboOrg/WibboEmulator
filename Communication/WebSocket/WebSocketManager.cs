using NetCoreServer;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using WibboEmulator.Communication.Packets.Incoming;
using WibboEmulator.Core;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Utilities;
using Buffer = System.Buffer;

namespace WibboEmulator.Communication.WebSocket
{
    public class WebSocketManager
    {
        private GameServer _webSocketServer;

        private readonly ConcurrentDictionary<string, int> _ipConnectionsCount;
        private readonly ConcurrentDictionary<string, int> _lastTimeConnection;
        private readonly List<string> _bannedIp;

        public WebSocketManager(int port, bool isSecure)
        {
            this._ipConnectionsCount = new ConcurrentDictionary<string, int>();
            this._lastTimeConnection = new ConcurrentDictionary<string, int>();
            this._bannedIp = new List<string>();

            if (isSecure)
            {
                string pemFile = WibboEnvironment.GetSettings().GetData<string>("game.ssl.pem.file.path");
                string pemKeyFile = WibboEnvironment.GetSettings().GetData<string>("game.ssl.keypem.file.path");

                var certificat = X509Certificate2.CreateFromPemFile(pemFile, pemKeyFile);

                if (certificat != null)
                {
                    var context = new SslContext(SslProtocols.Tls12, certificat);

                    this._webSocketServer = new GameServer(context, IPAddress.Any, port);
                }
            }
            this._webSocketServer.Start();
        }

        public void DisposeClient(GameWebSocket connection)
        {
            string ip = connection.GetIp();

            this.AlterIpConnectionCount(ip, (this.GetAmountOfConnectionFromIp(ip) - 1));

            WibboEnvironment.GetGame().GetGameClientManager().DisposeConnection(connection.Id.ToString());
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
                WibboEnvironment.GetGame().GetGameClientManager().CreateAndStartClient(connection.Id.ToString(), connection);
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

    class GameServer : WssServer
    {
        public GameServer(SslContext context, IPAddress address, int port) : base(context, address, port) { }

        protected override WssSession CreateSession() => new GameWebSocket(this);

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Chat WebSocket server caught an error with code {error}");
        }
    }

    public class GameWebSocket : WssSession
    {
        private string _ip;
        private readonly Dictionary<string, string> _headerList = new();

        public GameWebSocket(WssServer server) : base(server) { }


        protected override void OnError(SocketError error)
        {
            ExceptionLogger.LogException(error.ToString());
        }

        public override void OnWsDisconnected()
        {
            WibboEnvironment.GetWebSocketManager().DisposeClient(this);
        }

        public override void OnWsReceived(byte[] buffer, long offset, long size)
        {
            try
            {
                if (!this.IsConnected)
                    return;

                byte[] dataDecoded = buffer;

                if (dataDecoded.Length < 4)
                {
                    return;
                }

                GameClient client = WibboEnvironment.GetGame().GetGameClientManager().GetClientById(this.Id.ToString());

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

        public override void OnWsConnected(HttpRequest request)
        {
            base.OnWsConnected(request);

            for (int i = 0; i < request.Headers; i++)
            {
                var header = request.Header(i);

                this._headerList.Add(header.Item1, header.Item2);
            }

            WibboEnvironment.GetWebSocketManager().CreatedClient(this);
        }

        public void SendData(byte[] bytes)
        {
            this.SendBinary(bytes);
        }

        public string GetUserAgent()
        {
            return this._headerList["User-Agent"] ?? "";
        }

        public string GetOrigin()
        {
            return this._headerList["Origin"] ?? "";
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
            var RemoteIP = this.Socket.RemoteEndPoint.ToString().Split(':')[0];

            if (!this._headerList.ContainsKey("CF-Connecting-IP"))
                return RemoteIP;

            if (IPAddress.TryParse(this._headerList["CF-Connecting-IP"], out IPAddress realIP))
            {
                string[] cloudlareIPs = { 
                    "173.245.48.0/20", "103.21.244.0/22", "103.22.200.0/22", "103.31.4.0/22", "141.101.64.0/18", "108.162.192.0/18", "190.93.240.0/20", 
                    "188.114.96.0/20", "197.234.240.0/22", "198.41.128.0/17", "162.158.0.0/15", "104.16.0.0/13", "104.24.0.0/14", "172.64.0.0/13",
                    "131.0.72.0/22", "2400:cb00::/32", "2606:4700::/32", "2803:f800::/32", "2405:b500::/32", "2405:8100::/32", "2a06:98c0::/29",
                    "2c0f:f248::/32" 
                };

                foreach (string rangeIP in cloudlareIPs)
                {
                    if (IPRange.IsInSubnet(IPAddress.Parse(this.Socket.RemoteEndPoint.ToString()), rangeIP))
                        return realIP.ToString();
                }
            }

            return RemoteIP;
        }
    }
}
