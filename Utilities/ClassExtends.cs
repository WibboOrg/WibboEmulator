namespace WibboEmulator.Utilities;
using System.Collections.Generic;

internal static class ClassExtends
{
    public static void AddIfNotExists<T>(this ICollection<T> list, T value)
    {
        if (!list.Contains(value))
        {
            list.Add(value);
        }
    }

    public static T GetData<T>(this Dictionary<string, string> list, string key) where T : IConvertible
    {
        _ = list.TryGetValue(key, out var value);

        return (T)Convert.ChangeType(value, typeof(T));
    }

    public static T ToEnum<T>(this string value, T defaultValue) where T : struct, Enum => Enum.TryParse<T>(value, true, out var result) ? result : defaultValue;

    public static T ToEnum<T>(this int value, T defaultValue) where T : struct, Enum => Enum.IsDefined(typeof(T), value) ? (T)Enum.ToObject(typeof(T), value) : defaultValue;
}
