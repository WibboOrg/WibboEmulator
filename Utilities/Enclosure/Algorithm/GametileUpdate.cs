namespace WibboEmulator.Utilities.Enclosure.Algorithm;

public class GametileUpdate
{
    public byte Value { get; private set; }

    public int Y { get; private set; }

    public int X { get; private set; }

    public GametileUpdate(int x, int y, byte value)
    {
        this.X = x;
        this.Y = y;
        this.Value = value;
    }
}
