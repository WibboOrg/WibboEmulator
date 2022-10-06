namespace WibboEmulator.Games.Chat.Filter;
using System.Data;
using System.Text.RegularExpressions;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Database.Interfaces;

public sealed class WordFilterManager
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

        var Data = RoomSwearwordFilterDao.GetAll(dbClient);

        if (Data != null)
        {
            foreach (DataRow Row in Data.Rows)
            {
                this._filteredWords.Add(Convert.ToString(Row["word"]));
            }
        }

        var Data2 = WordFilterRetroDao.GetAll(dbClient);

        if (Data2 != null)
        {
            foreach (DataRow Row in Data2.Rows)
            {
                this._pubWords.Add(Convert.ToString(Row["word"]));
            }
        }
    }

    public void AddFilterPub(string Word)
    {
        if (!this._pubWords.Contains(Word))
        {
            this._pubWords.Add(Word);

            using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
            WordFilterRetroDao.Insert(dbClient, Word);
        }
    }

    public string CheckMessage(string message)
    {
        foreach (var filter in this._filteredWords.ToList())
        {
            if (message.ToLower().Contains(filter))
            {
                message = Regex.Replace(message, filter, "*****", RegexOptions.IgnoreCase);
            }
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
            message = new Regex(@"[^a-z]", RegexOptions.IgnoreCase).Replace(message, string.Empty);
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
        var OldLength = message.Replace(" ", "").Length;

        ClearMessage(ref message, false);

        var LetterDelCount = OldLength - message.Length;

        var WordPub = new List<string>() { "go",
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
                                                    "recru",
                                                    };

        var DetectCount = 0;
        foreach (var Pattern in WordPub)
        {
            if (message.Contains(Pattern))
            {
                DetectCount++;
                continue;
            }
        }

        if (DetectCount >= 4 || (LetterDelCount > 5 && DetectCount >= 4))
        {
            return true;
        }

        foreach (var Filter in this._filteredWords.ToList())
        {
            if (message.Contains(Filter))
            {
                return true;
            }
        }

        return false;
    }
}
