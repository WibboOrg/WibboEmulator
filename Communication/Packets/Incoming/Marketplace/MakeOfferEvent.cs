using Butterfly.Communication.Packets.Outgoing.MarketPlace;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.Catalog.Utilities;
using Butterfly.HabboHotel.Items;

namespace Butterfly.Communication.Packets.Incoming.Marketplace
{
    internal class MakeOfferEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int SellingPrice = Packet.PopInt();
            int ComissionPrice = Packet.PopInt();
            int ItemId = Packet.PopInt();

            Item Item = Session.GetHabbo().GetInventoryComponent().GetItem(ItemId);
            if (Item == null)
            {
                Session.SendPacket(new MarketplaceMakeOfferResultComposer(0));
                return;
            }

            if (!ItemUtility.IsRare(Item))
            {
                return;
            }

            if (SellingPrice > 999999 || SellingPrice <= 0)
            {
                Session.SendPacket(new MarketplaceMakeOfferResultComposer(0));
                return;
            }

            int Comission = ButterflyEnvironment.GetGame().GetCatalog().GetMarketplace().CalculateComissionPrice(SellingPrice);
            int TotalPrice = SellingPrice + Comission;
            int ItemType = 1;
            if (Item.GetBaseItem().Type == 'i')
            {
                ItemType++;
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("DELETE items, items_limited FROM items LEFT JOIN items_limited ON(items_limited.item_id = items.id) WHERE `id` = '" + ItemId + "'");

                dbClient.SetQuery("INSERT INTO `catalog_marketplace_offers` (`furni_id`,`item_id`,`user_id`,`asking_price`,`total_price`,`public_name`,`sprite_id`,`item_type`,`timestamp`,`extra_data`,`limited_number`,`limited_stack`) VALUES ('" + ItemId + "','" + Item.BaseItem + "','" + Session.GetHabbo().Id + "','" + SellingPrice + "','" + TotalPrice + "',@public_name,'" + Item.GetBaseItem().SpriteId + "','" + ItemType + "','" + ButterflyEnvironment.GetUnixTimestamp() + "',@extra_data, '" + Item.Limited + "', '" + Item.LimitedStack + "')");
                dbClient.AddParameter("public_name", Item.GetBaseItem().ItemName);
                dbClient.AddParameter("extra_data", Item.ExtraData);
                dbClient.RunQuery();

            }

            Session.GetHabbo().GetInventoryComponent().RemoveItem(ItemId);
            Session.SendPacket(new MarketplaceMakeOfferResultComposer(1));
        }
    }
}