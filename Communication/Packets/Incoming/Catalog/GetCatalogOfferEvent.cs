using Butterfly.Communication.Packets.Outgoing.Catalog;
using Butterfly.Game.Catalog;
using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetCatalogOfferEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            int id = Packet.PopInt();
            CatalogItem Item = ButterflyEnvironment.GetGame().GetCatalog().FindItem(id, Session.GetUser().Rank);
            if (Item == null)
            {
                return;
            }

            if (!ButterflyEnvironment.GetGame().GetCatalog().TryGetPage(Item.PageID, out CatalogPage Page))
            {
                return;
            }

            if (!Page.Enabled || Page.MinimumRank > Session.GetUser().Rank)
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