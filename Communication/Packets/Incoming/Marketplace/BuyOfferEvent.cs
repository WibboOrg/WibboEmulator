using Butterfly.Communication.Packets.Outgoing.Catalog;
using Butterfly.Communication.Packets.Outgoing.Inventory.Furni;
using Butterfly.Communication.Packets.Outgoing.Inventory.Purse;
using Butterfly.Communication.Packets.Outgoing.MarketPlace;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Catalog.Marketplace;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using System;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.Communication.Packets.Incoming.Marketplace
{
    internal class BuyOfferEvent : IPacketEvent
    {
        public double Delay => 1000;

        public void Parse(Client Session, ClientPacket Packet)
        {
            int OfferId = Packet.PopInt();

            DataRow Row = null;
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                Row = CatalogMarketplaceOfferDao.GetOneByOfferId(dbClient, OfferId);

            if (Row == null)
            {
                this.ReloadOffers(Session);
                return;
            }

            if (Convert.ToString(Row["state"]) == "2")
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.buyoffer.error.1", Session.Langue));
                this.ReloadOffers(Session);
                return;
            }

            if (ButterflyEnvironment.GetGame().GetCatalog().GetMarketplace().FormatTimestamp() > (Convert.ToDouble(Row["timestamp"])))
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.buyoffer.error.2", Session.Langue));
                this.ReloadOffers(Session);
                return;
            }

            if (!ButterflyEnvironment.GetGame().GetItemManager().GetItem(Convert.ToInt32(Row["item_id"]), out ItemData Item))
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.buyoffer.error.3", Session.Langue));
                this.ReloadOffers(Session);
                return;
            }
            else
            {
                if (Convert.ToInt32(Row["user_id"]) == Session.GetUser().Id)
                {
                    Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.buyoffer.error.4", Session.Langue));
                    return;
                }

                if (Convert.ToInt32(Row["total_price"]) > Session.GetUser().WibboPoints)
                {
                    Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.buyoffer.error.5", Session.Langue));
                    return;
                }

                Session.GetUser().WibboPoints -= Convert.ToInt32(Row["total_price"]);
                Session.SendPacket(new ActivityPointNotificationComposer(Session.GetUser().WibboPoints, 0, 105));

                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    UserDao.UpdateRemovePoints(dbClient, Session.GetUser().Id, Convert.ToInt32(Row["total_price"]));
                }

                Item GiveItem = ItemFactory.CreateSingleItem(Item, Session.GetUser(), Convert.ToString(Row["extra_data"]), Convert.ToInt32(Row["furni_id"]), Convert.ToInt32(Row["limited_number"]), Convert.ToInt32(Row["limited_stack"]));
                if (GiveItem != null)
                {
                    Session.GetUser().GetInventoryComponent().TryAddItem(GiveItem);
                    Session.SendPacket(new FurniListNotificationComposer(GiveItem.Id, 1));

                    Session.SendPacket(new PurchaseOKComposer());
                }


                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    CatalogMarketplaceOfferDao.UpdateState(dbClient, OfferId);

                    CatalogMarketplaceDataDao.Replace(dbClient, Item.SpriteId, Convert.ToInt32(Row["total_price"]));

                    if (ButterflyEnvironment.GetGame().GetCatalog().GetMarketplace().MarketAverages.ContainsKey(Item.SpriteId) && ButterflyEnvironment.GetGame().GetCatalog().GetMarketplace().MarketCounts.ContainsKey(Item.SpriteId))
                    {
                        int num3 = ButterflyEnvironment.GetGame().GetCatalog().GetMarketplace().MarketCounts[Item.SpriteId];
                        int num4 = (ButterflyEnvironment.GetGame().GetCatalog().GetMarketplace().MarketAverages[Item.SpriteId] += Convert.ToInt32(Row["total_price"]));

                        ButterflyEnvironment.GetGame().GetCatalog().GetMarketplace().MarketAverages.Remove(Item.SpriteId);
                        ButterflyEnvironment.GetGame().GetCatalog().GetMarketplace().MarketAverages.Add(Item.SpriteId, num4);
                        ButterflyEnvironment.GetGame().GetCatalog().GetMarketplace().MarketCounts.Remove(Item.SpriteId);
                        ButterflyEnvironment.GetGame().GetCatalog().GetMarketplace().MarketCounts.Add(Item.SpriteId, num3 + 1);
                    }
                    else
                    {
                        if (!ButterflyEnvironment.GetGame().GetCatalog().GetMarketplace().MarketAverages.ContainsKey(Item.SpriteId))
                        {
                            ButterflyEnvironment.GetGame().GetCatalog().GetMarketplace().MarketAverages.Add(Item.SpriteId, Convert.ToInt32(Row["total_price"]));
                        }

                        if (!ButterflyEnvironment.GetGame().GetCatalog().GetMarketplace().MarketCounts.ContainsKey(Item.SpriteId))
                        {
                            ButterflyEnvironment.GetGame().GetCatalog().GetMarketplace().MarketCounts.Add(Item.SpriteId, 1);
                        }
                    }
                }
            }

            this.ReloadOffers(Session);
        }

        private void ReloadOffers(Client Session)
        {
            int MinCost = -1;
            int MaxCost = -1;
            string SearchQuery = "";
            int FilterMode = 1;

            DataTable table = null;

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                table = CatalogMarketplaceOfferDao.GetAll(dbClient, SearchQuery, MinCost, MaxCost, FilterMode);

            ButterflyEnvironment.GetGame().GetCatalog().GetMarketplace().MarketItems.Clear();
            ButterflyEnvironment.GetGame().GetCatalog().GetMarketplace().MarketItemKeys.Clear();
            if (table != null)
            {
                foreach (DataRow row in table.Rows)
                {
                    if (!ButterflyEnvironment.GetGame().GetCatalog().GetMarketplace().MarketItemKeys.Contains(Convert.ToInt32(row["offer_id"])))
                    {
                        MarketOffer item = new MarketOffer(Convert.ToInt32(row["offer_id"]), Convert.ToInt32(row["sprite_id"]), Convert.ToInt32(row["total_price"]), int.Parse(row["item_type"].ToString()), Convert.ToInt32(row["limited_number"]), Convert.ToInt32(row["limited_stack"]));
                        ButterflyEnvironment.GetGame().GetCatalog().GetMarketplace().MarketItemKeys.Add(Convert.ToInt32(row["offer_id"]));
                        ButterflyEnvironment.GetGame().GetCatalog().GetMarketplace().MarketItems.Add(item);
                    }
                }
            }

            Dictionary<int, MarketOffer> dictionary = new Dictionary<int, MarketOffer>();
            Dictionary<int, int> dictionary2 = new Dictionary<int, int>();

            foreach (MarketOffer item in ButterflyEnvironment.GetGame().GetCatalog().GetMarketplace().MarketItems)
            {
                if (dictionary.ContainsKey(item.SpriteId))
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
                else
                {
                    dictionary.Add(item.SpriteId, item);
                    dictionary2.Add(item.SpriteId, 1);
                }
            }

            Session.SendPacket(new MarketPlaceOffersComposer(MinCost, MaxCost, dictionary, dictionary2));
        }
    }
}