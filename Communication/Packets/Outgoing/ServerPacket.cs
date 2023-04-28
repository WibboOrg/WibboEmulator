namespace WibboEmulator.Communication.Packets.Outgoing;
using System.Text;
using WibboEmulator.Communication.Interfaces;

public class ServerPacket : IServerPacket
{
    private readonly Encoding _encoding = Encoding.UTF8;
    private readonly List<byte> _body = new();

    public ServerPacket(int header)
    {
        this._body = new List<byte>();
        this.WriteShort(header);
    }

    public void WriteByte(byte b) => this.WriteByte(new byte[] { b }, false);

    public void WriteShort(int i) => this.WriteByte(BitConverter.GetBytes((short)i), true);

    public void WriteInteger(int i) => this.WriteByte(BitConverter.GetBytes(i), true);

    public void WriteDouble(double i) => this.WriteByte(BitConverter.GetBytes(i), true);

    public void WriteFloat(float i) => this.WriteByte(BitConverter.GetBytes(i), true);

    public void WriteBoolean(bool b) => this.WriteByte(new byte[1] { b ? (byte)1 : (byte)0 }, false);

    public void WriteString(string s)
    {
        var message = this._encoding.GetBytes(s);
        this.WriteShort(message.Length);
        this.WriteByte(message, false);
    }

    public void WriteByte(byte[] b, bool isInt)
    {
        if (isInt)
        {
            for (var i = b.Length - 1; i > -1; --i)
            {
                this._body.Add(b[i]);
            }
        }
        else
        {
            this._body.AddRange(b);
        }
    }

    public int Id { get; }

    public byte[] GetBytes()
    {
        var final = new List<byte>();
        final.AddRange(BitConverter.GetBytes(this._body.Count));
        final.Reverse();
        final.AddRange(this._body);
        return final.ToArray();
    }
}
