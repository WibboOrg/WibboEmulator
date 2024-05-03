namespace WibboEmulator.Communication.Packets.Incoming.Marketplace;

using WibboEmulator.Communication.Packets.Outgoing.MarketPlace;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Catalog;
using WibboEmulator.Games.Catalogs.Marketplace;
using WibboEmulator.Games.GameClients;

internal sealed class GetOffersEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var minCost = packet.PopInt();
        var maxCost = packet.PopInt();
        var searchQuery = packet.PopString();
        var filterMode = packet.PopInt();

        using var dbClient = DatabaseManager.Connection;

        var offerList = CatalogMarketplaceOfferDao.GetAll(dbClient, searchQuery, minCost, maxCost, filterMode);

        MarketplaceManager.MarketItems.Clear();
        MarketplaceManager.MarketItemKeys.Clear();
        if (offerList.Count != 0)
        {
            foreach (var offer in offerList)
            {
                if (!MarketplaceManager.MarketItemKeys.Contains(offer.OfferId))
                {
                    MarketplaceManager.MarketItemKeys.Add(offer.OfferId);
                    MarketplaceManager.MarketItems.Add(new MarketOffer(offer.OfferId, offer.SpriteId, offer.TotalPrice, offer.ItemType, offer.LimitedNumber, offer.LimitedStack));
                }
            }
        }

        var dictionary = new Dictionary<int, MarketOffer>();
        var dictionary2 = new Dictionary<int, int>();

        foreach (var item in MarketplaceManager.MarketItems)
        {
            if (dictionary.TryGetValue(item.SpriteId, out var spriteOffer))
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
                    if (spriteOffer.TotalPrice > item.TotalPrice)
                    {
                        _ = dictionary.Remove(item.SpriteId);
                        dictionary.Add(item.SpriteId, item);
                    }

                    var num = dictionary2[item.SpriteId];
                    _ = dictionary2.Remove(item.SpriteId);
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

        session.SendPacket(new MarketPlaceOffersComposer(dictionary, dictionary2));
    }
}
