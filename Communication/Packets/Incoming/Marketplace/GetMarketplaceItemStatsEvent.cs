namespace WibboEmulator.Communication.Packets.Incoming.Marketplace;
using WibboEmulator.Communication.Packets.Outgoing.MarketPlace;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Catalog;
using WibboEmulator.Games.GameClients;

internal sealed class GetMarketplaceItemStatsEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var itemId = packet.PopInt();
        var spriteId = packet.PopInt();

        var avgprice = 0;
        using (var dbClient = DatabaseManager.Connection)
        {
            avgprice = CatalogMarketplaceDataDao.GetPriceBySprite(dbClient, spriteId);
        }

        Session.SendPacket(new MarketplaceItemStatsComposer(itemId, spriteId, avgprice));
    }
}
