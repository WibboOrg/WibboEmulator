namespace WibboEmulator.Core.Settings;
using System.Data;
using WibboEmulator.Database.Daos.Emulator;
using WibboEmulator.Utilities;

public class SettingsManager
{
    private readonly Dictionary<string, string> _settings = new();

    public void Init(IDbConnection dbClient)
    {
        this._settings.Clear();

        var emulatorSettingsList = EmulatorSettingDao.GetAll(dbClient);

        if (emulatorSettingsList.Count == 0)
        {
            return;
        }

        foreach (var emulatorSettings in emulatorSettingsList)
        {
            this._settings.Add(emulatorSettings.Key, emulatorSettings.Value);
        }
    }

    public T GetData<T>(string key) where T : IConvertible => this._settings.GetData<T>(key);
}
