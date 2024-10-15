namespace WibboEmulator.Communication.Packets.Incoming.Marketplace;
using WibboEmulator.Communication.Packets.Outgoing.MarketPlace;
using WibboEmulator.Database;
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

        using var dbClient = DatabaseManager.Connection;

        var offer = CatalogMarketplaceOfferDao.GetByOfferId(dbClient, offerId);

        if (offer == null)
        {
            session.SendPacket(new MarketplaceCancelOfferResultComposer(offerId, false));
            return;
        }

        if (offer.State == 2)
        {
            session.SendPacket(new MarketplaceCancelOfferResultComposer(offerId, false));
            return;
        }

        if (offer.UserId != session.User.Id)
        {
            session.SendPacket(new MarketplaceCancelOfferResultComposer(offerId, false));
            return;
        }

        if (!ItemManager.GetItem(offer.ItemId, out var item))
        {
            session.SendPacket(new MarketplaceCancelOfferResultComposer(offerId, false));
            return;
        }

        CatalogMarketplaceOfferDao.DeleteUserOffer(dbClient, offerId, session.User.Id);

        var giveItem = ItemFactory.CreateSingleItem(dbClient, item, session.User, offer.ExtraData, offer.FurniId, offer.LimitedNumber, offer.LimitedStack);

        if (giveItem != null)
        {
            session.User.InventoryComponent.TryAddItem(giveItem);
        }

        session.SendPacket(new MarketplaceCancelOfferResultComposer(offerId, true));
    }
}
