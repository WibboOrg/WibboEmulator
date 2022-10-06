namespace WibboEmulator.Games.Achievements;

public class AchievementData
{
    public int Id { get; }
    public string GroupName { get; }
    public string Category { get; }
    public Dictionary<int, AchievementLevel> Levels { get; }

    public AchievementData(int id, string groupName, string category)
    {
        this.Id = id;
        this.GroupName = groupName;
        this.Category = category;
        this.Levels = new Dictionary<int, AchievementLevel>();
    }

    public void AddLevel(AchievementLevel level) => this.Levels.Add(level.Level, level);
}
