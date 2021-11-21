using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Game.GameClients;
using Butterfly.Game.LandingView;

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

            if (!(currentView.Count() > 0))
            {
                return;
            }

            ServerPacket Message = currentView.SmallPromoComposer(new ServerPacket(ServerPacketHeader.DESKTOP_NEWS));            Session.SendPacket(Message);
        }
    }
}
