namespace WibboEmulator.Communication.Packets.Outgoing.Inventory.Achievements;
using WibboEmulator.Games.Achievements;
using WibboEmulator.Games.GameClients;

internal sealed class AchievementsComposer : ServerPacket
{
    public AchievementsComposer(GameClient session, List<AchievementData> achievements)
        : base(ServerPacketHeader.ACHIEVEMENT_LIST)
    {
        this.WriteInteger(achievements.Count);
        foreach (var achievement in achievements)
        {
            var achievementData = session.User.AchievementComponent.GetAchievementData(achievement.GroupName);
            var targetLevel = achievementData != null ? achievementData.Level + 1 : 1;
            var count = achievement.Levels.Count;
            if (targetLevel > count)
            {
                targetLevel = count;
            }

            var achievementLevel = achievement.Levels[targetLevel];
            this.WriteInteger(achievement.Id);
            this.WriteInteger(targetLevel);
            this.WriteString(achievement.GroupName + targetLevel);
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
