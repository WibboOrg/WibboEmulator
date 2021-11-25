using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.LandingView;
using Butterfly.Game.Clients;
using Butterfly.Game.LandingView;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetPromoArticlesEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            LandingViewManager currentView = ButterflyEnvironment.GetGame().GetHotelView();            if (Session == null || Session.GetHabbo() == null)
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
