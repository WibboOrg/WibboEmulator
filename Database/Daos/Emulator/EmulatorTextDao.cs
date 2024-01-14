namespace WibboEmulator.Database.Daos.Emulator;
using System.Data;
using Dapper;

internal sealed class EmulatorTextDao
{
    internal static List<EmulatorTextEntity> GetAll(IDbConnection dbClient) => dbClient.Query<EmulatorTextEntity>(
        "SELECT `identifiant`, `value_fr`, `value_en`, `value_br` FROM `emulator_text`"
    ).ToList();
}

public class EmulatorTextEntity
{
    public int Id { get; set; }
    public string Identifiant { get; set; }
    public string ValueFr { get; set; }
    public string ValueEn { get; set; }
    public string ValueBr { get; set; }
}