namespace WibboEmulator.Utilities;
using System.Collections.Generic;

internal static class CollectionExtends
{
    public static void AddIfNotExists<T>(this ICollection<T> list, T value)
    {
        if (!list.Contains(value))
        {
            list.Add(value);
        }
    }
}
