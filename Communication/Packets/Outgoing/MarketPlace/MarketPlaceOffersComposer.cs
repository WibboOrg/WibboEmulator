using WibboEmulator.Game.Catalog.Marketplace;

namespace WibboEmulator.Communication.Packets.Outgoing.MarketPlace
{
    internal class MarketPlaceOffersComposer : ServerPacket
    {
        public MarketPlaceOffersComposer(int MinCost, int MaxCost, Dictionary<int, MarketOffer> dictionary, Dictionary<int, int> dictionary2)
            : base(ServerPacketHeader.MARKETPLACE_ITEMS_SEARCHED)
        {
            this.WriteInteger(dictionary.Count);
            if (dictionary.Count > 0)
            {
                foreach (KeyValuePair<int, MarketOffer> pair in dictionary)
                {
                    this.WriteInteger(pair.Value.OfferID);
                    this.WriteInteger(1);//State
                    this.WriteInteger(1);
                    this.WriteInteger(pair.Value.SpriteId);

                    this.WriteInteger(256);
                    this.WriteString("");
                    this.WriteInteger(pair.Value.LimitedNumber);
                    this.WriteInteger(pair.Value.LimitedStack);

                    this.WriteInteger(pair.Value.TotalPrice);
                    this.WriteInteger(0);
                    this.WriteInteger(WibboEnvironment.GetGame().GetCatalog().GetMarketplace().AvgPriceForSprite(pair.Value.SpriteId));
                    this.WriteInteger(dictionary2[pair.Value.SpriteId]);
                }
            }
            this.WriteInteger(dictionary.Count);//Item count to show how many were found.
        }
    }
}
