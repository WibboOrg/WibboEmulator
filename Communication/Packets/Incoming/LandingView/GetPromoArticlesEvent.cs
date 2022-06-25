using WibboEmulator.Communication.Packets.Outgoing.LandingView;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.LandingView;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class GetPromoArticlesEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            LandingViewManager currentView = WibboEnvironment.GetGame().GetHotelView();            if (Session == null || Session.GetUser() == null)
            {
                return;
            }

            if (!(currentView.Count() > 0))
            {
                return;
            }
            Session.SendPacket(new PromoArticlesComposer(currentView.HotelViewPromosIndexers));
        }
    }
}
