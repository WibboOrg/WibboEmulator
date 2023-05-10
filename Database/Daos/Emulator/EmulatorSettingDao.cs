namespace WibboEmulator.Database.Daos.Emulator;
using System.Data;
using WibboEmulator.Database.Interfaces;

internal sealed class EmulatorSettingDao
{
    internal static DataTable GetAll(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT `key`, `value` FROM `emulator_setting`");
        return dbClient.GetTable();
    }

    internal static void Update(IQueryAdapter dbClient, string key, string value)
    {
        dbClient.SetQuery("UPDATE `emulator_setting` SET `value` = @value WHERE `key` = @key");
        dbClient.AddParameter("value", value);
        dbClient.AddParameter("key", key);
        dbClient.RunQuery();
    }
}