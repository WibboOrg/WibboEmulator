namespace WibboEmulator.Communication.Packets.Incoming.Inventory.Achievements;

using WibboEmulator.Games.Achievements;
using WibboEmulator.Games.GameClients;

internal sealed class GetAchievementsEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet) => AchievementManager.GetList(session);
}
