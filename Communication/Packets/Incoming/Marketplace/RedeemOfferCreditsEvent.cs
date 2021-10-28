using Butterfly.Communication.Packets.Outgoing.Inventory.Purse;
using Butterfly.Database.Interfaces;
using System;
using System.Data;

namespace Butterfly.Communication.Packets.Incoming.Marketplace
{
    internal class RedeemOfferCreditsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int CreditsOwed = 0;

            DataTable Table = null;
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT asking_price FROM catalog_marketplace_offers WHERE user_id = '" + Session.GetHabbo().Id + "' AND state = '2'");
                Table = dbClient.GetTable();
            }

            if (Table != null)
            {
                foreach (DataRow row in Table.Rows)
                {
                    CreditsOwed += Convert.ToInt32(row["asking_price"]);
                }

                if (CreditsOwed >= 1)
                {
                    Session.GetHabbo().WibboPoints += CreditsOwed;
                    Session.SendPacket(new HabboActivityPointNotificationComposer(Session.GetHabbo().WibboPoints, 0, 105));

                    using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("DELETE FROM catalog_marketplace_offers WHERE user_id = '" + Session.GetHabbo().Id + "' AND state = '2'");
                        dbClient.RunQuery("UPDATE users SET vip_points = vip_points + '" + CreditsOwed + "' WHERE id = '" + Session.GetHabbo().Id + "'");
                    }
                }
            }
        }
    }
}