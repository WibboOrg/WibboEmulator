using SharedPacketLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ConnectionManager
{
    public class GameSocketManager
    {
        private Socket _connectionListener;
        private bool _acceptConnections;
        private int _maxIpConnectionCount;
        private int _acceptedConnections;
        private IDataParser _parser;
        private ConcurrentDictionary<string, int> _ipConnectionsCount;
        private ConcurrentDictionary<string, int> _lastTimeConnection;
        private List<string> _bannedIp;

        public event ConnectionEvent connectionEvent;

        public void Init(int portID, int connectionsPerIP, IDataParser parser)
        {
            this._ipConnectionsCount = new ConcurrentDictionary<string, int>();
            this._lastTimeConnection = new ConcurrentDictionary<string, int>();

            this._parser = parser;
            this._maxIpConnectionCount = connectionsPerIP;
            this._acceptedConnections = 0;

            this._bannedIp = new List<string>
            {
                "145.239.187.36",
                "149.56.182.243"
            };

            this._connectionListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                this._connectionListener.Bind(new IPEndPoint(IPAddress.Any, portID));
                this._connectionListener.Listen(100);
                this._connectionListener.BeginAccept(new AsyncCallback(this.NewConnectionRequest), this._connectionListener);
                //this.connectionListener.SendBufferSize = GameSocketManagerStatics.BUFFER_SIZE;
                //this.connectionListener.ReceiveBufferSize = GameSocketManagerStatics.BUFFER_SIZE;
                //this.connectionListener.SendTimeout = 1000 * 30;
                //this.connectionListener.ReceiveTimeout = 1000 * 30;
                this._connectionListener.DontFragment = false;
            }
            catch (Exception ex)
            {
                this.Destroy();
                Console.WriteLine(ex);
                return;
            }

            this._acceptConnections = true;
        }

        public void Destroy()
        {
            this._acceptConnections = false;
            try
            {
                this._connectionListener.Close();

            }
            catch
            {
            }
        }

        private void NewConnectionRequest(IAsyncResult iAr)
        {
            if (!this._acceptConnections)
            {
                Console.WriteLine("Connection denied, server is not currently accepting connections!");
                return;
            }
            try
            {
                Socket dataStream = this._connectionListener.EndAccept(iAr);

                string Ip = dataStream.RemoteEndPoint.ToString().Split(new char[1] { ':' })[0];

                if (this._bannedIp.Contains(Ip))
                {
                    return;
                }

                int ConnectionCount = this.GetAmountOfConnectionFromIp(Ip);
                if (ConnectionCount <= 10)
                {
                    Interlocked.Increment(ref this._acceptedConnections);

                    ConnectionInformation connection = new ConnectionInformation(dataStream, this._acceptedConnections, this._parser.Clone() as IDataParser, Ip);

                    connection.ConnectionClose += new ConnectionInformation.ConnectionChange(this.ConnectionChanged);

                    this.ReportUserLogin(Ip);

                    if (this.connectionEvent != null)
                    {
                        this.connectionEvent(connection);
                    }
                }
                else
                {
                    Console.WriteLine("[Connection limit] " + Ip + "(" + ConnectionCount + ")");

                    if (this._lastTimeConnection.ContainsKey(Ip))
                    {
                        if (!this._lastTimeConnection.TryGetValue(Ip, out int lastTime))
                        {
                            return;
                        }

                        int now = GameSocketManager.GetUnixTimestamp();

                        if (now - lastTime < 2)
                        {
                            Console.WriteLine("[Connection banned] " + Ip + "(" + ConnectionCount + ")");

                            this._bannedIp.Add(Ip);
                        }

                        this._lastTimeConnection.TryRemove(Ip, out lastTime);
                    }
                    else
                    {
                        this._lastTimeConnection.TryAdd(Ip, GameSocketManager.GetUnixTimestamp());
                    }
                }

            }
            catch
            {
            }
            finally
            {
                this._connectionListener.BeginAccept(new AsyncCallback(this.NewConnectionRequest), this._connectionListener);
            }
        }

        private static int GetUnixTimestamp()
        {
            return (int)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
        }

        private void ConnectionChanged(ConnectionInformation information)
        {
            this.ReportDisconnect(information);
        }

        public void ReportDisconnect(ConnectionInformation gameConnection)
        {
            gameConnection.ConnectionClose -= new ConnectionInformation.ConnectionChange(this.ConnectionChanged);
            this.ReportUserLogout(gameConnection.GetIp());
        }

        private void ReportUserLogin(string ip)
        {
            this.AlterIpConnectionCount(ip, (this.GetAmountOfConnectionFromIp(ip) + 1));
        }

        private void ReportUserLogout(string ip)
        {
            this.AlterIpConnectionCount(ip, (this.GetAmountOfConnectionFromIp(ip) - 1));
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

        public delegate void ConnectionEvent(ConnectionInformation connection);
    }
}
