namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class AchievementScoreComposer : ServerPacket
    {
        public AchievementScoreComposer(int achScore)
            : base(ServerPacketHeader.USER_ACHIEVEMENT_SCORE)
        {
            this.WriteInteger(achScore);
        }
    }
}
