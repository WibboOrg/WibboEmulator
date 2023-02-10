namespace WibboEmulator.Core.Language;
using System.Data;
using WibboEmulator.Database.Daos.Emulator;
using WibboEmulator.Database.Interfaces;

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

    public void Init(IQueryAdapter dbClient)
    {
        this._valuesFr.Clear();
        this._valuesEn.Clear();
        this._valuesBr.Clear();

        var table = EmulatorTextDao.GetAll(dbClient);

        if (table == null)
        {
            return;
        }

        foreach (DataRow dataRow in table.Rows)
        {
            var key = (string)dataRow["identifiant"];
            var value_fr = (string)dataRow["value_fr"];
            var value_en = (string)dataRow["value_en"];
            var value_br = (string)dataRow["value_br"];

            this._valuesFr.Add(key, value_fr);
            this._valuesEn.Add(key, value_en);
            this._valuesBr.Add(key, value_br);
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
