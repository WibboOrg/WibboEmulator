namespace WibboEmulator.Communication.Packets.Incoming.Marketplace;
using System.Data;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Database.Daos.Catalog;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;

internal sealed class RedeemOfferCreditsEvent : IPacketEvent
{
    public double Delay => 500;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var creditsOwed = 0;

        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();

        var table = CatalogMarketplaceOfferDao.GetPriceByUserId(dbClient, session.User.Id);

        if (table != null)
        {
            foreach (DataRow row in table.Rows)
            {
                creditsOwed += Convert.ToInt32(row["asking_price"]);
            }

            if (creditsOwed >= 1)
            {
                session.User.WibboPoints += creditsOwed;
                session.SendPacket(new ActivityPointNotificationComposer(session.User.WibboPoints, 0, 105));

                CatalogMarketplaceOfferDao.Delete(dbClient, session.User.Id);
                UserDao.UpdateAddPoints(dbClient, session.User.Id, creditsOwed);
            }
        }
    }
}
