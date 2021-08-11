using Butterfly.HabboHotel.Achievements;

namespace Butterfly.Communication.Packets.Outgoing.Inventory.Achievements
{
    internal class AchievementProgressedComposer : ServerPacket
    {
        public AchievementProgressedComposer(Achievement Achievement, int TargetLevel, AchievementLevel TargetLevelData, int TotalLevels, UserAchievement UserData)
            : base(ServerPacketHeader.ACHIEVEMENT_PROGRESSED)
        {
            this.WriteInteger(Achievement.Id);
            this.WriteInteger(TargetLevel);
            this.WriteString(Achievement.GroupName + TargetLevel);
            this.WriteInteger(0);
            this.WriteInteger(TargetLevelData.Requirement);
            this.WriteInteger(TargetLevelData.RewardPixels);
            this.WriteInteger(0);
            this.WriteInteger(UserData != null ? UserData.Progress : 0);
            this.WriteBoolean(UserData != null && UserData.Level >= TotalLevels);
            this.WriteString(Achievement.Category);
            this.WriteString(string.Empty);
            this.WriteInteger(TotalLevels);
            this.WriteInteger(0);
        }
    }
}