namespace WibboEmulator.Games.Rooms.PathFinding;

public readonly struct HeightInfo(int maxX, int maxY, double[,] map)
{
    public double GetState(int x, int y)
    {
        if (x >= maxX || x < 0 || y >= maxY || y < 0)
        {
            return 0.0;
        }
        else
        {
            return map[x, y];
        }
    }
}
