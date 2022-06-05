using Wibbo.Communication.Packets.Outgoing.Inventory.Furni;
using Wibbo.Communication.Packets.Outgoing.MarketPlace;
using Wibbo.Database.Daos;
using Wibbo.Database.Interfaces;
using Wibbo.Game.Clients;
using Wibbo.Game.Items;
using System.Data;

namespace Wibbo.Communication.Packets.Incoming.Marketplace
{
    internal class CancelOfferEvent : IPacketEvent
    {
        public double Delay => 1000;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetUser() == null)
            {
                return;
            }

            DataRow Row = null;
            int OfferId = Packet.PopInt();

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
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

            if (Convert.ToInt32(Row["user_id"]) != Session.GetUser().Id)
            {
                Session.SendPacket(new MarketplaceCancelOfferResultComposer(OfferId, false));
                return;
            }

            if (!WibboEnvironment.GetGame().GetItemManager().GetItem(Convert.ToInt32(Row["item_id"]), out ItemData Item))
            {
                Session.SendPacket(new MarketplaceCancelOfferResultComposer(OfferId, false));
                return;
            }

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                CatalogMarketplaceOfferDao.DeleteUserOffer(dbClient, OfferId, Session.GetUser().Id);

            Item GiveItem = ItemFactory.CreateSingleItem(Item, Session.GetUser(), Convert.ToString(Row["extra_data"]), Convert.ToInt32(Row["furni_id"]), Convert.ToInt32(Row["limited_number"]), Convert.ToInt32(Row["limited_stack"]));

            if (GiveItem != null)
            {
                Session.GetUser().GetInventoryComponent().TryAddItem(GiveItem);
                Session.SendPacket(new FurniListNotificationComposer(GiveItem.Id, 1));
            }

            Session.SendPacket(new MarketplaceCancelOfferResultComposer(OfferId, true));
        }
    }
}
