namespace WibboEmulator.Communication.Packets.Incoming.Marketplace;
using WibboEmulator.Communication.Packets.Outgoing.MarketPlace;
using WibboEmulator.Database.Daos.Catalog;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;

internal sealed class CancelOfferEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session == null || session.User == null)
        {
            return;
        }

        var offerId = packet.PopInt();

        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();

        var row = CatalogMarketplaceOfferDao.GetByOfferId(dbClient, offerId);

        if (row == null)
        {
            session.SendPacket(new MarketplaceCancelOfferResultComposer(offerId, false));
            return;
        }

        if (Convert.ToString(row["state"]) == "2")
        {
            session.SendPacket(new MarketplaceCancelOfferResultComposer(offerId, false));
            return;
        }

        if (Convert.ToInt32(row["user_id"]) != session.User.Id)
        {
            session.SendPacket(new MarketplaceCancelOfferResultComposer(offerId, false));
            return;
        }

        if (!WibboEnvironment.GetGame().GetItemManager().GetItem(Convert.ToInt32(row["item_id"]), out var item))
        {
            session.SendPacket(new MarketplaceCancelOfferResultComposer(offerId, false));
            return;
        }

        CatalogMarketplaceOfferDao.DeleteUserOffer(dbClient, offerId, session.User.Id);

        var giveItem = ItemFactory.CreateSingleItem(dbClient, item, session.User, Convert.ToString(row["extra_data"]), Convert.ToInt32(row["furni_id"]), Convert.ToInt32(row["limited_number"]), Convert.ToInt32(row["limited_stack"]));

        if (giveItem != null)
        {
            session.User.InventoryComponent.TryAddItem(giveItem);
        }

        session.SendPacket(new MarketplaceCancelOfferResultComposer(offerId, true));
    }
}
