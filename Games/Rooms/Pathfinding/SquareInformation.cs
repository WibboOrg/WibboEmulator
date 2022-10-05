namespace WibboEmulator.Games.Rooms.PathFinding;

public struct SquareInformation
{
    private readonly SquarePoint[] _pos;

    public SquareInformation(int x, int y, SquarePoint target, ModelInfo map, bool userOverride, bool calculateDiagonal, bool allowWalkthrough, bool disableOblique)
    {
        this.Point = new SquarePoint(x, y, target.X, target.Y, map.GetState(x, y), userOverride, allowWalkthrough, map.GetStateUser(x, y));
        this._pos = new SquarePoint[8];

        if (calculateDiagonal || userOverride)
        {
            this._pos[1] = new SquarePoint(x - 1, y - 1, target.X, target.Y, map.GetState(x - 1, y - 1), userOverride, allowWalkthrough, map.GetStateUser(x - 1, y - 1));
            this._pos[3] = new SquarePoint(x - 1, y + 1, target.X, target.Y, map.GetState(x - 1, y + 1), userOverride, allowWalkthrough, map.GetStateUser(x - 1, y + 1));
            this._pos[5] = new SquarePoint(x + 1, y + 1, target.X, target.Y, map.GetState(x + 1, y + 1), userOverride, allowWalkthrough, map.GetStateUser(x + 1, y + 1));
            this._pos[7] = new SquarePoint(x + 1, y - 1, target.X, target.Y, map.GetState(x + 1, y - 1), userOverride, allowWalkthrough, map.GetStateUser(x + 1, y - 1));
        }

        if (disableOblique || userOverride)
        {
            this._pos[0] = new SquarePoint(x, y - 1, target.X, target.Y, map.GetState(x, y - 1), userOverride, allowWalkthrough, map.GetStateUser(x, y - 1));
            this._pos[2] = new SquarePoint(x - 1, y, target.X, target.Y, map.GetState(x - 1, y), userOverride, allowWalkthrough, map.GetStateUser(x - 1, y));
            this._pos[4] = new SquarePoint(x, y + 1, target.X, target.Y, map.GetState(x, y + 1), userOverride, allowWalkthrough, map.GetStateUser(x, y + 1));
            this._pos[6] = new SquarePoint(x + 1, y, target.X, target.Y, map.GetState(x + 1, y), userOverride, allowWalkthrough, map.GetStateUser(x + 1, y));
        }
    }

    public SquarePoint Point { get; }

    public SquarePoint Pos(int val) => this._pos[val];
}
