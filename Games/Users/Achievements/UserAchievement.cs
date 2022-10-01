namespace WibboEmulator.Game.Users.Achievements
{
    public class UserAchievement
    {
        public string Group { get; set; }
        public int Level { get; set; }
        public int Progress { get; set; }

        public UserAchievement(string achievementGroup, int level, int progress)
        {
            this.Group = achievementGroup;
            this.Level = level;
            this.Progress = progress;
        }
    }
}
