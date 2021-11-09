using Butterfly.Communication.Packets.Outgoing;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.LandingView;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetPromoArticlesEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            LandingViewManager currentView = ButterflyEnvironment.GetGame().GetHotelView();            if (Session == null || Session.GetHabbo() == null)
            {
                return;
            }

            if (!(currentView.HotelViewPromosIndexers.Count > 0))
            {
                return;
            }

            ServerPacket Message = currentView.SmallPromoComposer(new ServerPacket(ServerPacketHeader.DESKTOP_NEWS));            Session.SendPacket(Message);
        }
    }
}
