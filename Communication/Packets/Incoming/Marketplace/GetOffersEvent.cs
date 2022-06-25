using WibboEmulator.Communication.Packets.Outgoing.MarketPlace;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Game.Catalog.Marketplace;
using WibboEmulator.Game.Clients;
using System.Data;

namespace WibboEmulator.Communication.Packets.Incoming.Marketplace
{
    internal class GetOffersEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            int MinCost = Packet.PopInt();
            int MaxCost = Packet.PopInt();
            string SearchQuery = Packet.PopString();
            int FilterMode = Packet.PopInt();

            DataTable table = null;

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                table = CatalogMarketplaceOfferDao.GetAll(dbClient, SearchQuery, MinCost, MaxCost, FilterMode);
            }

            WibboEnvironment.GetGame().GetCatalog().GetMarketplace().MarketItems.Clear();
            WibboEnvironment.GetGame().GetCatalog().GetMarketplace().MarketItemKeys.Clear();
            if (table != null)
            {
                foreach (DataRow row in table.Rows)
                {
                    if (!WibboEnvironment.GetGame().GetCatalog().GetMarketplace().MarketItemKeys.Contains(Convert.ToInt32(row["offer_id"])))
                    {
                        WibboEnvironment.GetGame().GetCatalog().GetMarketplace().MarketItemKeys.Add(Convert.ToInt32(row["offer_id"]));
                        WibboEnvironment.GetGame().GetCatalog().GetMarketplace().MarketItems.Add(new MarketOffer(Convert.ToInt32(row["offer_id"]), Convert.ToInt32(row["sprite_id"]), Convert.ToInt32(row["total_price"]), int.Parse(row["item_type"].ToString()), Convert.ToInt32(row["limited_number"]), Convert.ToInt32(row["limited_stack"])));
                    }
                }
            }

            Dictionary<int, MarketOffer> dictionary = new Dictionary<int, MarketOffer>();
            Dictionary<int, int> dictionary2 = new Dictionary<int, int>();

            foreach (MarketOffer item in WibboEnvironment.GetGame().GetCatalog().GetMarketplace().MarketItems)
            {
                if (dictionary.ContainsKey(item.SpriteId))
                {
                    if (item.LimitedNumber > 0)
                    {
                        if (!dictionary.ContainsKey(item.OfferID))
                        {
                            dictionary.Add(item.OfferID, item);
                        }

                        if (!dictionary2.ContainsKey(item.OfferID))
                        {
                            dictionary2.Add(item.OfferID, 1);
                        }
                    }
                    else
                    {
                        if (dictionary[item.SpriteId].TotalPrice > item.TotalPrice)
                        {
                            dictionary.Remove(item.SpriteId);
                            dictionary.Add(item.SpriteId, item);
                        }

                        int num = dictionary2[item.SpriteId];
                        dictionary2.Remove(item.SpriteId);
                        dictionary2.Add(item.SpriteId, num + 1);
                    }
                }
                else
                {
                    if (!dictionary.ContainsKey(item.SpriteId))
                    {
                        dictionary.Add(item.SpriteId, item);
                    }

                    if (!dictionary2.ContainsKey(item.SpriteId))
                    {
                        dictionary2.Add(item.SpriteId, 1);
                    }
                }
            }

            Session.SendPacket(new MarketPlaceOffersComposer(MinCost, MaxCost, dictionary, dictionary2));
        }
    }
}
