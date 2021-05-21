using Butterfly.Communication.Packets.Incoming;
using Butterfly.Communication.WebSocket;
using Butterfly.HabboHotel.WebClients;
using Butterfly.Utilities;
using SharedPacketLib;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Butterfly.Net
{
    public class WebPacketParser : IDataParser, IDisposable, ICloneable
    {
        public event WebPacketParser.HandlePacket OnNewPacket;
        private readonly WebClient _currentClient;

        public WebPacketParser(WebClient client)
        {
            this._currentClient = client;
            this.OnNewPacket = null;
        }

        public void HandlePacketData(byte[] Data, bool deciphered = false)
        {
            try
            {
                if (OnNewPacket == null)
                {
                    return;
                }

                if (Data[0] == 71 && Data[1] == 69)
                {
                    this.PolicyRequest(Data);
                    return;
                }

                try
                {
                    Data = EncodeDecode.DecodeMessage(Data);
                }
                catch
                {
                    return;
                }

                using (BinaryReader Reader = new BinaryReader(new MemoryStream(Data)))
                {
                    if (Data.Length < 4)
                    {
                        return;
                    }

                    int MsgLen = HabboEncoding.DecodeInt32(Reader.ReadBytes(4));

                    if ((Reader.BaseStream.Length) < MsgLen)
                    {
                        return;
                    }

                    if (MsgLen < 0 || MsgLen > 5120)
                    {
                        return;
                    }

                    byte[] Packet = Reader.ReadBytes(MsgLen);

                    using (BinaryReader R = new BinaryReader(new MemoryStream(Packet)))
                    {
                        int Header = HabboEncoding.DecodeInt16(R.ReadBytes(2));

                        byte[] Content = new byte[Packet.Length - 2];
                        Buffer.BlockCopy(Packet, 2, Content, 0, Packet.Length - 2);

                        ClientPacket Message = new ClientPacket(Header, Content);
                        OnNewPacket.Invoke(Message);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Packet Error! " + e);
            }
        }

        private void PolicyRequest(byte[] packet)
        {
            string data = Encoding.UTF8.GetString(packet);
            /* Handshaking and managing ClientSocket */

            if (!data.Contains("ey:"))
            {
                return;
            }

            string key = data.Replace("ey:", "`")
                      .Split('`')[1]                     // dGhlIHNhbXBsZSBub25jZQ== \r\n .......
                      .Replace("\r", "").Split('\n')[0]  // dGhlIHNhbXBsZSBub25jZQ==
                      .Trim();

            // key should now equal dGhlIHNhbXBsZSBub25jZQ==
            string longKey = key + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
            SHA1 sha1 = SHA1.Create();
            byte[] hashBytes = sha1.ComputeHash(Encoding.ASCII.GetBytes(longKey));
            string test1 = Convert.ToBase64String(hashBytes);

            string newLine = "\r\n";

            string response = "HTTP/1.1 101 Switching Protocols" + newLine
                 + "Upgrade: websocket" + newLine
                 + "Connection: Upgrade" + newLine
                 + "Sec-WebSocket-Accept: " + test1 + newLine + newLine
                 ;

            // which one should I use? none of them fires the onopen method
            this._currentClient.GetConnection().SendData(Encoding.UTF8.GetBytes(response));
        }

        public void Dispose()
        {
            this.OnNewPacket = null;
        }

        public object Clone()
        {
            return new WebPacketParser(this._currentClient);
        }

        public delegate void HandlePacket(ClientPacket message);
    }
}
