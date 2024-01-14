namespace WibboEmulator.Database.Daos.Emulator;
using System.Data;
using Dapper;

internal sealed class EmulatorAchievementDao
{
    internal static List<EmulatorAchievementEntity> GetAll(IDbConnection dbClient) => dbClient.Query<EmulatorAchievementEntity>(
        "SELECT id, category, group_name, level, reward_pixels, reward_points, progress_needed FROM `emulator_achievement`"
    ).ToList();
}

public class EmulatorAchievementEntity
{
    public int Id { get; set; }
    public string GroupName { get; set; }
    public string Category { get; set; }
    public int Level { get; set; }
    public int RewardPixels { get; set; }
    public int RewardPoints { get; set; }
    public int ProgressNeeded { get; set; }
}
