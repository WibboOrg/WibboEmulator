using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using System.Data;

namespace WibboEmulator.Communication.Packets.Outgoing.MarketPlace
{
    internal class MarketPlaceOwnOffersComposer : ServerPacket
    {
        public MarketPlaceOwnOffersComposer(int UserId)
           : base(ServerPacketHeader.MARKETPLACE_OWN_ITEMS)
        {
            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DataTable table = CatalogMarketplaceOfferDao.GetOneByUserId(dbClient, UserId);

                int i = CatalogMarketplaceOfferDao.GetSunPrice(dbClient, UserId);

                this.WriteInteger(i);
                if (table != null)
                {
                    this.WriteInteger(table.Rows.Count);
                    foreach (DataRow row in table.Rows)
                    {
                        int num2 = Convert.ToInt32(Math.Floor((double)(((((double)row["timestamp"]) + 172800.0) - WibboEnvironment.GetUnixTimestamp()) / 60.0)));
                        int num3 = int.Parse(row["state"].ToString());
                        if ((num2 <= 0) && (num3 != 2))
                        {
                            num3 = 3;
                            num2 = 0;
                        }
                        this.WriteInteger(Convert.ToInt32(row["offer_id"]));
                        this.WriteInteger(num3);
                        this.WriteInteger(1);
                        this.WriteInteger(Convert.ToInt32(row["sprite_id"]));

                        this.WriteInteger(256);
                        this.WriteString("");
                        this.WriteInteger(Convert.ToInt32(row["limited_number"]));
                        this.WriteInteger(Convert.ToInt32(row["limited_stack"]));

                        this.WriteInteger(Convert.ToInt32(row["total_price"]));
                        this.WriteInteger(num2);
                        this.WriteInteger(Convert.ToInt32(row["sprite_id"]));
                    }
                }
                else
                {
                    this.WriteInteger(0);
                }
            }
        }
    }
}