namespace WibboEmulator.Database.Daos.Emulator;
using System.Data;
using Dapper;

internal sealed class EmulatorQuestDao
{
    internal static List<EmulatorQuestEntity> GetAll(IDbConnection dbClient) => dbClient.Query<EmulatorQuestEntity>(
        "SELECT `id`, `category`, `series_number`, `goal_type`, `goal_data`, `name`, `reward`, `data_bit` FROM `emulator_quest`"
    ).ToList();
}

public class EmulatorQuestEntity
{
    public int Id { get; set; }
    public string Category { get; set; }
    public int SeriesNumber { get; set; }
    public int GoalType { get; set; }
    public int GoalData { get; set; }
    public string Name { get; set; }
    public int Reward { get; set; }
    public string DataBit { get; set; }
}
