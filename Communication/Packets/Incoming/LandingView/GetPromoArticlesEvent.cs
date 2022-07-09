using Wibbo.Communication.Packets.Outgoing.LandingView;
using Wibbo.Game.Clients;
using Wibbo.Game.LandingView;

namespace Wibbo.Communication.Packets.Incoming.Structure
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
