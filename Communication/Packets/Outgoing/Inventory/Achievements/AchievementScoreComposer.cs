namespace WibboEmulator.Communication.Packets.Outgoing.Inventory.Achievements;

internal sealed class AchievementScoreComposer : ServerPacket
{
    public AchievementScoreComposer(int achScore)
        : base(ServerPacketHeader.USER_ACHIEVEMENT_SCORE) => this.WriteInteger(achScore);
}
