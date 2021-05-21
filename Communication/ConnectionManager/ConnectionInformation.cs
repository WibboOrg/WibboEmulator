using Butterfly.Communication.WebSocket;
using SharedPacketLib;
using System;
using System.Net.Sockets;

namespace ConnectionManager
{
    public class ConnectionInformation : IDisposable
    {
        private readonly Socket _dataSocket;
        private readonly string _ip;
        private readonly int _connectionID;
        private bool _isConnected;
        private readonly byte[] _buffer;
        private readonly AsyncCallback _sendCallback;

        public IDataParser Parser;
        public event ConnectionChange ConnectionClose;
        public delegate void ConnectionChange(ConnectionInformation information);
        public bool IsWebSocket;

        public ConnectionInformation(Socket dataStream, int connectionID, IDataParser parser, string ip)
        {
            this.Parser = parser;
            this._buffer = new byte[GameSocketManagerStatics.BUFFER_SIZE];

            this._dataSocket = dataStream;
            this._dataSocket.SendTimeout = 1000 * 30;
            this._dataSocket.ReceiveTimeout = 1000 * 30;
            this._dataSocket.SendBufferSize = GameSocketManagerStatics.BUFFER_SIZE;
            this._dataSocket.ReceiveBufferSize = GameSocketManagerStatics.BUFFER_SIZE;
            this._dataSocket.DontFragment = false;

            this._sendCallback = new AsyncCallback(this.SentData);

            this._ip = ip;
            this._connectionID = connectionID;
        }

        public void StartPacketProcessing()
        {
            if (this._isConnected)
            {
                return;
            }

            this._isConnected = true;

            try
            {
                this._dataSocket.BeginReceive(this._buffer, 0, this._buffer.Length, SocketFlags.None, this.IncomingDataPacket, this._dataSocket);
            }
            catch
            {
                this.Disconnect();
            }
        }

        public string GetIp()
        {
            return this._ip;
        }

        public int GetConnectionID()
        {
            return this._connectionID;
        }

        public void Disconnect()
        {
            try
            {
                if (!this._isConnected)
                {
                    return;
                }

                this._isConnected = false;

                if (this._dataSocket != null)
                {
                    try
                    {

                        if (this._dataSocket.Connected)
                        {
                            this._dataSocket.Shutdown(SocketShutdown.Both);
                            this._dataSocket.Close();
                        }
                    }

                    catch { }

                    this._dataSocket.Dispose();
                }

                if (this.Parser != null)
                {
                    this.Parser.Dispose();
                }

                if (this.ConnectionClose != null)
                {
                    this.ConnectionClose(this);
                }

                this.ConnectionClose = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void Dispose()
        {
            if (this._isConnected)
            {
                this.Disconnect();
            }

            GC.SuppressFinalize(this);
        }

        private void IncomingDataPacket(IAsyncResult iAr)
        {
            if (!this._isConnected)
            {
                return;
            }

            int length = 0;

            try
            {
                length = this._dataSocket.EndReceive(iAr);
            }
            catch
            {
                this.Disconnect();
                return;
            }

            if (length == 0)
            {
                this.Disconnect();
            }

            if (this.Parser == null)
            {
                return;
            }
            else
            {
                try
                {
                    byte[] packet = new byte[length];
                    Array.Copy(this._buffer, packet, length);

                    this.Parser.HandlePacketData(packet);
                }
                catch
                {
                    this.Disconnect();
                }
                finally
                {
                    try
                    {
                        this._dataSocket.BeginReceive(this._buffer, 0, this._buffer.Length, SocketFlags.None, new AsyncCallback(this.IncomingDataPacket), this._dataSocket);
                    }
                    catch
                    {
                        this.Disconnect();
                    }
                }
            }
        }

        public void SendData(byte[] packet)
        {
            if (!this._isConnected)
            {
                return;
            }

            try
            {
                if (this.IsWebSocket)
                {
                    packet = EncodeDecode.EncodeMessage(packet);
                }

                this._dataSocket.BeginSend(packet, 0, packet.Length, SocketFlags.None, this._sendCallback, null);
            }
            catch
            {
                this.Disconnect();
            }
        }

        private void SentData(IAsyncResult iAr)
        {
            try
            {
                this._dataSocket.EndSend(iAr);
            }
            catch
            {
                this.Disconnect();
            }
        }
    }
}
