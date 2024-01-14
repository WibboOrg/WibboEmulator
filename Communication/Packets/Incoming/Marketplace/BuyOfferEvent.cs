namespace WibboEmulator.Communication.Packets.Incoming.Marketplace;
using System.Data;
using WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Communication.Packets.Outgoing.MarketPlace;
using WibboEmulator.Database.Daos.Catalog;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.Catalogs.Marketplace;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;

internal sealed class BuyOfferEvent : IPacketEvent
{
    public double Delay => 1000;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var offerId = packet.PopInt();

        using var dbClient = WibboEnvironment.GetDatabaseManager().Connection();

        var offer = CatalogMarketplaceOfferDao.GetOneByOfferId(dbClient, offerId);

        if (offer == null)
        {
            ReloadOffers(session, dbClient);
            return;
        }

        if (offer.State == 2)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.buyoffer.error.1", session.Langue));
            ReloadOffers(session, dbClient);
            return;
        }

        if (MarketplaceManager.FormatTimestamp() > offer.Timestamp)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.buyoffer.error.2", session.Langue));
            ReloadOffers(session, dbClient);
            return;
        }

        if (!WibboEnvironment.GetGame().GetItemManager().GetItem(offer.ItemId, out var item))
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.buyoffer.error.3", session.Langue));
            ReloadOffers(session, dbClient);
            return;
        }
        else
        {
            if (offer.UserId == session.User.Id)
            {
                session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.buyoffer.error.4", session.Langue));
                return;
            }

            if (offer.TotalPrice > session.User.WibboPoints)
            {
                session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.buyoffer.error.5", session.Langue));
                return;
            }

            session.User.WibboPoints -= offer.TotalPrice;
            session.SendPacket(new ActivityPointNotificationComposer(session.User.WibboPoints, 0, 105));

            UserDao.UpdateRemovePoints(dbClient, session.User.Id, offer.TotalPrice);

            var giveItem = ItemFactory.CreateSingleItem(dbClient, item, session.User, offer.ExtraData, offer.FurniId, offer.LimitedNumber, offer.LimitedStack);
            if (giveItem != null)
            {
                session.User.InventoryComponent.TryAddItem(giveItem);

                session.SendPacket(new PurchaseOKComposer());
            }

            CatalogMarketplaceOfferDao.UpdateState(dbClient, offerId);
            CatalogMarketplaceDataDao.Replace(dbClient, item.SpriteId, offer.TotalPrice);

            if (WibboEnvironment.GetGame().GetCatalog().GetMarketplace().MarketAverages.TryGetValue(item.SpriteId, out var value) && WibboEnvironment.GetGame().GetCatalog().GetMarketplace().MarketCounts.ContainsKey(item.SpriteId))
            {
                var num3 = value;
                var num4 = WibboEnvironment.GetGame().GetCatalog().GetMarketplace().MarketAverages[item.SpriteId] += offer.TotalPrice;

                _ = WibboEnvironment.GetGame().GetCatalog().GetMarketplace().MarketAverages.Remove(item.SpriteId);
                WibboEnvironment.GetGame().GetCatalog().GetMarketplace().MarketAverages.Add(item.SpriteId, num4);
                _ = WibboEnvironment.GetGame().GetCatalog().GetMarketplace().MarketCounts.Remove(item.SpriteId);
                WibboEnvironment.GetGame().GetCatalog().GetMarketplace().MarketCounts.Add(item.SpriteId, num3 + 1);
            }
            else
            {
                if (!WibboEnvironment.GetGame().GetCatalog().GetMarketplace().MarketAverages.ContainsKey(item.SpriteId))
                {
                    WibboEnvironment.GetGame().GetCatalog().GetMarketplace().MarketAverages.Add(item.SpriteId, offer.TotalPrice);
                }

                if (!WibboEnvironment.GetGame().GetCatalog().GetMarketplace().MarketCounts.ContainsKey(item.SpriteId))
                {
                    WibboEnvironment.GetGame().GetCatalog().GetMarketplace().MarketCounts.Add(item.SpriteId, 1);
                }
            }
        }

        ReloadOffers(session, dbClient);
    }

    private static void ReloadOffers(GameClient session, IDbConnection dbClient)
    {
        var minCost = -1;
        var maxCost = -1;
        var searchQuery = "";
        var filterMode = 1;

        var offerList = CatalogMarketplaceOfferDao.GetAll(dbClient, searchQuery, minCost, maxCost, filterMode);

        WibboEnvironment.GetGame().GetCatalog().GetMarketplace().MarketItems.Clear();
        WibboEnvironment.GetGame().GetCatalog().GetMarketplace().MarketItemKeys.Clear();
        if (offerList.Count != 0)
        {
            foreach (var offer in offerList)
            {
                if (!WibboEnvironment.GetGame().GetCatalog().GetMarketplace().MarketItemKeys.Contains(offer.OfferId))
                {
                    var item = new MarketOffer(offer.OfferId, offer.SpriteId, offer.TotalPrice, offer.ItemType, offer.LimitedNumber, offer.LimitedStack);
                    WibboEnvironment.GetGame().GetCatalog().GetMarketplace().MarketItemKeys.Add(offer.OfferId);
                    WibboEnvironment.GetGame().GetCatalog().GetMarketplace().MarketItems.Add(item);
                }
            }
        }

        var dictionary = new Dictionary<int, MarketOffer>();
        var dictionary2 = new Dictionary<int, int>();

        foreach (var item in WibboEnvironment.GetGame().GetCatalog().GetMarketplace().MarketItems)
        {
            if (dictionary.TryGetValue(item.SpriteId, out var marketOffer))
            {
                if (dictionary[item.SpriteId].TotalPrice > item.TotalPrice)
                {
                    _ = dictionary.Remove(item.SpriteId);
                    dictionary.Add(item.SpriteId, item);
                }

                var num = dictionary2[item.SpriteId];
                _ = dictionary2.Remove(item.SpriteId);
                dictionary2.Add(item.SpriteId, num + 1);
            }
            else
            {
                dictionary.Add(item.SpriteId, item);
                dictionary2.Add(item.SpriteId, 1);
            }
        }

        session.SendPacket(new MarketPlaceOffersComposer(dictionary, dictionary2));
    }
}
