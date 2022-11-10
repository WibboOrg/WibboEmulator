namespace WibboEmulator.Games.Achievements;

public readonly struct AchievementLevel
{
    public readonly int Level { get; }
    public readonly int RewardPixels { get; }
    public readonly int RewardPoints { get; }
    public readonly int Requirement { get; }

    public AchievementLevel(int level, int rewardPixels, int rewardPoints, int requirement)
    {
        this.Level = level;
        this.RewardPixels = rewardPixels;
        this.RewardPoints = rewardPoints;
        this.Requirement = requirement;
    }
}
