namespace WibboEmulator.Games.Chats.Filter;
using System.Data;
using System.Text.RegularExpressions;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Daos.Room;

public static partial class WordFilterManager
{
    private static readonly List<string> FilteredWords = new();
    private static readonly List<string> PubWords = new();

    public static void Initialize(IDbConnection dbClient)
    {
        FilteredWords.Clear();
        PubWords.Clear();

        var worlds = RoomSwearwordFilterDao.GetAll(dbClient);

        foreach (var world in worlds)
        {
            FilteredWords.Add(world);
        }

        var worldFilterRetros = WordFilterRetroDao.GetAll(dbClient);

        PubWords.AddRange(worldFilterRetros);
    }

    public static void AddFilterPub(string word)
    {
        if (!PubWords.Contains(word))
        {
            PubWords.Add(word);

            using var dbClient = DatabaseManager.Connection;
            WordFilterRetroDao.Insert(dbClient, word);
        }
    }

    public static string CheckMessage(string message)
    {
        var patternEmail = @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b";
        message = Regex.Replace(message, patternEmail, "*****");

        var patternIP = @"\b(?:\d{1,3}\.){3}\d{1,3}\b";
        message = Regex.Replace(message, patternIP, "*****");

        foreach (var filter in FilteredWords.ToList())
        {
            message = Regex.Replace(message, filter, "*****", RegexOptions.IgnoreCase);
        }

        return message;
    }

    private static void StringTranslate(ref string input, string frm, char to)
    {
        for (var i = 0; i < frm.Length; i++)
        {
            input = input.Replace(frm[i], to);
        }
    }

    private static void ClearMessage(ref string message, bool onlyLetter = true)
    {
        message = message.Replace("()", "o").Replace("Æ", "ae");

        StringTranslate(ref message, "ĀāĂăĄąΑΔΆÀÁÂÃÄÅàáâãäå4@å", 'a');
        StringTranslate(ref message, "ßΒþ", 'b');
        StringTranslate(ref message, "¢©ĆćĈĉĊċČč€Çç", 'c');
        StringTranslate(ref message, "ĎďĐđ", 'd');
        StringTranslate(ref message, "ĒēĔĕĖėĘęĚěΈÈÉÊËèéêë3", 'e');
        StringTranslate(ref message, "ĜĝĞğĠġĢģ", 'g');
        StringTranslate(ref message, "ĤĥĦħΉ", 'h');
        StringTranslate(ref message, "¡ĨĩĪīĬĭĮįİıΊΐìíîïÌÍÎÏ1!", 'i');
        StringTranslate(ref message, "Ĵĵ", 'j');
        StringTranslate(ref message, "Ķķĸ", 'k');
        StringTranslate(ref message, "£¦ĹĺĻļĽľĿŀŁłℓ", 'l');
        StringTranslate(ref message, "M", 'm');
        StringTranslate(ref message, "ŃńŅņŇňŉŊŋπñÑ", 'n');
        StringTranslate(ref message, "¤ŌōŎŏŐőΌoOòóôõöÒÓÔÕÖøΩð0", 'o');
        StringTranslate(ref message, "Pp₱", 'p');
        StringTranslate(ref message, "ŔŕŖŗŘřя®", 'r');
        StringTranslate(ref message, "§ŚśŜŝŞşSŠš", 's');
        StringTranslate(ref message, "ŢţŤťŦŧ", 't');
        StringTranslate(ref message, "ųŨũŪūŬŭŮůŰűŲųùúûüÙÚÛÜ", 'u');
        StringTranslate(ref message, "√", 'v');
        StringTranslate(ref message, "Ŵŵω", 'w');
        StringTranslate(ref message, "×", 'x');
        StringTranslate(ref message, "ŶŷΎýÿÝÝ", 'y');
        StringTranslate(ref message, "ŹźŻż", 'z');

        if (onlyLetter)
        {
            message = MyRegex().Replace(message, string.Empty);
        }

        message = message.ToLower();
    }

    public static bool Ispub(string message)
    {
        if (message.Length <= 3)
        {
            return false;
        }

        ClearMessage(ref message);

        foreach (var pattern in PubWords)
        {
            if (message.Contains(pattern))
            {
                return true;
            }
        }

        return false;
    }

    public static bool CheckMessageWord(string message)
    {
        if (ContainsIPAddress(message))
        {
            return true;
        }

        var originalLength = message.Replace(" ", "").Length;

        ClearMessage(ref message, false);

        var letterDeletionCount = originalLength - message.Length;

        var forbiddenWords = new List<string>() {
            "go",
            ".fr",
            ".com",
            ".me",
            ".org",
            ".be",
            ".eu",
            ".net",
            "mobi",
            "nouveau",
            "nouvo",
            "connect",
            "invite",
            "recru",
            "staff",
            "ouvr",
            "rejoign",
            "retro",
            "meilleur",
            "direction",
            "rejoin",
            "gratuit",
            "open",
            "http",
            "recrutement",
            "animation",
            "habb",
            "bbo",
            "sansle",
            "city",
            "alpha",
            "gosur",
            "=bb",
            "catalogue",
            "recru"
        };

        var detectedCount = forbiddenWords.Count(word => message.Contains(word));

        if (detectedCount >= 4 || (letterDeletionCount > 5 && detectedCount >= 4))
        {
            return true;
        }

        return FilteredWords.Any(filter => message.Contains(filter));
    }

    private static bool ContainsIPAddress(string message)
    {
        var regex = RegexIp();
        return regex.IsMatch(message);
    }

    [GeneratedRegex("[^a-z]", RegexOptions.IgnoreCase, "fr-BE")]
    private static partial Regex MyRegex();

    [GeneratedRegex("\\b(?:\\d{1,3}\\.){3}\\d{1,3}\\b")]
    private static partial Regex RegexIp();
}
