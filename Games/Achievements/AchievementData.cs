namespace WibboEmulator.Games.Achievements;

public class AchievementData(int id, string groupName, string category)
{
    public int Id { get; } = id;
    public string GroupName { get; } = groupName;
    public string Category { get; } = category;
    public Dictionary<int, AchievementLevel> Levels { get; } = [];

    public void AddLevel(AchievementLevel level) => this.Levels.Add(level.Level, level);
}
