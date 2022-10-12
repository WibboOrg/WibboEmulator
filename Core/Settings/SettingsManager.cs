namespace WibboEmulator.Core.Settings;
using System.Data;
using WibboEmulator.Database.Daos.Emulator;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Utilities;

public class SettingsManager
{
    private readonly Dictionary<string, string> _settings = new();

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

    public T GetData<T>(string key) where T : IConvertible => this._settings.GetData<T>(key);
}
