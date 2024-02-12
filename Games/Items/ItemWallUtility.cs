namespace WibboEmulator.Games.Items;
using System;
using System.Linq;
using WibboEmulator.Core;

public class ItemWallUtility
{
    public static string WallPositionCheck(string wallPosition)
    {
        const int maxWidthHeight = 2000;

        try
        {
            if (wallPosition.Contains((char)13) || wallPosition.Contains((char)9))
            {
                return ":w=0,0 l=0,0 l";
            }

            var parts = wallPosition.Split(' ');
            if (parts.Length != 3 || !(parts[2] == "l" || parts[2] == "r"))
            {
                return ":w=0,0 l=0,0 l";
            }

            var dimensions = parts[0][3..].Split(',').Select(int.Parse).ToArray();
            var size = (dimensions[0], dimensions[1]);
            if (size.Item1 < 0 || size.Item1 > maxWidthHeight || size.Item2 < 0 || size.Item2 > maxWidthHeight)
            {
                return ":w=0,0 l=0,0 l";
            }

            var length = parts[1][2..].Split(',').Select(int.Parse).ToArray();
            if (length[0] < 0 || length[0] > maxWidthHeight || length[1] < 0 || length[1] > maxWidthHeight)
            {
                return ":w=0,0 l=0,0 l";
            }

            return $":w={size.Item1},{size.Item2} l={length[0]},{length[1]} {parts[2]}";
        }
        catch (Exception ex)
        {
            ExceptionLogger.LogException(ex.ToString());
            return ":w=0,0 l=0,0 l";
        }
    }
}
