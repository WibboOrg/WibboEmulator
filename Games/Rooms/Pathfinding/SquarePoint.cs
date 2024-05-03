namespace WibboEmulator.Games.Rooms.PathFinding;

public readonly struct SquarePoint(int x, int y, int targetX, int targetY, byte squareData, bool isOverride, bool allowWalkthrough, byte squareDataUser)
{
    private readonly bool _lastStep = x == targetX && y == targetY;

    public int X { get; } = x;

    public int Y { get; } = y;

    public double Distance { get; } = GetDistance(x, y, targetX, targetY);

    public bool CanWalk
    {
        get
        {
            if (this._lastStep)
            {
                return isOverride || squareData == 3 || squareData == 1;
            }
            else
            {
                return isOverride || squareData == 1;
            }
        }
    }

    public bool AllowWalkthrough => allowWalkthrough || squareDataUser == 0;

    public static double GetDistance(int x1, int y1, int x2, int y2)
    {
        var dx = x1 - x2;
        var dy = y1 - y2;
        return (dx * dx) + (dy * dy);
    }
}
