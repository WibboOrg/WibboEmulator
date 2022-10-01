namespace WibboEmulator.Games.Achievements
{
    public class AchievementData
    {
        public int Id { get; }
        public string GroupName { get; }
        public string Category { get; }
        public Dictionary<int, AchievementLevel> Levels { get; }

        public AchievementData(int id, string groupName, string category)
        {
            Id = id;
            GroupName = groupName;
            Category = category;
            Levels = new Dictionary<int, AchievementLevel>();
        }

        public void AddLevel(AchievementLevel Level)
        {
            this.Levels.Add(Level.Level, Level);
        }
    }
}
