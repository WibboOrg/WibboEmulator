using Butterfly.Communication.Packets.Outgoing.MarketPlace;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;

namespace Butterfly.Communication.Packets.Incoming.Marketplace
{
    internal class GetMarketplaceItemStatsEvent : IPacketEvent
    {
        public void Parse(Game.GameClients.GameClient Session, ClientPacket Packet)
        {
            int ItemId = Packet.PopInt();
            int SpriteId = Packet.PopInt();

            int avgprice = 0;
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                avgprice = CatalogMarketplaceDataDao.GetPriceBySprite(dbClient, SpriteId);

            Session.SendPacket(new MarketplaceItemStatsComposer(ItemId, SpriteId, avgprice));
        }
    }
}
