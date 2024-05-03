namespace WibboEmulator.Games.Users.Achievements;

public class UserAchievement(string achievementGroup, int level, int progress)
{
    public string Group { get; set; } = achievementGroup;
    public int Level { get; set; } = level;
    public int Progress { get; set; } = progress;
}
