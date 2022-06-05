using Wibbo.Communication.Packets.Outgoing.Catalog;
using Wibbo.Game.Catalog;
using Wibbo.Game.Clients;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class GetCatalogPageEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            int PageId = Packet.PopInt();
            int Something = Packet.PopInt();
            string CataMode = Packet.PopString();

            WibboEnvironment.GetGame().GetCatalog().TryGetPage(PageId, out CatalogPage Page);
            if (Page == null || Page.MinimumRank > Session.GetUser().Rank)
            {
                return;
            }

            Session.SendPacket(new CatalogPageComposer(Page, CataMode, Session.Langue));
        }
    }
}