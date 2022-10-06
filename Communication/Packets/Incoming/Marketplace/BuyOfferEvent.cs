namespace WibboEmulator.Communication.Packets.Incoming.Marketplace;
using System.Data;
using WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Communication.Packets.Outgoing.MarketPlace;
using WibboEmulator.Database.Daos.Catalog;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.Catalog.Marketplace;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;

internal class BuyOfferEvent : IPacketEvent
{
    public double Delay => 1000;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var OfferId = packet.PopInt();

        DataRow Row = null;
        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            Row = CatalogMarketplaceOfferDao.GetOneByOfferId(dbClient, OfferId);
        }

        if (Row == null)
        {
            ReloadOffers(session);
            return;
        }

        if (Convert.ToString(Row["state"]) == "2")
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.buyoffer.error.1", session.Langue));
            ReloadOffers(session);
            return;
        }

        if (WibboEnvironment.GetGame().GetCatalog().GetMarketplace().FormatTimestamp() > Convert.ToDouble(Row["timestamp"]))
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.buyoffer.error.2", session.Langue));
            ReloadOffers(session);
            return;
        }

        if (!WibboEnvironment.GetGame().GetItemManager().GetItem(Convert.ToInt32(Row["item_id"]), out var Item))
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.buyoffer.error.3", session.Langue));
            ReloadOffers(session);
            return;
        }
        else
        {
            if (Convert.ToInt32(Row["user_id"]) == session.GetUser().Id)
            {
                session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.buyoffer.error.4", session.Langue));
                return;
            }

            if (Convert.ToInt32(Row["total_price"]) > session.GetUser().WibboPoints)
            {
                session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.buyoffer.error.5", session.Langue));
                return;
            }

            session.GetUser().WibboPoints -= Convert.ToInt32(Row["total_price"]);
            session.SendPacket(new ActivityPointNotificationComposer(session.GetUser().WibboPoints, 0, 105));

            using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                UserDao.UpdateRemovePoints(dbClient, session.GetUser().Id, Convert.ToInt32(Row["total_price"]));
            }

            var GiveItem = ItemFactory.CreateSingleItem(Item, session.GetUser(), Convert.ToString(Row["extra_data"]), Convert.ToInt32(Row["furni_id"]), Convert.ToInt32(Row["limited_number"]), Convert.ToInt32(Row["limited_stack"]));
            if (GiveItem != null)
            {
                _ = session.GetUser().GetInventoryComponent().TryAddItem(GiveItem);
                session.SendPacket(new FurniListNotificationComposer(GiveItem.Id, 1));

                session.SendPacket(new PurchaseOKComposer());
            }


            using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                CatalogMarketplaceOfferDao.UpdateState(dbClient, OfferId);

                CatalogMarketplaceDataDao.Replace(dbClient, Item.SpriteId, Convert.ToInt32(Row["total_price"]));

                if (WibboEnvironment.GetGame().GetCatalog().GetMarketplace().MarketAverages.ContainsKey(Item.SpriteId) && WibboEnvironment.GetGame().GetCatalog().GetMarketplace().MarketCounts.ContainsKey(Item.SpriteId))
                {
                    var num3 = WibboEnvironment.GetGame().GetCatalog().GetMarketplace().MarketCounts[Item.SpriteId];
                    var num4 = WibboEnvironment.GetGame().GetCatalog().GetMarketplace().MarketAverages[Item.SpriteId] += Convert.ToInt32(Row["total_price"]);

                    _ = WibboEnvironment.GetGame().GetCatalog().GetMarketplace().MarketAverages.Remove(Item.SpriteId);
                    WibboEnvironment.GetGame().GetCatalog().GetMarketplace().MarketAverages.Add(Item.SpriteId, num4);
                    _ = WibboEnvironment.GetGame().GetCatalog().GetMarketplace().MarketCounts.Remove(Item.SpriteId);
                    WibboEnvironment.GetGame().GetCatalog().GetMarketplace().MarketCounts.Add(Item.SpriteId, num3 + 1);
                }
                else
                {
                    if (!WibboEnvironment.GetGame().GetCatalog().GetMarketplace().MarketAverages.ContainsKey(Item.SpriteId))
                    {
                        WibboEnvironment.GetGame().GetCatalog().GetMarketplace().MarketAverages.Add(Item.SpriteId, Convert.ToInt32(Row["total_price"]));
                    }

                    if (!WibboEnvironment.GetGame().GetCatalog().GetMarketplace().MarketCounts.ContainsKey(Item.SpriteId))
                    {
                        WibboEnvironment.GetGame().GetCatalog().GetMarketplace().MarketCounts.Add(Item.SpriteId, 1);
                    }
                }
            }
        }

        ReloadOffers(session);
    }

    private static void ReloadOffers(GameClient session)
    {
        var MinCost = -1;
        var MaxCost = -1;
        var SearchQuery = "";
        var FilterMode = 1;

        DataTable table = null;

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
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
                    var item = new MarketOffer(Convert.ToInt32(row["offer_id"]), Convert.ToInt32(row["sprite_id"]), Convert.ToInt32(row["total_price"]), Convert.ToInt32(row["item_type"].ToString()), Convert.ToInt32(row["limited_number"]), Convert.ToInt32(row["limited_stack"]));
                    WibboEnvironment.GetGame().GetCatalog().GetMarketplace().MarketItemKeys.Add(Convert.ToInt32(row["offer_id"]));
                    WibboEnvironment.GetGame().GetCatalog().GetMarketplace().MarketItems.Add(item);
                }
            }
        }

        var dictionary = new Dictionary<int, MarketOffer>();
        var dictionary2 = new Dictionary<int, int>();

        foreach (var item in WibboEnvironment.GetGame().GetCatalog().GetMarketplace().MarketItems)
        {
            if (dictionary.ContainsKey(item.SpriteId))
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

        session.SendPacket(new MarketPlaceOffersComposer(MinCost, MaxCost, dictionary, dictionary2));
    }
}
