namespace WibboEmulator.Communication.Packets.Incoming.Marketplace;
using System.Data;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Communication.Packets.Outgoing.MarketPlace;
using WibboEmulator.Database.Daos.Catalog;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;

internal class CancelOfferEvent : IPacketEvent
{
    public double Delay => 1000;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (session == null || session.GetUser() == null)
        {
            return;
        }

        DataRow Row = null;
        var OfferId = Packet.PopInt();

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            Row = CatalogMarketplaceOfferDao.GetByOfferId(dbClient, OfferId);
        }

        if (Row == null)
        {
            session.SendPacket(new MarketplaceCancelOfferResultComposer(OfferId, false));
            return;
        }

        if (Convert.ToString(Row["state"]) == "2")
        {
            session.SendPacket(new MarketplaceCancelOfferResultComposer(OfferId, false));
            return;
        }

        if (Convert.ToInt32(Row["user_id"]) != session.GetUser().Id)
        {
            session.SendPacket(new MarketplaceCancelOfferResultComposer(OfferId, false));
            return;
        }

        if (!WibboEnvironment.GetGame().GetItemManager().GetItem(Convert.ToInt32(Row["item_id"]), out var Item))
        {
            session.SendPacket(new MarketplaceCancelOfferResultComposer(OfferId, false));
            return;
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            CatalogMarketplaceOfferDao.DeleteUserOffer(dbClient, OfferId, session.GetUser().Id);
        }

        var GiveItem = ItemFactory.CreateSingleItem(Item, session.GetUser(), Convert.ToString(Row["extra_data"]), Convert.ToInt32(Row["furni_id"]), Convert.ToInt32(Row["limited_number"]), Convert.ToInt32(Row["limited_stack"]));

        if (GiveItem != null)
        {
            session.GetUser().GetInventoryComponent().TryAddItem(GiveItem);
            session.SendPacket(new FurniListNotificationComposer(GiveItem.Id, 1));
        }

        session.SendPacket(new MarketplaceCancelOfferResultComposer(OfferId, true));
    }
}
