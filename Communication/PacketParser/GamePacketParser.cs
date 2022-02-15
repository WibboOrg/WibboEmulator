using Butterfly.Communication.Packets.Incoming;
using Butterfly.Communication.WebSocket;
using Butterfly.Core;
using Butterfly.Game.Clients;
using Butterfly.Utilities;
using SharedPacketLib;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Butterfly.Net
{
    public class GamePacketParser : IDataParser, IDisposable, ICloneable
    {
        public event HandlePacket OnNewPacket;

        private readonly Client _currentClient;
        private bool _halfDataRecieved = false;
        private byte[] _halfData = null;
        private bool _isWebSocket = false;
        private bool _policySended = false;

        public GamePacketParser(Client me)
        {
            this.OnNewPacket = null;
            this._currentClient = me;
        }

        public void HandlePacketData(byte[] Data, bool deciphered = false)
        {
            try
            {
                if (this.OnNewPacket == null)
                {
                    return;
                }

                if (Data.Length < 4)
                    return;

                if (!this._policySended && (Data[0] == 71 && Data[1] == 69))
                {
                    this.PolicyRequest(Data);

                    this._isWebSocket = true;
                    this._policySended = true;
                    this._currentClient.GetConnection().IsWebSocket = true;

                    return;
                }

                if (!this._policySended && (Data[0] == 60 && Data[1] == 112))
                {
                    this._currentClient.GetConnection().SendData(Encoding.Default.GetBytes(GetXmlPolicy()));

                    this._isWebSocket = false;
                    this._policySended = true;
                    this._currentClient.GetConnection().IsWebSocket = false;

                    return;
                }


                if (this._isWebSocket)
                {
                    try
                    {
                        Data = EncodeDecode.DecodeMessage(Data);
                    }
                    catch (Exception e)
                    {
                        Logging.LogException($"Length: {Data.Length} Message: {e.Message}");
                        return;
                    }
                }

                if (this._currentClient != null && this._currentClient.RC4Client != null && !deciphered)
                {
                    this._currentClient.RC4Client.Decrypt(ref Data);
                }

                if (this._halfDataRecieved)
                {
                    byte[] FullDataRcv = new byte[this._halfData.Length + Data.Length];
                    Buffer.BlockCopy(this._halfData, 0, FullDataRcv, 0, this._halfData.Length);
                    Buffer.BlockCopy(Data, 0, FullDataRcv, this._halfData.Length, Data.Length);

                    this._halfDataRecieved = false; // mark done this round
                    this.HandlePacketData(FullDataRcv, true); // repeat now we have the combined array
                    return;
                }

                using (BinaryReader Reader = new BinaryReader(new MemoryStream(Data)))
                {
                    if (Data.Length < 4)
                    {
                        return;
                    }

                    int MsgLen = HabboEncoding.DecodeInt32(Reader.ReadBytes(4));
                    if (MsgLen < 2)
                    {
                        return;
                    }
                    else if ((Reader.BaseStream.Length - 4) < MsgLen)
                    {
                        if (!this._isWebSocket)
                        {
                            this._halfData = Data;
                            this._halfDataRecieved = true;
                        }

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

                    if (Reader.BaseStream.Length - 4 > MsgLen)
                    {
                        byte[] Extra = new byte[Reader.BaseStream.Length - Reader.BaseStream.Position];
                        Buffer.BlockCopy(Data, (int)Reader.BaseStream.Position, Extra, 0, ((int)Reader.BaseStream.Length - (int)Reader.BaseStream.Position));

                        this.HandlePacketData(Extra, true);
                    }
                }
            }
            catch (Exception e)
            {
                Logging.LogException("Packet Error : " + e.Message);
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

        private static string GetXmlPolicy()
        {
            return "<?xml version=\"1.0\"?>\r\n" +
                          "<!DOCTYPE cross-domain-policy SYSTEM \"/xml/dtds/cross-domain-policy.dtd\">\r\n" +
                          "<cross-domain-policy>\r\n" +
                          "<allow-access-from domain=\"*\" to-ports=\"*\" />\r\n" +
                          "</cross-domain-policy>\x0";
        }

        public void Dispose()
        {
            this.OnNewPacket = null;
            GC.SuppressFinalize(this);
        }

        public object Clone()
        {
            return new GamePacketParser(this._currentClient);
        }

        public delegate void HandlePacket(ClientPacket message);
    }
}
