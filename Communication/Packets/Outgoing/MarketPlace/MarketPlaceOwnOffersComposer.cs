namespace WibboEmulator.Communication.Packets.Outgoing.MarketPlace;
using System.Data;
using WibboEmulator.Database.Daos.Catalog;

internal sealed class MarketPlaceOwnOffersComposer : ServerPacket
{
    public MarketPlaceOwnOffersComposer(int userId)
       : base(ServerPacketHeader.MARKETPLACE_OWN_ITEMS)
    {
        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        var table = CatalogMarketplaceOfferDao.GetOneByUserId(dbClient, userId);

        var i = CatalogMarketplaceOfferDao.GetSunPrice(dbClient, userId);

        this.WriteInteger(i);
        if (table != null)
        {
            this.WriteInteger(table.Rows.Count);
            foreach (DataRow row in table.Rows)
            {
                var num2 = Convert.ToInt32(Math.Floor((double)((((int)row["timestamp"]) + 172800 - WibboEnvironment.GetUnixTimestamp()) / 60)));
                var num3 = Convert.ToInt32(row["state"].ToString());
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
