namespace WibboEmulator.Games.Rooms.PathFinding;

public static class Pathfinder
{
    public static SquarePoint GetNextStep(int pUserX, int pUserY, int pUserTargetX, int pUserTargetY, byte[,] pGameMap, double[,] pHeight,
        byte[,] pUserOnMap, byte[,] pSquareTaking, int maxX, int maxY, bool pUserOverride, bool pDiagonal, bool pAllowWalkthrough, bool pOblique)
    {
        var pMap = new ModelInfo(maxX, maxY, pGameMap, pUserOnMap, pSquareTaking);
        var pTarget = new SquarePoint(pUserTargetX, pUserTargetY, pUserTargetX, pUserTargetY, pMap.GetState(pUserTargetX, pUserTargetY), pUserOverride, pAllowWalkthrough, pMap.GetStateUser(pUserTargetX, pUserTargetY));
        if (pUserX == pUserTargetX && pUserY == pUserTargetY)
        {
            return pTarget;
        }
        else
        {
            return GetClosetSqare(new SquareInformation(pUserX, pUserY, pTarget, pMap, pUserOverride, pDiagonal, pAllowWalkthrough, pOblique), new HeightInfo(maxX, maxY, pHeight), pUserOverride);
        }
    }

    private static SquarePoint GetClosetSqare(SquareInformation pInfo, HeightInfo height, bool pUserOverride)
    {
        var closest = pInfo.Point.Distance;
        var squarePoint1 = pInfo.Point;
        var state = height.GetState(pInfo.Point.X, pInfo.Point.Y);

        for (var val = 0; val < 8; val++)
        {
            var squarePoint2 = pInfo.Pos(val);
            if ((squarePoint2.AllowWalkthrough && squarePoint2.CanWalk && height.GetState(squarePoint2.X, squarePoint2.Y) - state < 2) || pUserOverride)
            {
                var getDistance = squarePoint2.Distance;
                if (closest > getDistance)
                {
                    closest = getDistance;
                    squarePoint1 = squarePoint2;
                }
            }
        }

        return squarePoint1;
    }
}
