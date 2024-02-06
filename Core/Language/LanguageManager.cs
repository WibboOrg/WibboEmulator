namespace WibboEmulator.Core.Language;
using System.Data;
using WibboEmulator.Database.Daos.Emulator;

public enum Language
{
    None,
    French,
    English,
    Portuguese,
}

public class LanguageManager
{
    private readonly Dictionary<string, string> _valuesFr;
    private readonly Dictionary<string, string> _valuesEn;
    private readonly Dictionary<string, string> _valuesBr;

    public LanguageManager()
    {
        this._valuesFr = new Dictionary<string, string>();
        this._valuesEn = new Dictionary<string, string>();
        this._valuesBr = new Dictionary<string, string>();
    }

    public void Initialize(IDbConnection dbClient)
    {
        this._valuesFr.Clear();
        this._valuesEn.Clear();
        this._valuesBr.Clear();

        var emulatorTextList = EmulatorTextDao.GetAll(dbClient);

        if (emulatorTextList.Count == 0)
        {
            return;
        }

        foreach (var emulatorText in emulatorTextList)
        {
            var key = emulatorText.Identifiant;

            this._valuesFr.Add(key, emulatorText.ValueFr);
            this._valuesEn.Add(key, emulatorText.ValueEn);
            this._valuesBr.Add(key, emulatorText.ValueBr);
        }
    }

    public string TryGetValue(string key, Language language)
    {
        if (language == Language.French)
        {
            return this._valuesFr.TryGetValue(key, out var value) ? value : "Pas de language locale trouver pour [" + key + "] (fr)";
        }
        else if (language == Language.English)
        {
            return this._valuesEn.TryGetValue(key, out var value) ? value : "Pas de language locale trouver pour [" + key + "] (en)";
        }
        else if (language == Language.Portuguese)
        {
            return this._valuesBr.TryGetValue(key, out var value) ? value : "Pas de language locale trouver pour [" + key + "] (br)";
        }
        else
        {
            return this._valuesFr.TryGetValue(key, out var value) ? value : "Pas de language locale trouver pour [" + key + "] (def)";
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
