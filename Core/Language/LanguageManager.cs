namespace WibboEmulator.Core.Language;
using System.Data;
using WibboEmulator.Database.Daos.Emulator;

public static class LanguageManager
{
    private static readonly Dictionary<string, string> ValuesFr = new();
    private static readonly Dictionary<string, string> ValuesEn = new();
    private static readonly Dictionary<string, string> ValuesBr = new();

    public static void Initialize(IDbConnection dbClient)
    {
        ValuesFr.Clear();
        ValuesEn.Clear();
        ValuesBr.Clear();

        var emulatorTextList = EmulatorTextDao.GetAll(dbClient);

        foreach (var emulatorText in emulatorTextList)
        {
            var key = emulatorText.Identifiant;

            ValuesFr.Add(key, emulatorText.ValueFr);
            ValuesEn.Add(key, emulatorText.ValueEn);
            ValuesBr.Add(key, emulatorText.ValueBr);
        }
    }

    public static string TryGetValue(string key, Language language)
    {
        if (language == Language.French)
        {
            return ValuesFr.TryGetValue(key, out var value) ? value : "Pas de language locale trouver pour [" + key + "] (fr)";
        }
        else if (language == Language.English)
        {
            return ValuesEn.TryGetValue(key, out var value) ? value : "Pas de language locale trouver pour [" + key + "] (en)";
        }
        else if (language == Language.Portuguese)
        {
            return ValuesBr.TryGetValue(key, out var value) ? value : "Pas de language locale trouver pour [" + key + "] (br)";
        }
        else
        {
            return ValuesFr.TryGetValue(key, out var value) ? value : "Pas de language locale trouver pour [" + key + "] (def)";
        }
    }

    public static Language ParseLanguage(string country) => country switch
    {
        "fr" => Language.French,
        "en" => Language.English,
        "br" => Language.Portuguese,
        _ => Language.French,
    };
}
