namespace WibboEmulator.Communication.Packets.Outgoing.Inventory.Achievements;
using WibboEmulator.Games.Achievements;
using WibboEmulator.Games.GameClients;

internal class AchievementsComposer : ServerPacket
{
    public AchievementsComposer(GameClient session, List<AchievementData> Achievements)
        : base(ServerPacketHeader.ACHIEVEMENT_LIST)
    {
        this.WriteInteger(Achievements.Count);
        foreach (var achievement in Achievements)
        {
            var achievementData = session.GetUser().GetAchievementComponent().GetAchievementData(achievement.GroupName);
            var TargetLevel = achievementData != null ? achievementData.Level + 1 : 1;
            var count = achievement.Levels.Count;
            if (TargetLevel > count)
            {
                TargetLevel = count;
            }

            var achievementLevel = achievement.Levels[TargetLevel];
            this.WriteInteger(achievement.Id);
            this.WriteInteger(TargetLevel);
            this.WriteString(achievement.GroupName + TargetLevel);
            this.WriteInteger(0);
            this.WriteInteger(achievementLevel.Requirement); //?
            this.WriteInteger(achievementLevel.RewardPixels);
            this.WriteInteger(0); //-1 = rien, 5 = PointWinwin?
            this.WriteInteger(achievementData != null ? achievementData.Progress : 0);
            this.WriteBoolean(achievementData != null && achievementData.Level >= count);
            this.WriteString(achievement.Category);
            this.WriteString(string.Empty);
            this.WriteInteger(count);
            this.WriteInteger(0);
        }
        this.WriteString(string.Empty);
    }
}
