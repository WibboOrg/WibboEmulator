namespace WibboEmulator.Communication.Packets.Outgoing.Inventory.Achievements;
using WibboEmulator.Games.Achievements;
using WibboEmulator.Games.Users.Achievements;

internal sealed class AchievementProgressedComposer : ServerPacket
{
    public AchievementProgressedComposer(AchievementData achievement, int targetLevel, AchievementLevel targetLevelData, int totalLevels, UserAchievement userData)
        : base(ServerPacketHeader.ACHIEVEMENT_PROGRESSED)
    {
        this.WriteInteger(achievement.Id);
        this.WriteInteger(targetLevel);
        this.WriteString(achievement.GroupName + targetLevel);
        this.WriteInteger(0);
        this.WriteInteger(targetLevelData.Requirement);
        this.WriteInteger(targetLevelData.RewardPixels);
        this.WriteInteger(0);
        this.WriteInteger(userData != null ? userData.Progress : 0);
        this.WriteBoolean(userData != null && userData.Level >= totalLevels);
        this.WriteString(achievement.Category);
        this.WriteString(string.Empty);
        this.WriteInteger(totalLevels);
        this.WriteInteger(0);
    }
}
