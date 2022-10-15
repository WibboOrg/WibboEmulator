namespace WibboEmulator.Communication.RCON;
using System.Net;
using System.Net.Sockets;
using WibboEmulator.Communication.RCON.Commands;
using WibboEmulator.Core;

public class RCONSocket : IDisposable
{
    private readonly Socket _socket;
    private readonly int _musPort;

    private readonly List<string> _allowedIps;
    private readonly CommandManager _commands;

    public RCONSocket(int musPort, string[] allowedIps)
    {
        this._musPort = musPort;
        this._allowedIps = new List<string>();
        foreach (var str in allowedIps)
        {
            this._allowedIps.Add(str);
        }

        try
        {
            this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this._socket.Bind(new IPEndPoint(IPAddress.Any, this._musPort));
            this._socket.Listen(0);
            _ = this._socket.BeginAccept(new AsyncCallback(this.OnNewConnection), this._socket);
        }
        catch (Exception ex)
        {
            throw new ArgumentException("Could not set up MUS socket:\n" + ex.ToString());
        }

        this._commands = new CommandManager();
    }

    public void OnNewConnection(IAsyncResult iAr)
    {
        try
        {
            if (iAr == null || iAr.AsyncState == null)
            {
                return;
            }

            var socket = ((Socket)iAr.AsyncState).EndAccept(iAr);

            var str = socket.RemoteEndPoint?.ToString()?.Split(':')[0];
            if (this._allowedIps.Contains(str) || str == "127.0.0.1")
            {
                _ = new RCONConnection(socket);
            }
            else
            {
                ExceptionLogger.LogDenial("MusSocket Ip non autorisé: " + str);
                socket.Close();
            }
        }
        catch
        {
        }
        _ = this._socket.BeginAccept(new AsyncCallback(this.OnNewConnection), this._socket);
    }

    public CommandManager GetCommands() => this._commands;

    public void Dispose()
    {
        this._socket.Dispose();

        GC.SuppressFinalize(this);
    }
}
