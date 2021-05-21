using Butterfly.Core;
using System;
using System.Net.Sockets;
using System.Text;

namespace Butterfly.Net
{
    public class RCONConnection
    {
        private byte[] buffer = new byte[1024];
        private Socket socket;

        private readonly Encoding Encoding = Encoding.GetEncoding("Windows-1252");

        public RCONConnection(Socket _socket)
        {
            this.socket = _socket;
            try
            {
                this.socket.BeginReceive(this.buffer, 0, this.buffer.Length, SocketFlags.None, new AsyncCallback(this.OnCallBack), this.socket);
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
                if (!int.TryParse(this.socket.EndReceive(iAr).ToString(), out int bytes))
                {
                    this.Dispose();
                    return;
                }

                string data = this.Encoding.GetString(this.buffer, 0, bytes);

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
                this.socket.Shutdown(SocketShutdown.Both);
                this.socket.Close();
                this.socket.Dispose();
            }
            catch
            {
            }
            this.socket = null;
            this.buffer = null;
        }
    }
}
