using Butterfly.Core;
using System;
using System.Net.Sockets;
using System.Text;

namespace Butterfly.Net
{
    public class RCONConnection
    {
        private byte[] _buffer = new byte[1024];
        private Socket _socket;

        private readonly Encoding Encoding = Encoding.GetEncoding("Windows-1252");

        public RCONConnection(Socket _socket)
        {
            this._socket = _socket;
            try
            {
                this._socket.BeginReceive(this._buffer, 0, this._buffer.Length, SocketFlags.None, new AsyncCallback(this.OnCallBack), this._socket);
            }
            catch
            {
                this.Dispose();
            }
        }

        public void OnCallBack(IAsyncResult iAr)
        {
            try
            {
                if (!int.TryParse(this._socket.EndReceive(iAr).ToString(), out int bytes))
                {
                    this.Dispose();
                    return;
                }

                string data = this.Encoding.GetString(this._buffer, 0, bytes);

                if (!ButterflyEnvironment.GetRCONSocket().GetCommands().Parse(data))
                {
                    Logging.WriteLine("Failed to execute a MUS command. Raw data: " + data);
                }
            }
            catch (Exception ex)
            {
                Logging.LogException("Erreur mus: " + ex);
            }

            this.Dispose();
        }

        public void Dispose()
        {
            try
            {
                this._socket.Shutdown(SocketShutdown.Both);
                this._socket.Close();
                this._socket.Dispose();
            }
            catch
            {
            }
            this._socket = null;
            this._buffer = null;
        }
    }
}
