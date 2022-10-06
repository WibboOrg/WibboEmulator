namespace WibboEmulator.Communication.Packets.Incoming.Marketplace;
using WibboEmulator.Communication.Packets.Outgoing.MarketPlace;
using WibboEmulator.Database.Daos.Catalog;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Games.Catalog.Utilities;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;

internal class MakeOfferEvent : IPacketEvent
{
    public double Delay => 1000;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        var SellingPrice = Packet.PopInt();
        var ComissionPrice = Packet.PopInt();
        var ItemId = Packet.PopInt();

        var Item = session.GetUser().GetInventoryComponent().GetItem(ItemId);
        if (Item == null)
        {
            session.SendPacket(new MarketplaceMakeOfferResultComposer(0));
            return;
        }

        if (!ItemUtility.IsRare(Item))
        {
            return;
        }

        if (SellingPrice is > 999999 or <= 0)
        {
            session.SendPacket(new MarketplaceMakeOfferResultComposer(0));
            return;
        }

        var Comission = WibboEnvironment.GetGame().GetCatalog().GetMarketplace().CalculateComissionPrice(SellingPrice);
        var TotalPrice = SellingPrice + Comission;
        var ItemType = 1;
        if (Item.GetBaseItem().Type == 'i')
        {
            ItemType++;
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            ItemDao.Delete(dbClient, ItemId);

            CatalogMarketplaceOfferDao.Insert(dbClient, Item.GetBaseItem().ItemName, Item.ExtraData, ItemId, Item.BaseItem, session.GetUser().Id, SellingPrice, TotalPrice, Item.GetBaseItem().SpriteId, ItemType, Item.Limited, Item.LimitedStack);

        }

        session.GetUser().GetInventoryComponent().RemoveItem(ItemId);
        session.SendPacket(new MarketplaceMakeOfferResultComposer(1));
    }
}