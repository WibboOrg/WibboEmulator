namespace WibboEmulator.Communication.Packets.Incoming.LandingView;
using WibboEmulator.Communication.Packets.Outgoing.LandingView;
using WibboEmulator.Games.GameClients;

internal sealed class GetCommunityGoalHallOfFameEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var hof = WibboEnvironment.GetGame().GetHallOFFame();

        session.SendPacket(new CommunityGoalHallOfFameComposer(hof.UserRanking));
    }
}
