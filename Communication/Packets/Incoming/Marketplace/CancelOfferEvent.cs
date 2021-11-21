using Butterfly.Communication.Packets.Outgoing.Inventory.Furni;
using Butterfly.Communication.Packets.Outgoing.MarketPlace;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.GameClients;
using Butterfly.Game.Items;
using System;
using System.Data;

namespace Butterfly.Communication.Packets.Incoming.Marketplace
{
    internal class CancelOfferEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
            {
                return;
            }

            DataRow Row = null;
            int OfferId = Packet.PopInt();

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                Row = CatalogMarketplaceOfferDao.GetByOfferId(dbClient, OfferId);

            if (Row == null)
            {
                Session.SendPacket(new MarketplaceCancelOfferResultComposer(OfferId, false));
                return;
            }

            if (Convert.ToString(Row["state"]) == "2")
            {
                Session.SendPacket(new MarketplaceCancelOfferResultComposer(OfferId, false));
                return;
            }

            if (Convert.ToInt32(Row["user_id"]) != Session.GetHabbo().Id)
            {
                Session.SendPacket(new MarketplaceCancelOfferResultComposer(OfferId, false));
                return;
            }

            if (!ButterflyEnvironment.GetGame().GetItemManager().GetItem(Convert.ToInt32(Row["item_id"]), out ItemData Item))
            {
                Session.SendPacket(new MarketplaceCancelOfferResultComposer(OfferId, false));
                return;
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                CatalogMarketplaceOfferDao.DeleteUserOffer(dbClient, OfferId, Session.GetHabbo().Id);

            Item GiveItem = ItemFactory.CreateSingleItem(Item, Session.GetHabbo(), Convert.ToString(Row["extra_data"]), Convert.ToInt32(Row["furni_id"]), Convert.ToInt32(Row["limited_number"]), Convert.ToInt32(Row["limited_stack"]));

            if (GiveItem != null)
            {
                Session.GetHabbo().GetInventoryComponent().TryAddItem(GiveItem);
                Session.SendPacket(new FurniListNotificationComposer(GiveItem.Id, 1));
            }

            Session.SendPacket(new MarketplaceCancelOfferResultComposer(OfferId, true));
        }
    }
}
