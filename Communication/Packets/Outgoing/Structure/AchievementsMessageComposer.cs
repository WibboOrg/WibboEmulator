using Butterfly.HabboHotel.Achievements;
using Butterfly.HabboHotel.GameClients;
using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class AchievementsMessageComposer : ServerPacket
    {
        public AchievementsMessageComposer(GameClient Session, List<Achievement> Achievements)
            : base(ServerPacketHeader.ACHIEVEMENT_LIST)
        {
            this.WriteInteger(Achievements.Count);
            foreach (Achievement achievement in Achievements)
            {
                UserAchievement achievementData = Session.GetHabbo().GetAchievementData(achievement.GroupName);
                int TargetLevel = achievementData != null ? achievementData.Level + 1 : 1;
                int count = achievement.Levels.Count;
                if (TargetLevel > count)
                {
                    TargetLevel = count;
                }

                AchievementLevel achievementLevel = achievement.Levels[TargetLevel];
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
}
