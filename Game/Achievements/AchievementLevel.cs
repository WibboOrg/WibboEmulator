namespace WibboEmulator.Game.Achievements
{
    public struct AchievementLevel
    {
        public readonly int Level { get; }
        public readonly int RewardPixels { get; }
        public readonly int RewardPoints { get; }
        public readonly int Requirement { get; }

        public AchievementLevel(int level, int rewardPixels, int rewardPoints, int requirement)
        {
            Level = level;
            RewardPixels = rewardPixels;
            RewardPoints = rewardPoints;
            Requirement = requirement;
        }
    }
}
