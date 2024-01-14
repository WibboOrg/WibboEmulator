namespace WibboEmulator.Database.Daos.Emulator;
using System.Data;
using Dapper;

internal sealed class EmulatorCommandDao
{
    internal static List<EmulatorCommandEntity> GetAll(IDbConnection dbClient) => dbClient.Query<EmulatorCommandEntity>(
        "SELECT `id`, `input`, `minrank`, `description_fr`, `description_en`, `description_br` FROM `emulator_command`"
    ).ToList();
}

public class EmulatorCommandEntity
{
    public int Id { get; set; }
    public string Input { get; set; }
    public int MinRank { get; set; }
    public string DescriptionFr { get; set; }
    public string DescriptionEn { get; set; }
    public string DescriptionBr { get; set; }
}
