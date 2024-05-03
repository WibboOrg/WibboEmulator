namespace WibboEmulator.Utilities.Enclosure.Algorithm;

public class GametileUpdate(int x, int y, byte value)
{
    public byte Value { get; private set; } = value;

    public int Y { get; private set; } = y;

    public int X { get; private set; } = x;
}
