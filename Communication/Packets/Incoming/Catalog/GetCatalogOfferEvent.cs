using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.HabboHotel.Catalog;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetCatalogOfferEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int id = Packet.PopInt();
            CatalogItem Item = ButterflyEnvironment.GetGame().GetCatalog().FindItem(id, Session.GetHabbo().Rank);
            if (Item == null)
            {
                return;
            }

            if (!ButterflyEnvironment.GetGame().GetCatalog().TryGetPage(Item.PageID, out CatalogPage Page))
            {
                return;
            }

            if (!Page.Enabled || Page.MinimumRank > Session.GetHabbo().Rank)
            {
                return;
            }

            if (Item.IsLimited)
            {
                return;
            }

            Session.SendPacket(new CatalogOfferComposer(Item));
        }
    }
}