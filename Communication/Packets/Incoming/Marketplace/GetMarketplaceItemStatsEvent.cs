namespace WibboEmulator.Communication.Packets.Incoming.Marketplace;
using WibboEmulator.Communication.Packets.Outgoing.MarketPlace;
using WibboEmulator.Database.Daos.Catalog;
using WibboEmulator.Games.GameClients;

internal class GetMarketplaceItemStatsEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var ItemId = packet.PopInt();
        var SpriteId = packet.PopInt();

        var avgprice = 0;
        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            avgprice = CatalogMarketplaceDataDao.GetPriceBySprite(dbClient, SpriteId);
        }

        session.SendPacket(new MarketplaceItemStatsComposer(ItemId, SpriteId, avgprice));
    }
}
