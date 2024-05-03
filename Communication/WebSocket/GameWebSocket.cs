namespace WibboEmulator.Communication.WebSocket;
using System.IO;
using System.Net;
using WebSocketSharp;
using WebSocketSharp.Server;
using WibboEmulator.Communication.Packets;
using WibboEmulator.Communication.Packets.Incoming;
using WibboEmulator.Core;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Utilities;

public class GameWebSocket : WebSocketBehavior
{
    private string _ip;

    protected override void OnError(WebSocketSharp.ErrorEventArgs e) => ExceptionLogger.LogException(e.Message);

    protected override void OnClose(CloseEventArgs e) => WebSocketManager.DisposeClient(this);

    protected override void OnMessage(MessageEventArgs e)
    {
        try
        {
            if (this.ConnectionState != WebSocketState.Open)
            {
                return;
            }

            if (!e.IsBinary)
            {
                return;
            }

            var dataDecoded = e.RawData;

            if (dataDecoded.Length < 4)
            {
                return;
            }

            var client = GameClientManager.GetClientById(this.ID);

            if (client == null)
            {
                return;
            }

            using var reader = new BinaryReader(new MemoryStream(dataDecoded));
            var msgLen = IntEncoding.DecodeInt32(reader.ReadBytes(4));

            if (msgLen is < 2 or > 1024000)
            {
                return;
            }
            else if ((reader.BaseStream.Length - 4) < msgLen)
            {
                return;
            }

            var packet = reader.ReadBytes(msgLen);

            using var r = new BinaryReader(new MemoryStream(packet));
            int header = IntEncoding.DecodeInt16(r.ReadBytes(2));

            var content = new byte[packet.Length - 2];
            Buffer.BlockCopy(packet, 2, content, 0, packet.Length - 2);

            var message = new ClientPacket(header, content);

            try
            {
                PacketManager.TryExecutePacket(client, message);
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogPacketException(message.ToString() + " in " + client.User?.Username ?? client.Connection.Ip, ex.ToString());
            }
        }
        catch (Exception ex)
        {
            ExceptionLogger.LogException(ex.ToString());
        }
    }

    protected override void OnOpen() => WebSocketManager.CreatedClient(this);

    public void SendData(byte[] bytes)
    {
        if (this.ConnectionState != WebSocketState.Open)
        {
            return;
        }

        this.Send(bytes);
    }

    public void Disconnect() => this.Close(CloseStatusCode.Normal, "Disconnected");

    public string UserAgent => this.Headers["User-Agent"] ?? "";

    public string Origin => this.Headers["Origin"] ?? "";

    public string Ip
    {
        get
        {
            if (this._ip != null)
            {
                return this._ip;
            }

            this._ip = this.RealIP;

            return this._ip;
        }
    }

    private string RealIP
    {
        get
        {
            var actualIP = this.Context.UserEndPoint.Address;

            if (actualIP.ToString() == "127.0.0.1" && IPAddress.TryParse(this.Headers["X-Real-IP"], out var realIP))
            {
                return realIP.ToString();
            }

            if (IPAddress.TryParse(this.Headers["CF-Connecting-IP"], out var connectiongIP))
            {
                string[] cloudlareIPs = {
                "173.245.48.0/20", "103.21.244.0/22", "103.22.200.0/22", "103.31.4.0/22", "141.101.64.0/18", "108.162.192.0/18", "190.93.240.0/20",
                "188.114.96.0/20", "197.234.240.0/22", "198.41.128.0/17", "162.158.0.0/15", "104.16.0.0/13", "104.24.0.0/14", "172.64.0.0/13",
                "131.0.72.0/22", "2400:cb00::/32", "2606:4700::/32", "2803:f800::/32", "2405:b500::/32", "2405:8100::/32", "2a06:98c0::/29",
                "2c0f:f248::/32"
            };

                foreach (var rangeIP in cloudlareIPs)
                {
                    if (IPRange.IsInSubnet(actualIP, rangeIP))
                    {
                        return connectiongIP.ToString();
                    }
                }
            }

            return actualIP.ToString();
        }
    }
}
