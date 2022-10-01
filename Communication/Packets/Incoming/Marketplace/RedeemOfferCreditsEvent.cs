using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Clients;
using System.Data;

namespace WibboEmulator.Communication.Packets.Incoming.Marketplace
{
    internal class RedeemOfferCreditsEvent : IPacketEvent
    {
        public double Delay => 1000;

        public void Parse(Client Session, ClientPacket Packet)
        {
            int CreditsOwed = 0;

            DataTable Table = null;
            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                Table = CatalogMarketplaceOfferDao.GetPriceByUserId(dbClient, Session.GetUser().Id);

            if (Table != null)
            {
                foreach (DataRow row in Table.Rows)
                {
                    CreditsOwed += Convert.ToInt32(row["asking_price"]);
                }

                if (CreditsOwed >= 1)
                {
                    Session.GetUser().WibboPoints += CreditsOwed;
                    Session.SendPacket(new ActivityPointNotificationComposer(Session.GetUser().WibboPoints, 0, 105));

                    using IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
                    CatalogMarketplaceOfferDao.Delete(dbClient, Session.GetUser().Id);
                    UserDao.UpdateAddPoints(dbClient, Session.GetUser().Id, CreditsOwed);
                }
            }
        }
    }
}