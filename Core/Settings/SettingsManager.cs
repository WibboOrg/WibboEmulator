namespace WibboEmulator.Core.Settings;
using System.Data;
using WibboEmulator.Database.Daos.Emulator;
using WibboEmulator.Utilities;

public static class SettingsManager
{
    private static readonly Dictionary<string, string> Settings = [];

    public static void Initialize(IDbConnection dbClient)
    {
        Settings.Clear();

        var emulatorSettingsList = EmulatorSettingDao.GetAll(dbClient);

        foreach (var emulatorSettings in emulatorSettingsList)
        {
            Settings.Add(emulatorSettings.Key, emulatorSettings.Value);
        }
    }

    public static T GetData<T>(string key) where T : IConvertible => Settings.GetData<T>(key);
}
