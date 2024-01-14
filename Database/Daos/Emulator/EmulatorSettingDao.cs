namespace WibboEmulator.Database.Daos.Emulator;
using System.Data;
using Dapper;

internal sealed class EmulatorSettingDao
{
    internal static List<EmulatorSettingEntity> GetAll(IDbConnection dbClient) => dbClient.Query<EmulatorSettingEntity>(
        "SELECT `key`, `value` FROM `emulator_setting`"
    ).ToList();

    internal static void Update(IDbConnection dbConnection, string key, string value) => dbConnection.Execute(
        "UPDATE emulator_setting SET value = @Value WHERE key = @Key",
        new { Value = value, Key = key });

}

public class EmulatorSettingEntity
{
    public int Id { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }
}