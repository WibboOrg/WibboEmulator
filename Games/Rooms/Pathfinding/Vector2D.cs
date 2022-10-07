namespace WibboEmulator.Games.Rooms.PathFinding;

public class Vector2D
{
    public static Vector2D Zero { get; set; } = new(0, 0);

    public Vector2D()
    {
    }

    public Vector2D(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    public int X { get; set; }

    public int Y { get; set; }

    public int GetDistanceSquared(Vector2D point)
    {
        var dx = this.X - point.X;
        var dy = this.Y - point.Y;
        return (dx * dx) + (dy * dy);
    }

    public override bool Equals(object obj)
    {
        if (obj is Vector2D v2d)
        {
            return v2d.X == this.X && v2d.Y == this.Y;
        }

        return false;
    }

    public override int GetHashCode() => (this.X + " " + this.Y).GetHashCode();

    public override string ToString() => this.X + ", " + this.Y;

    public static Vector2D operator +(Vector2D one, Vector2D two) => new(one.X + two.X, one.Y + two.Y);

    public static Vector2D operator -(Vector2D one, Vector2D two) => new(one.X - two.X, one.Y - two.Y);
}
