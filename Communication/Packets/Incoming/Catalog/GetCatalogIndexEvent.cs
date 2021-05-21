using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetCatalogIndexEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Session.SendPacket(new CatalogIndexComposer(Session, ButterflyEnvironment.GetGame().GetCatalog().GetPages()));//, Sub));

            Session.SendPacket(new CatalogItemDiscountComposer());
            Session.SendPacket(new BCBorrowedItemsComposer());
        }
    }
}