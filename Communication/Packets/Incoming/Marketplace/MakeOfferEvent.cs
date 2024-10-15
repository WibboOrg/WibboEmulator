namespace WibboEmulator.Communication.Packets.Incoming.Marketplace;
using WibboEmulator.Communication.Packets.Outgoing.MarketPlace;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Catalog;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Games.Catalogs.Utilities;
using WibboEmulator.Games.GameClients;

internal sealed class MakeOfferEvent : IPacketEvent
{
    public double Delay => 1000;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var sellingPrice = packet.PopInt();
        _ = packet.PopInt();
        var itemId = packet.PopInt();

        var item = Session.User.InventoryComponent.GetItem(itemId);
        if (item == null)
        {
            Session.SendPacket(new MarketplaceMakeOfferResultComposer(0));
            return;
        }

        if (!ItemUtility.IsRare(item) || !item.ItemData.AllowMarketplaceSell)
        {
            return;
        }

        if (sellingPrice is > 99999 or <= 0)
        {
            Session.SendPacket(new MarketplaceMakeOfferResultComposer(0));
            return;
        }

        var totalPrice = sellingPrice;
        var itemType = 1;
        if (item.ItemData.Type == ItemType.I)
        {
            itemType++;
        }

        using (var dbClient = DatabaseManager.Connection)
        {
            ItemDao.Delete(dbClient, itemId);

            CatalogMarketplaceOfferDao.Insert(dbClient, item.ItemData.ItemName, item.ExtraData, itemId, item.BaseItemId, Session.User.Id, sellingPrice, totalPrice, item.ItemData.SpriteId, itemType, item.Limited, item.LimitedStack);
        }

        Session.User.InventoryComponent.RemoveItem(itemId);
        Session.SendPacket(new MarketplaceMakeOfferResultComposer(1));
    }
}
