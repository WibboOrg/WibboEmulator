namespace WibboEmulator.Games.Rooms.PathFinding;

public struct HeightInfo
{
    private readonly double[,] _map;
    private readonly int _maxX;
    private readonly int _maxY;

    public HeightInfo(int maxX, int maxY, double[,] map)
    {
        this._map = map;
        this._maxX = maxX;
        this._maxY = maxY;
    }

    public double GetState(int x, int y)
    {
        if (x >= this._maxX || x < 0 || y >= this._maxY || y < 0)
        {
            return 0.0;
        }
        else
        {
            return this._map[x, y];
        }
    }
}
