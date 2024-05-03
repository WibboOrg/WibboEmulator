namespace WibboEmulator.Games.Rooms.PathFinding;

public readonly struct ModelInfo(int maxX, int maxY, byte[,] map, byte[,] userOnMap, byte[,] squareTaking)
{
    public byte GetStateUser(int x, int y)
    {
        if (x >= maxX || x < 0 || y >= maxY || y < 0)
        {
            return 1;
        }

        if (userOnMap[x, y] == 1 || squareTaking[x, y] == 1)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    public byte GetState(int x, int y)
    {
        if (x >= maxX || x < 0 || y >= maxY || y < 0)
        {
            return 0;
        }
        else
        {
            return map[x, y];
        }
    }
}
