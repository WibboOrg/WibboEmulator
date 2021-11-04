using Butterfly.Communication.Packets.Outgoing.MarketPlace;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using System;
using System.Data;

namespace Butterfly.Communication.Packets.Incoming.Marketplace
{
    internal class GetMarketplaceItemStatsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int ItemId = Packet.PopInt();
            int SpriteId = Packet.PopInt();

            DataRow Row = null;
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                Row = CatalogMarketplaceDataDao.GetPriceBySprite(dbClient, SpriteId);

            Session.SendPacket(new MarketplaceItemStatsComposer(ItemId, SpriteId, (Row != null ? Convert.ToInt32(Row["avgprice"]) : 0)));
        }
    }
}
