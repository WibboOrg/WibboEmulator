namespace WibboEmulator.Core;

public sealed class IDGenerator
{
    private const string ENCODE_32_CHARS = "0123456789ABCDEFGHIJKLMNOPQRSTUV";
    private static char[] _buffer = new char[13];
    private static long _lastId = DateTime.UtcNow.Ticks;

    private IDGenerator() { }

    /// <summary>
    /// Returns a single instance of the <see cref="IDGenerator"/>.
    /// </summary>
    public static IDGenerator Instance { get; } = new IDGenerator();

    /// <summary>
    /// Returns an ID. e.g: <c>XOGLN1-0HLHI1F5INOFA</c>
    /// </summary>
    public string Next => GenerateImpl(Interlocked.Increment(ref _lastId));

    private static string GenerateImpl(long id)
    {
        var buffer = _buffer;

        buffer[0] = ENCODE_32_CHARS[(int)(id >> 60) & 31];
        buffer[1] = ENCODE_32_CHARS[(int)(id >> 55) & 31];
        buffer[2] = ENCODE_32_CHARS[(int)(id >> 50) & 31];
        buffer[3] = ENCODE_32_CHARS[(int)(id >> 45) & 31];
        buffer[4] = ENCODE_32_CHARS[(int)(id >> 40) & 31];
        buffer[5] = ENCODE_32_CHARS[(int)(id >> 35) & 31];
        buffer[6] = ENCODE_32_CHARS[(int)(id >> 30) & 31];
        buffer[7] = ENCODE_32_CHARS[(int)(id >> 25) & 31];
        buffer[8] = ENCODE_32_CHARS[(int)(id >> 20) & 31];
        buffer[9] = ENCODE_32_CHARS[(int)(id >> 15) & 31];
        buffer[10] = ENCODE_32_CHARS[(int)(id >> 10) & 31];
        buffer[11] = ENCODE_32_CHARS[(int)(id >> 5) & 31];
        buffer[12] = ENCODE_32_CHARS[(int)id & 31];

        return new string(buffer, 0, buffer.Length);
    }
}
