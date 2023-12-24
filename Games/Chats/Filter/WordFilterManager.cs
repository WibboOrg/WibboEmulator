namespace WibboEmulator.Games.Chats.Filter;
using System.Data;
using System.Text.RegularExpressions;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Database.Interfaces;

public sealed partial class WordFilterManager
{
    private readonly List<string> _filteredWords;
    private readonly List<string> _pubWords;

    public WordFilterManager()
    {
        this._filteredWords = new List<string>();
        this._pubWords = new List<string>();
    }

    public void Init(IQueryAdapter dbClient)
    {
        if (this._filteredWords.Count > 0)
        {
            this._filteredWords.Clear();
        }

        if (this._pubWords.Count > 0)
        {
            this._pubWords.Clear();
        }

        var data = RoomSwearwordFilterDao.GetAll(dbClient);

        if (data != null)
        {
            foreach (DataRow row in data.Rows)
            {
                this._filteredWords.Add(Convert.ToString(row["word"]));
            }
        }

        var dataTwo = WordFilterRetroDao.GetAll(dbClient);

        if (dataTwo != null)
        {
            foreach (DataRow row in dataTwo.Rows)
            {
                this._pubWords.Add(Convert.ToString(row["word"]));
            }
        }
    }

    public void AddFilterPub(string word)
    {
        if (!this._pubWords.Contains(word))
        {
            this._pubWords.Add(word);

            using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
            WordFilterRetroDao.Insert(dbClient, word);
        }
    }

    public string CheckMessage(string message)
    {
        var patternEmail = @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b";
        message = Regex.Replace(message, patternEmail, "*****");

        var patternIP = @"\b(?:\d{1,3}\.){3}\d{1,3}\b";
        message = Regex.Replace(message, patternIP, "*****");

        foreach (var filter in this._filteredWords.ToList())
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

    public bool Ispub(string message)
    {
        if (message.Length <= 3)
        {
            return false;
        }

        ClearMessage(ref message);

        foreach (var pattern in this._pubWords)
        {
            if (message.Contains(pattern))
            {
                return true;
            }
        }

        return false;
    }

    public bool CheckMessageWord(string message)
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

        return this._filteredWords.Any(filter => message.Contains(filter));
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
