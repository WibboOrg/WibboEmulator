namespace WibboEmulator.Communication.Packets.Incoming.LandingView;
using WibboEmulator.Communication.Packets.Outgoing.LandingView;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.LandingView;

internal sealed class GetPromoArticlesEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {Session.
        if (Session == null || Session.User == null)
        {
            return;
        }

        if (!(LandingViewManager.Count > 0))
        {
            return;
        }Session.

        Session.SendPacket(new PromoArticlesComposer(LandingViewManager.HotelViewPromosIndexers));
    }
}
