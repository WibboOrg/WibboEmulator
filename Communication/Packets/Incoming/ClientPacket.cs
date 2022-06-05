using System.Text;

namespace Butterfly.Communication.Packets.Incoming
{
    public class ClientPacket
    {
        private byte[] Body;
        private int Pointer;
        private readonly Encoding Encoding = Encoding.UTF8;

        public ClientPacket(int messageID, byte[] body)
        {
            this.Init(messageID, body);
        }

        public int Id { get; private set; }

        public int RemainingLength => this.Body.Length - this.Pointer;

        public int Header => this.Id;

        public void Init(int messageID, byte[] body)
        {
            if (body == null)
            {
                body = new byte[0];
            }

            this.Id = messageID;
            this.Body = body;

            this.Pointer = 0;
        }

        public byte[] ReadBytes(int Bytes)
        {
            if (Bytes > this.RemainingLength || Bytes < 0)
            {
                Bytes = this.RemainingLength;
            }

            byte[] data = new byte[Bytes];
            for (int i = 0; i < Bytes; i++)
            {
                data[i] = this.Body[this.Pointer++];
            }

            return data;
        }

        public byte[] ReadFixedValue()
        {
            int len = 0;
            if (this.RemainingLength >= 2)
            {
                len = this.DecodeInt16(this.ReadBytes(2));
            }

            return this.ReadBytes(len);
        }

        public string PopString()
        {
            return this.Encoding.GetString(this.ReadFixedValue());
        }

        public bool PopBoolean()
        {
            if (this.RemainingLength > 0 && this.Body[this.Pointer++] == Convert.ToChar(1))
            {
                return true;
            }

            return false;
        }

        public int PopInt()
        {
            if (this.RemainingLength < 4)
            {
                return 0;
            }

            byte[] Data = this.ReadBytes(4);

            int i = this.DecodeInt32(Data);

            return i;
        }

        public override string ToString()
        {
            return "[" + this.Header + "] BODY: " + (this.Encoding.GetString(this.Body).Replace(Convert.ToChar(0).ToString(), "[0]"));
        }

        public int DecodeInt32(byte[] v)
        {

            if ((v[0] | v[1] | v[2] | v[3]) < 0)
            {
                return 0;
            }
            return (v[0] << 24) + (v[1] << 16) + (v[2] << 8) + (v[3]);

        }

        public short DecodeInt16(byte[] v)
        {
            if ((v[0] | v[1]) < 0)
            {
                return 0;
            }
            int result = (v[0] << 8) + (v[1]);
            return (short)result;
        }

    }
}
