namespace WibboEmulator.Communication.Packets.Incoming.Marketplace;
using System.Data;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Database.Daos.Catalog;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;

internal class RedeemOfferCreditsEvent : IPacketEvent
{
    public double Delay => 1000;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        var CreditsOwed = 0;

        DataTable Table = null;
        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            Table = CatalogMarketplaceOfferDao.GetPriceByUserId(dbClient, session.GetUser().Id);
        }

        if (Table != null)
        {
            foreach (DataRow row in Table.Rows)
            {
                CreditsOwed += Convert.ToInt32(row["asking_price"]);
            }

            if (CreditsOwed >= 1)
            {
                session.GetUser().WibboPoints += CreditsOwed;
                session.SendPacket(new ActivityPointNotificationComposer(session.GetUser().WibboPoints, 0, 105));

                using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
                CatalogMarketplaceOfferDao.Delete(dbClient, session.GetUser().Id);
                UserDao.UpdateAddPoints(dbClient, session.GetUser().Id, CreditsOwed);
            }
        }
    }
}