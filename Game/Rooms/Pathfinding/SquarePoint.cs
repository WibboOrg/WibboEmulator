namespace Butterfly.Game.Rooms.Pathfinding
{
    public struct SquarePoint
    {
        private readonly int _x;
        private readonly int _y;
        private readonly double _distance;
        private readonly byte _squareData;
        private readonly bool _override;
        private readonly bool _lastStep;
        private readonly bool _allowWalkthrough;
        private readonly byte _squareDataUser;

        public SquarePoint(int pX, int pY, int pTargetX, int pTargetY, byte SquareData, bool pOverride, bool pAllowWalkthrough, byte SquareDataUser)
        {
            this._x = pX;
            this._y = pY;
            this._squareData = SquareData;
            this._squareDataUser = SquareDataUser;
            this._override = pOverride;
            this._lastStep = pX == pTargetX && pY == pTargetY;
            this._distance = GetDistance(pX, pY, pTargetX, pTargetY);
            this._allowWalkthrough = pAllowWalkthrough;
        }

        public int X => this._x;

        public int Y => this._y;

        public double Distance => this._distance;

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
            int dx = (x1 - x2);
            int dy = (y1 - y2);
            return (dx * dx) + (dy * dy);
        }
    }
}
