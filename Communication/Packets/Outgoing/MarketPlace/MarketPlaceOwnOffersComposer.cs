namespace WibboEmulator.Communication.Packets.Outgoing.MarketPlace;

using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Catalog;

internal sealed class MarketPlaceOwnOffersComposer : ServerPacket
{
    public MarketPlaceOwnOffersComposer(int userId)
       : base(ServerPacketHeader.MARKETPLACE_OWN_ITEMS)
    {
        using var dbClient = DatabaseManager.Connection;

        var offerList = CatalogMarketplaceOfferDao.GetAllByUserId(dbClient, userId);
        var sunPrice = CatalogMarketplaceOfferDao.GetSunPrice(dbClient, userId);

        this.WriteInteger(sunPrice);
        if (offerList.Count != 0)
        {
            this.WriteInteger(offerList.Count);
            foreach (var offer in offerList)
            {
                var num2 = Convert.ToInt32(Math.Floor((double)((offer.Timestamp + 172800 - WibboEnvironment.GetUnixTimestamp()) / 60)));
                var num3 = offer.State;
                if ((num2 <= 0) && (num3 != 2))
                {
                    num3 = 3;
                    num2 = 0;
                }
                this.WriteInteger(offer.OfferId);
                this.WriteInteger(num3);
                this.WriteInteger(1);
                this.WriteInteger(offer.SpriteId);

                this.WriteInteger(256);
                this.WriteString("");
                this.WriteInteger(offer.LimitedNumber);
                this.WriteInteger(offer.LimitedStack);

                this.WriteInteger(offer.TotalPrice);
                this.WriteInteger(num2);
                this.WriteInteger(offer.SpriteId);
            }
        }
        else
        {
            this.WriteInteger(0);
        }
    }
}
