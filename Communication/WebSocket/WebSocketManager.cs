using Butterfly.Communication.Packets.Incoming;
using Butterfly.Core;
using Butterfly.Game.Clients;
using Butterfly.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Butterfly.Communication.WebSocket
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
            if (isSecure)
            {
                string patchCertificate = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/certificate.pfx";
                this._webSocketServer.SslConfiguration.ServerCertificate = new X509Certificate2(patchCertificate, certificatePassword);
            }
            this._webSocketServer.AddWebSocketService<GameWebSocket>("/");
            this._webSocketServer.Start();

            #if DEBUG
            this._webSocketServer.Log.Level = LogLevel.Trace;
            #endif
        }

        public void DisposeClient(GameWebSocket connection)
        {
            string ip = connection.GetIp();

            this.AlterIpConnectionCount(ip, (this.GetAmountOfConnectionFromIp(ip) - 1));

            ButterflyEnvironment.GetGame().GetClientManager().DisposeConnection(connection.ID);
        }

        public void CreatedClient(GameWebSocket connection)
        {
            string ip = connection.GetIp();

            if (ip.Contains(",") || this._bannedIp.Contains(ip) || !ButterflyEnvironment.WebSocketOrigins.Contains(connection.GetOrigin()) || connection.GetUserAgent() == "")
            {
                ExceptionLogger.LogDenial("[IP: " + connection.GetIp() + "] [Origin: " + connection.GetOrigin() + "] [User-Agent: " + connection.GetUserAgent() + "]");
                return;
            }

            this.AlterIpConnectionCount(ip, (this.GetAmountOfConnectionFromIp(ip) + 1));

            int ConnectionCount = this.GetAmountOfConnectionFromIp(ip);
            if (ConnectionCount <= 10)
            {
                ButterflyEnvironment.GetGame().GetClientManager().CreateAndStartClient(connection.ID, connection);
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
            if (ip == "127.0.0.1" || ip == "178.33.7.19")
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
                if (ip == "127.0.0.1" || ip == "178.33.7.19")
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
        protected override void OnError(WebSocketSharp.ErrorEventArgs e)
        {
            //Console.WriteLine(e.Message);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            ButterflyEnvironment.GetWebSocketManager().DisposeClient(this);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            if (this.ConnectionState != WebSocketState.Open)
                return;

            if (!e.IsBinary) return;

            byte[] dataDecoded = e.RawData;

            if (dataDecoded.Length < 4)
            {
                return;
            }

            Client client = ButterflyEnvironment.GetGame().GetClientManager().GetClientById(this.ID);

            if (client == null)
                return;

            using (BinaryReader reader = new BinaryReader(new MemoryStream(dataDecoded)))
            {
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

                using (BinaryReader r = new BinaryReader(new MemoryStream(packet)))
                {
                    int header = IntEncoding.DecodeInt16(r.ReadBytes(2));

                    byte[] content = new byte[packet.Length - 2];
                    Buffer.BlockCopy(packet, 2, content, 0, packet.Length - 2);

                    ClientPacket message = new ClientPacket(header, content);

                    try
                    {
                        ButterflyEnvironment.GetGame().GetPacketManager().TryExecutePacket(client, message);
                    }
                    catch (Exception ex)
                    {
                        ExceptionLogger.LogPacketException(message.ToString(), (ex).ToString());
                    }
                }
            }
        }

        protected override void OnOpen()
        {
            ButterflyEnvironment.GetWebSocketManager().CreatedClient(this);
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
            return this.Headers["CF-Connecting-IP"] ?? this.Context.Host.Split(':')[0];
        }
    }
}
