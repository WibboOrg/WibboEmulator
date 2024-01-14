namespace WibboEmulator.Communication.Packets.Incoming.Marketplace;
using WibboEmulator.Communication.Packets.Outgoing.MarketPlace;
using WibboEmulator.Database.Daos.Catalog;
using WibboEmulator.Games.GameClients;

internal sealed class GetMarketplaceItemStatsEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var itemId = packet.PopInt();
        var spriteId = packet.PopInt();

        var avgprice = 0;
        using (var dbClient = WibboEnvironment.GetDatabaseManager().Connection())
        {
            avgprice = CatalogMarketplaceDataDao.GetPriceBySprite(dbClient, spriteId);
        }

        session.SendPacket(new MarketplaceItemStatsComposer(itemId, spriteId, avgprice));
    }
}
