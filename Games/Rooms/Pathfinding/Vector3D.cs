namespace WibboEmulator.Games.Rooms.PathFinding;

internal sealed class Vector3D
{
    public int X { get; set; }

    public int Y { get; set; }

    public double Z { get; set; }

    public Vector3D() { }

    public Vector3D(int x, int y, double z)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
    }

    public Vector2D ToVector2D() => new(this.X, this.Y);
}