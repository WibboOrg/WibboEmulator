namespace WibboEmulator.Communication.Packets.Incoming.LandingView;
using WibboEmulator.Communication.Packets.Outgoing.LandingView;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.LandingView;

internal sealed class GetCommunityGoalHallOfFameEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet) => session.SendPacket(new CommunityGoalHallOfFameComposer(HallOfFameManager.UserRanking));
}
