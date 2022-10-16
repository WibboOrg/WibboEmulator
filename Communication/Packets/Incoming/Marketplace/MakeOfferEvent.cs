namespace WibboEmulator.Communication.Packets.Incoming.Marketplace;
using WibboEmulator.Communication.Packets.Outgoing.MarketPlace;
using WibboEmulator.Database.Daos.Catalog;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Games.Catalog.Marketplace;
using WibboEmulator.Games.Catalog.Utilities;
using WibboEmulator.Games.GameClients;

internal class MakeOfferEvent : IPacketEvent
{
    public double Delay => 1000;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var sellingPrice = packet.PopInt();
        _ = packet.PopInt();
        var itemId = packet.PopInt();

        var item = session.User.InventoryComponent.GetItem(itemId);
        if (item == null)
        {
            session.SendPacket(new MarketplaceMakeOfferResultComposer(0));
            return;
        }

        if (!ItemUtility.IsRare(item))
        {
            return;
        }

        if (sellingPrice is > 999999 or <= 0)
        {
            session.SendPacket(new MarketplaceMakeOfferResultComposer(0));
            return;
        }

        var comission = MarketplaceManager.CalculateComissionPrice(sellingPrice);
        var totalPrice = sellingPrice + comission;
        var itemType = 1;
        if (item.GetBaseItem().Type == 'i')
        {
            itemType++;
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            ItemDao.Delete(dbClient, itemId);

            CatalogMarketplaceOfferDao.Insert(dbClient, item.GetBaseItem().ItemName, item.ExtraData, itemId, item.BaseItem, session.User.Id, sellingPrice, totalPrice, item.GetBaseItem().SpriteId, itemType, item.Limited, item.LimitedStack);

        }

        session.User.InventoryComponent.RemoveItem(itemId);
        session.SendPacket(new MarketplaceMakeOfferResultComposer(1));
    }
}
