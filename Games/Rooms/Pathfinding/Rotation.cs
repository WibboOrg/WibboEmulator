namespace WibboEmulator.Games.Rooms.PathFinding;

public static class Rotation
{
    public static int Calculate(int X1, int Y1, int X2, int Y2)
    {
        var num = 0;
        if (X1 > X2 && Y1 > Y2)
        {
            num = 7;
        }
        else if (X1 < X2 && Y1 < Y2)
        {
            num = 3;
        }
        else if (X1 > X2 && Y1 < Y2)
        {
            num = 5;
        }
        else if (X1 < X2 && Y1 > Y2)
        {
            num = 1;
        }
        else if (X1 > X2)
        {
            num = 6;
        }
        else if (X1 < X2)
        {
            num = 2;
        }
        else if (Y1 < Y2)
        {
            num = 4;
        }
        else if (Y1 > Y2)
        {
            num = 0;
        }

        return num;
    }

    public static int Calculate(int x1, int y1, int x2, int y2, bool moonwalk)
    {
        var rot = Calculate(x1, y1, x2, y2);
        if (!moonwalk)
        {
            return rot;
        }
        else
        {
            return RotationIverse(rot);
        }
    }

    public static int RotationIverse(int rot)
    {
        if (rot > 3)
        {
            rot -= 4;
        }
        else
        {
            rot += 4;
        }

        return rot;
    }
}
