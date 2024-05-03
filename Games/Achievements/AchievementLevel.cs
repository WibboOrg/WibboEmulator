namespace WibboEmulator.Games.Achievements;

public readonly struct AchievementLevel(int level, int rewardPixels, int rewardPoints, int requirement)
{
    public readonly int Level { get; } = level;
    public readonly int RewardPixels { get; } = rewardPixels;
    public readonly int RewardPoints { get; } = rewardPoints;
    public readonly int Requirement { get; } = requirement;
}
