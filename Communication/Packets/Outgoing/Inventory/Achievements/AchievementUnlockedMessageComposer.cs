using Butterfly.Game.Achievement;

namespace Butterfly.Communication.Packets.Outgoing.Inventory.Achievements
{
    internal class AchievementUnlockedMessageComposer : ServerPacket
    {
        public AchievementUnlockedMessageComposer(AchievementData Achievement, int Level, int PointReward, int PixelReward)
            : base(ServerPacketHeader.AchievementUnlockedMessageComposer)
        {
            this.WriteInteger(Achievement.Id);
            this.WriteInteger(Level);
            this.WriteInteger(144);
            this.WriteString(Achievement.GroupName + Level);
            this.WriteInteger(PointReward);
            this.WriteInteger(PixelReward);
            this.WriteInteger(0);
            this.WriteInteger(10);
            this.WriteInteger(21);
            this.WriteString(Level > 1 ? Achievement.GroupName + (Level - 1) : string.Empty);
            this.WriteString(Achievement.Category);
            this.WriteString(string.Empty);
        }
    }
}
