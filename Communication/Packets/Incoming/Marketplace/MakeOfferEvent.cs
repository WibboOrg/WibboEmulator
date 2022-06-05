using Wibbo.Communication.Packets.Outgoing.MarketPlace;
using Wibbo.Database.Daos;
using Wibbo.Database.Interfaces;
using Wibbo.Game.Catalog.Utilities;
using Wibbo.Game.Clients;
using Wibbo.Game.Items;

namespace Wibbo.Communication.Packets.Incoming.Marketplace
{
    internal class MakeOfferEvent : IPacketEvent
    {
        public double Delay => 1000;

        public void Parse(Client Session, ClientPacket Packet)
        {
            int SellingPrice = Packet.PopInt();
            int ComissionPrice = Packet.PopInt();
            int ItemId = Packet.PopInt();

            Item Item = Session.GetUser().GetInventoryComponent().GetItem(ItemId);
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

            int Comission = WibboEnvironment.GetGame().GetCatalog().GetMarketplace().CalculateComissionPrice(SellingPrice);
            int TotalPrice = SellingPrice + Comission;
            int ItemType = 1;
            if (Item.GetBaseItem().Type == 'i')
            {
                ItemType++;
            }

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                ItemDao.Delete(dbClient, ItemId);

                CatalogMarketplaceOfferDao.Insert(dbClient, Item.GetBaseItem().ItemName, Item.ExtraData, ItemId, Item.BaseItem, Session.GetUser().Id, SellingPrice, TotalPrice, Item.GetBaseItem().SpriteId, ItemType, Item.Limited, Item.LimitedStack);

            }

            Session.GetUser().GetInventoryComponent().RemoveItem(ItemId);
            Session.SendPacket(new MarketplaceMakeOfferResultComposer(1));
        }
    }
}