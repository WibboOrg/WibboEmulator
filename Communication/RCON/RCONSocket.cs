namespace WibboEmulator.Communication.RCON;
using System.Net;
using System.Net.Sockets;
using WibboEmulator.Communication.RCON.Commands;
using WibboEmulator.Core;

public static class RCONSocket
{
    private static Socket _socket;
    private static int _musPort;

    private static List<string> _allowedIps;

    public static void Initialize(int musPort, List<string> allowedIps)
    {
        _musPort = musPort;
        _allowedIps = allowedIps;

        try
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Bind(new IPEndPoint(IPAddress.Any, _musPort));
            _socket.Listen(0);
            _ = _socket.BeginAccept(new AsyncCallback(OnNewConnection), _socket);
        }
        catch (Exception ex)
        {
            throw new ArgumentException("Could not set up MUS socket:\n" + ex.ToString());
        }

        RCONCommandManager.Initialize();
    }

    public static void OnNewConnection(IAsyncResult iAr)
    {
        try
        {
            if (iAr == null || iAr.AsyncState == null)
            {
                return;
            }

            var socket = ((Socket)iAr.AsyncState).EndAccept(iAr);

            var str = socket.RemoteEndPoint?.ToString()?.Split(':')[0];
            if (_allowedIps.Contains(str) || str == "127.0.0.1")
            {
                _ = new RCONConnection(socket);
            }
            else
            {
                ExceptionLogger.LogDenial("MusSocket Ip non autorisé: " + str);
                socket.Close();
            }
        }
        catch (Exception ex)
        {
            ExceptionLogger.LogException(ex.ToString());
        }
        _ = _socket.BeginAccept(new AsyncCallback(OnNewConnection), _socket);
    }
}
