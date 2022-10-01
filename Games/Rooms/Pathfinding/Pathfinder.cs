namespace WibboEmulator.Games.Rooms.PathFinding
{
    public static class Pathfinder
    {
        public static SquarePoint GetNextStep(int pUserX, int pUserY, int pUserTargetX, int pUserTargetY, byte[,] pGameMap, double[,] pHeight, byte[,] pUserOnMap, byte[,] pSquareTaking, int MaxX, int MaxY, bool pUserOverride, bool pDiagonal, bool pAllowWalkthrough, bool pOblique)
        {
            ModelInfo pMap = new ModelInfo(MaxX, MaxY, pGameMap, pUserOnMap, pSquareTaking);
            SquarePoint pTarget = new SquarePoint(pUserTargetX, pUserTargetY, pUserTargetX, pUserTargetY, pMap.GetState(pUserTargetX, pUserTargetY), pUserOverride, pAllowWalkthrough, pMap.GetStateUser(pUserTargetX, pUserTargetY));
            if (pUserX == pUserTargetX && pUserY == pUserTargetY)
            {
                return pTarget;
            }
            else
            {
                return GetClosetSqare(new SquareInformation(pUserX, pUserY, pTarget, pMap, pUserOverride, pDiagonal, pAllowWalkthrough, pOblique), new HeightInfo(MaxX, MaxY, pHeight), pUserOverride);
            }
        }

        private static SquarePoint GetClosetSqare(SquareInformation pInfo, HeightInfo Height, bool pUserOverride)
        {
            double Closest = pInfo.Point.Distance;
            SquarePoint squarePoint1 = pInfo.Point;
            double state = Height.GetState(pInfo.Point.X, pInfo.Point.Y);

            for (int val = 0; val < 8; val++)
            {
                SquarePoint squarePoint2 = pInfo.Pos(val);
                if ((squarePoint2.AllowWalkthrough && squarePoint2.CanWalk && ((Height.GetState(squarePoint2.X, squarePoint2.Y) - state < 2)) || pUserOverride))
                {
                    double getDistance = squarePoint2.Distance;
                    if (Closest > getDistance)
                    {
                        Closest = getDistance;
                        squarePoint1 = squarePoint2;
                    }
                }
            }

            return squarePoint1;
        }
    }
}
