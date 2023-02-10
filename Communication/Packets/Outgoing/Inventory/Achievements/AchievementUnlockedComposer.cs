namespace WibboEmulator.Communication.Packets.Outgoing.Inventory.Achievements;
using WibboEmulator.Games.Achievements;

internal sealed class AchievementUnlockedComposer : ServerPacket
{
    public AchievementUnlockedComposer(AchievementData achievement, int level, int pointReward, int pixelReward)
        : base(ServerPacketHeader.ACHIEVEMENT_NOTIFICATION)
    {
        this.WriteInteger(achievement.Id);
        this.WriteInteger(level);
        this.WriteInteger(144);
        this.WriteString(achievement.GroupName + level);
        this.WriteInteger(pointReward);
        this.WriteInteger(pixelReward);
        this.WriteInteger(0);
        this.WriteInteger(10);
        this.WriteInteger(21);
        this.WriteString(level > 1 ? achievement.GroupName + (level - 1) : string.Empty);
        this.WriteString(achievement.Category);
        this.WriteString(string.Empty);
    }
}
