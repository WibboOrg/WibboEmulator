namespace WibboEmulator.Communication.Packets.Incoming.LandingView;
using WibboEmulator.Communication.Packets.Outgoing.LandingView;
using WibboEmulator.Games.GameClients;

internal sealed class GetPromoArticlesEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var currentView = WibboEnvironment.GetGame().GetHotelView();        if (session == null || session.User == null)
        {
            return;
        }

        if (!(currentView.Count() > 0))
        {
            return;
        }
        session.SendPacket(new PromoArticlesComposer(currentView.HotelViewPromosIndexers));
    }
}
