namespace WibboEmulator.Core;
using System.Data;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;

public class SettingsManager
{
    private Dictionary<string, string> _settings = new();


    public void Init(IQueryAdapter dbClient)
    {
        this._settings.Clear();

        var table = EmulatorSettingDao.GetAll(dbClient);

        if (table == null)
        {
            return;
        }

        foreach (DataRow dataRow in table.Rows)
        {
            var key = (string)dataRow["key"];
            var value = (string)dataRow["value"];

            this._settings.Add(key, value);
        }
    }

    public bool GetDataBool(string key)
    {
        this._settings.TryGetValue(key, out var value);

        return value == "true";
    }

    public string GetDataString(string key)
    {
        this._settings.TryGetValue(key, out var value);

        return value;
    }

    public T GetData<T>(string key) where T : IConvertible
    {
        this._settings.TryGetValue(key, out var value);

        return (T)Convert.ChangeType(value, typeof(T));
    }
}