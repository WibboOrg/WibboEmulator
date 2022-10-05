namespace WibboEmulator.Games.Rooms.PathFinding;

public struct SquarePoint
{
    private readonly byte _squareData;
    private readonly bool _override;
    private readonly bool _lastStep;
    private readonly bool _allowWalkthrough;
    private readonly byte _squareDataUser;

    public SquarePoint(int x, int y, int targetX, int targetY, byte squareData, bool isOverride, bool allowWalkthrough, byte squareDataUser)
    {
        this.X = x;
        this.Y = y;
        this._squareData = squareData;
        this._squareDataUser = squareDataUser;
        this._override = isOverride;
        this._lastStep = x == targetX && y == targetY;
        this.Distance = GetDistance(x, y, targetX, targetY);
        this._allowWalkthrough = allowWalkthrough;
    }

    public int X { get; }

    public int Y { get; }

    public double Distance { get; }

    public bool CanWalk
    {
        get
        {
            if (this._lastStep)
            {
                return this._override || this._squareData == 3 || this._squareData == 1;
            }
            else
            {
                return this._override || this._squareData == 1;
            }
        }
    }

    public bool AllowWalkthrough => this._allowWalkthrough || this._squareDataUser == 0;

    public static double GetDistance(int x1, int y1, int x2, int y2)
    {
        var dx = x1 - x2;
        var dy = y1 - y2;
        return (dx * dx) + (dy * dy);
    }
}
