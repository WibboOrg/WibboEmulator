using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.HabboHotel.Catalog;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetCatalogPageEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int PageId = Packet.PopInt();
            int Something = Packet.PopInt();
            string CataMode = Packet.PopString();

            ButterflyEnvironment.GetGame().GetCatalog().TryGetPage(PageId, out CatalogPage Page);
            if (Page == null || Page.MinimumRank > Session.GetHabbo().Rank)
            {
                return;
            }

            Session.SendPacket(new CatalogPageComposer(Page, CataMode, Session.Langue));
        }
    }
}