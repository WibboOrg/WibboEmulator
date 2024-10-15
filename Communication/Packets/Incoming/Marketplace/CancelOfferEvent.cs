namespace WibboEmulator.Communication.Packets.Incoming.Marketplace;
using WibboEmulator.Communication.Packets.Outgoing.MarketPlace;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Catalog;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;

internal sealed class CancelOfferEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session == null || Session.User == null)
        {
            return;
        }

        var offerId = packet.PopInt();

        using var dbClient = DatabaseManager.Connection;

        var offer = CatalogMarketplaceOfferDao.GetByOfferId(dbClient, offerId);

        if (offer == null)
        {
            Session.SendPacket(new MarketplaceCancelOfferResultComposer(offerId, false));
            return;
        }

        if (offer.State == 2)
        {
            Session.SendPacket(new MarketplaceCancelOfferResultComposer(offerId, false));
            return;
        }

        if (offer.UserId != Session.User.Id)
        {
            Session.SendPacket(new MarketplaceCancelOfferResultComposer(offerId, false));
            return;
        }

        if (!ItemManager.GetItem(offer.ItemId, out var item))
        {
            Session.SendPacket(new MarketplaceCancelOfferResultComposer(offerId, false));
            return;
        }

        CatalogMarketplaceOfferDao.DeleteUserOffer(dbClient, offerId, Session.User.Id);

        var giveItem = ItemFactory.CreateSingleItem(dbClient, item, Session.User, offer.ExtraData, offer.FurniId, offer.LimitedNumber, offer.LimitedStack);

        if (giveItem != null)
        {
            Session.User.InventoryComponent.TryAddItem(giveItem);
        }

        Session.SendPacket(new MarketplaceCancelOfferResultComposer(offerId, true));
    }
}
