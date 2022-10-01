using WibboEmulator.Communication.Packets.Outgoing.MarketPlace;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Clients;

namespace WibboEmulator.Communication.Packets.Incoming.Marketplace
{
    internal class GetMarketplaceItemStatsEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            int ItemId = Packet.PopInt();
            int SpriteId = Packet.PopInt();

            int avgprice = 0;
            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                avgprice = CatalogMarketplaceDataDao.GetPriceBySprite(dbClient, SpriteId);

            Session.SendPacket(new MarketplaceItemStatsComposer(ItemId, SpriteId, avgprice));
        }
    }
}
