namespace WibboEmulator.Communication.Packets.Incoming.Marketplace;

using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Catalog;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;

internal sealed class RedeemOfferCreditsEvent : IPacketEvent
{
    public double Delay => 500;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        using var dbClient = DatabaseManager.Connection;

        var creditsOwed = CatalogMarketplaceOfferDao.GetSunPrice(dbClient, Session.User.Id);

        if (creditsOwed >= 1)
        {
            CatalogMarketplaceOfferDao.Delete(dbClient, Session.User.Id);
            UserDao.UpdateAddPoints(dbClient, Session.User.Id, creditsOwed);

            Session.User.WibboPoints += creditsOwed;
            Session.SendPacket(new ActivityPointNotificationComposer(Session.User.WibboPoints, 0, 105));
        }
    }
}
