namespace WibboEmulator.Communication.Packets.Incoming.LandingView;
using WibboEmulator.Communication.Packets.Outgoing.LandingView;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.LandingView;

internal sealed class GetPromoArticlesEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {        if (session == null || session.User == null)
        {
            return;
        }

        if (!(LandingViewManager.Count > 0))
        {
            return;
        }
        session.SendPacket(new PromoArticlesComposer(LandingViewManager.HotelViewPromosIndexers));
    }
}
