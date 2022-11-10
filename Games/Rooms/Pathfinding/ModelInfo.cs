namespace WibboEmulator.Games.Rooms.PathFinding;

public readonly struct ModelInfo
{
    private readonly byte[,] _map;
    private readonly int _maxX;
    private readonly int _maxY;
    private readonly byte[,] _userOnMap;
    private readonly byte[,] _squareTaking;

    public ModelInfo(int maxX, int maxY, byte[,] map, byte[,] userOnMap, byte[,] squareTaking)
    {
        this._map = map;
        this._maxX = maxX;
        this._maxY = maxY;
        this._userOnMap = userOnMap;
        this._squareTaking = squareTaking;
    }

    public byte GetStateUser(int x, int y)
    {
        if (x >= this._maxX || x < 0 || y >= this._maxY || y < 0)
        {
            return 1;
        }

        if (this._userOnMap[x, y] == 1 || this._squareTaking[x, y] == 1)
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
        if (x >= this._maxX || x < 0 || y >= this._maxY || y < 0)
        {
            return 0;
        }
        else
        {
            return this._map[x, y];
        }
    }
}
