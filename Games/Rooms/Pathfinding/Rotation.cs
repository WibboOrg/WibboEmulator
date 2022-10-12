namespace WibboEmulator.Games.Rooms.PathFinding;

public static class Rotation
{
    public static int Calculate(int x1, int y1, int x2, int y2)
    {
        var num = 0;
        if (x1 > x2 && y1 > y2)
        {
            num = 7;
        }
        else if (x1 < x2 && y1 < y2)
        {
            num = 3;
        }
        else if (x1 > x2 && y1 < y2)
        {
            num = 5;
        }
        else if (x1 < x2 && y1 > y2)
        {
            num = 1;
        }
        else if (x1 > x2)
        {
            num = 6;
        }
        else if (x1 < x2)
        {
            num = 2;
        }
        else if (y1 < y2)
        {
            num = 4;
        }
        else if (y1 > y2)
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
            return RotationInverse(rot);
        }
    }

    public static int RotationInverse(int rot)
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
