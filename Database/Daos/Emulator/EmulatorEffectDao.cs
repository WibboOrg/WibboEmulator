namespace WibboEmulator.Database.Daos.Emulator;
using System.Data;
using Dapper;

internal sealed class EmulatorEffectDao
{
    internal static List<EmulatorEffectEntity> GetAll(IDbConnection dbClient) => dbClient.Query<EmulatorEffectEntity>(
        "SELECT `id`, `only_staff` FROM `emulator_effect` ORDER BY `id` ASC"
    ).ToList();
}

public class EmulatorEffectEntity
{
    public int Id { get; set; }
    public bool OnlyStaff { get; set; }
}
