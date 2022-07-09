using Wibbo.Communication.RCON.Commands;
using System.Net;
using System.Net.Sockets;

namespace Wibbo.Net
{
    public class RCONSocket
    {
        private readonly Socket _socket;
        private readonly int _musPort;

        private readonly List<string> _allowedIps;
        private readonly CommandManager _commands;

        public RCONSocket(int _musPort, string[] _allowedIps)
        {
            this._musPort = _musPort;
            this._allowedIps = new List<string>();
            foreach (string str in _allowedIps)
            {
                this._allowedIps.Add(str);
            }

            try
            {
                this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this._socket.Bind(new IPEndPoint(IPAddress.Any, this._musPort));
                this._socket.Listen(0);
                this._socket.BeginAccept(new AsyncCallback(this.OnNewConnection), this._socket);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Could not set up MUS socket:\n" + (ex).ToString());
            }

            this._commands = new CommandManager();
        }

        public void OnNewConnection(IAsyncResult iAr)
        {
            try
            {
                Socket _socket = ((Socket)iAr.AsyncState).EndAccept(iAr);
                string str = _socket.RemoteEndPoint.ToString().Split(new char[1] { ':' })[0];
                if (this._allowedIps.Contains(str) || str == "127.0.0.1")
                {
                    new RCONConnection(_socket);
                }
                else
                {
                    Console.WriteLine("MusSocket Ip non autorisé: " + str);
                    _socket.Close();
                }
            }
            catch
            {
            }
            this._socket.BeginAccept(new AsyncCallback(this.OnNewConnection), this._socket);
        }

        public CommandManager GetCommands()
        {
            return this._commands;
        }
    }
}
