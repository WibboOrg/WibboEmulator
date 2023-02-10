namespace WibboEmulator.Utilities;
using System.Text.RegularExpressions;

internal static partial class StringCharFilter
{
    private static readonly Regex AllowedChars = MyRegex();
    private static readonly Regex AllowedAlphaNum = MyRegex1();
    private static readonly Regex ScapesRegex = MyRegex2();
    private static readonly Regex BreakLinesRegex = MyRegex3();

    public static bool IsValid(string input) => AllowedChars.IsMatch(input);

    public static bool IsValidAlphaNumeric(char input) => AllowedAlphaNum.IsMatch(input.ToString());

    public static bool IsValidAlphaNumeric(string input) => AllowedAlphaNum.IsMatch(input);


    /// <summary>
    /// Escapes the characters used for injecting special chars from a user input.
    /// </summary>
    /// <param name="str">The string/text to escape.</param>
    /// <param name="allowBreaks">Allow line breaks to be used (\r\n).</param>
    /// <returns></returns>
    public static string Escape(string str, bool allowBreaks = false)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            return string.Empty;
        }

        if (!allowBreaks)
        {
            str = BreakLinesRegex.Replace(str, " ");
        }

        return ScapesRegex.Replace(str, string.Empty);
    }

    [GeneratedRegex("^[a-zA-Z0-9-.]+$")]
    private static partial Regex MyRegex();
    [GeneratedRegex("^[a-zA-Z0-9]+$")]
    private static partial Regex MyRegex1();
    [GeneratedRegex("[\\u0001-\\u0008\\u000B-\\u000C\\u000E-\\u001F\\u007F-\\u009F]")]
    private static partial Regex MyRegex2();
    [GeneratedRegex("[\\r\\n]")]
    private static partial Regex MyRegex3();
}
