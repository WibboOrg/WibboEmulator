using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using System.Data;
using MySqlX.XDevAPI.Common;

namespace WibboEmulator.Core;

public class SettingsManager
{
    private Dictionary<string, string> _settings = new();


    public void Init(IQueryAdapter dbClient)
    {
        this._settings.Clear();

        DataTable table = EmulatorSettingDao.GetAll(dbClient);

        if (table == null)
        {
            return;
        }

        foreach (DataRow dataRow in table.Rows)
        {
            string key = (string)dataRow["key"];
            string value = (string)dataRow["value"];

            this._settings.Add(key, value);
        }
    }

    public bool GetDataBool(string key)
    {
        this._settings.TryGetValue(key, out string value);

        return value == "true";
    }

    public string GetDataString(string key)
    {
        this._settings.TryGetValue(key, out string value);

        return value;
    }

    public T GetData<T>(string key) where T : IConvertible
    {
        this._settings.TryGetValue(key, out string value);

        return (T)Convert.ChangeType(value, typeof(T));
    }
}