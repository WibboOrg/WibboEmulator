namespace WibboEmulator.Utilities;
using WibboEmulator.Communication.Interfaces;

public class ServerPacketList
{
    private byte[] _totalBytes;
    private int _current;

    public ServerPacketList()
    {
        this._totalBytes = Array.Empty<byte>();
        this._current = 0;
        this.Count = 0;
    }

    public void Add(IServerPacket packet)
    {
        var toAdd = packet.Bytes;

        var newLen = this._totalBytes.Length + toAdd.Length;

        Array.Resize(ref this._totalBytes, newLen);

        for (var i = 0; i < toAdd.Length; i++)
        {
            this._totalBytes[this._current] = toAdd[i];
            this._current++;
        }

        this.Count++;
    }

    public void Clear()
    {
        this._totalBytes = Array.Empty<byte>();
        this._current = 0;
        this.Count = 0;
    }

    public int Count { get; private set; }

    public byte[] Bytes => this._totalBytes;
}
