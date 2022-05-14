using Butterfly.Communication.ConnectionManager;
using Butterfly.Communication.Packets.Incoming;
using Butterfly.Core;
using Butterfly.Game.Clients;
using Butterfly.Utilities;
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
        private byte[] _halfData = new byte[0];

        public GamePacketParser(Client me)
        {
            this.OnNewPacket = null;
            this._currentClient = me;
        }

        public void HandlePacketData(byte[] bytes, bool isDecoded = false)
        {
            try
            {
                if (this.OnNewPacket == null)
                {
                    return;
                }

                if (bytes.Length < 4)
                    return;

                bool isFrament = false;

                if (!isDecoded)
                {
                    int controlFrame = ParseControlFrame(bytes[0]);

                    if (controlFrame == (int)ControlFrame.NA)
                    {
                        this.PolicyRequest(bytes);
                        return;
                    }
                    else if ((int)(controlFrame & (int)ControlFrame.Ping) > 0 ||
                        (int)(controlFrame & (int)ControlFrame.Pong) > 0)
                    {
                        return;
                    }
                    else
                    {
                        if ((int)(controlFrame & (int)ControlFrame.CloseConnection) > 0)
                        {
                            return;
                        }

                        if ((int)(controlFrame & (int)ControlFrame.Binary) == 0)
                        {
                            return;
                        }

                        if ((int)(controlFrame & (int)ControlFrame.ContinueFrame) > 0)
                        {
                            isFrament = true;
                        }
                    }
                }

                if(isFrament)
                {
                    this._halfDataRecieved = true;
                }

                if (this._halfDataRecieved)
                {
                    byte[] fullDataRcv = new byte[this._halfData.Length + bytes.Length];
                    Buffer.BlockCopy(this._halfData, 0, fullDataRcv, 0, this._halfData.Length);
                    Buffer.BlockCopy(bytes, 0, fullDataRcv, this._halfData.Length, bytes.Length);

                    if (!isFrament)
                    {
                        this._halfDataRecieved = false;
                        this._halfData = new byte[0];
                        this.HandlePacketData(fullDataRcv);
                    }
                    else
                    {
                        this._halfData = fullDataRcv;
                    }
                    return;
                }


                byte[] dataDecoded = null;
                if (!isDecoded)
                {
                    try
                    {
                        dataDecoded = EncodeDecode.DecodeMessage(bytes);
                    }
                    catch (Exception e)
                    {
                        ExceptionLogger.LogException($"Length: {bytes.Length} Message: {e.Message}");
                        return;
                    }
                }

                if (dataDecoded == null)
                    return;

                if (dataDecoded.Length < 4)
                {
                    return;
                }

                using (BinaryReader reader = new BinaryReader(new MemoryStream(dataDecoded)))
                {
                    int msgLen = IntEncoding.DecodeInt32(reader.ReadBytes(4));

                    if (msgLen < 2 || msgLen > 1024000)
                    {
                        return;
                    }
                    else if ((reader.BaseStream.Length - 4) < msgLen)
                    {
                        this._halfData = bytes;
                        this._halfDataRecieved = true;

                        return;
                    }

                    byte[] packet = reader.ReadBytes(msgLen);

                    using (BinaryReader r = new BinaryReader(new MemoryStream(packet)))
                    {
                        int header = IntEncoding.DecodeInt16(r.ReadBytes(2));

                        byte[] content = new byte[packet.Length - 2];
                        Buffer.BlockCopy(packet, 2, content, 0, packet.Length - 2);

                        ClientPacket message = new ClientPacket(header, content);
                        OnNewPacket.Invoke(message);
                    }

                    if (reader.BaseStream.Length - 4 > msgLen)
                    {
                        byte[] extra = new byte[reader.BaseStream.Length - reader.BaseStream.Position];
                        Buffer.BlockCopy(dataDecoded, (int)reader.BaseStream.Position, extra, 0, ((int)reader.BaseStream.Length - (int)reader.BaseStream.Position));

                        this.HandlePacketData(extra, true);
                    }
                }
            }
            catch (Exception e)
            {
                ExceptionLogger.LogException("Packet Error : " + e.Message);
            }
        }

        public enum ControlFrame { NA = 0, CloseConnection = 1, Ping = 2, Pong = 4, Text = 8, Binary = 16, ContinueFrame = 32, FinalFrame = 64 };

        private int ParseControlFrame(byte controlFrame)
        {
            int rv = (int)ControlFrame.NA;
            bool isFinalFrame = (controlFrame & 0x80) == 0x80;
            byte opCode = (byte)((controlFrame & 0x0F));
            if (opCode >= 0x3 && opCode <= 0x7 ||
                opCode >= 0xB && opCode <= 0xF)//special frame, ignore it
            {
                Console.WriteLine("Reserved Frame received");
                return rv;
            }
            if (opCode == 0x8 || opCode == 0x0 || opCode == 0x1 || opCode == 0x2 || opCode == 0x9 || opCode == 0xA) //proceed furter
            {
                if (opCode == 0x0) //continue frame
                {
                    rv |= (int)ControlFrame.ContinueFrame;
                    Console.WriteLine("Continue Frame received");
                }
                if (opCode == 0x1) //text frame
                {
                    rv |= (int)ControlFrame.Text;
                    Console.WriteLine("Text Frame received");
                }
                if (opCode == 0x2) //binary frame
                {
                    rv |= (int)ControlFrame.Binary;
                    Console.WriteLine("Binary frame received");
                }
                if (opCode == 0x8) //connection closed
                {
                    rv |= (int)ControlFrame.CloseConnection;
                    Console.WriteLine("CloseConnection Frame received");
                }
                if (opCode == 0x9) //ping
                {
                    rv |= (int)ControlFrame.Ping;
                    Console.WriteLine("PING received");
                }
                if (opCode == 0xA) //pong
                {
                    rv |= (int)ControlFrame.Pong;
                    Console.WriteLine("PONG received");
                }
            }
            else // invalid control bit, must close the connection
            {
                Console.WriteLine("invalid control frame received, must close connection");
                rv = (int)ControlFrame.CloseConnection;
            }
            if (isFinalFrame) //Final frame ...
            {
                rv |= (int)ControlFrame.FinalFrame;
                Console.WriteLine("Final frame received");
            }
            else
            {
                rv |= (int)ControlFrame.ContinueFrame;
                Console.WriteLine("Continue frame received");
            }
            return rv;
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
            //this._currentClient.GetConnection().SendData(Encoding.UTF8.GetBytes(response), true);
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
