using Butterfly.Communication.Packets.Outgoing.MarketPlace;
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
            {
                dbClient.SetQuery("SELECT `avgprice` FROM `catalog_marketplace_data` WHERE `sprite` = @SpriteId LIMIT 1");
                dbClient.AddParameter("SpriteId", SpriteId);
                Row = dbClient.GetRow();
            }

            Session.SendPacket(new MarketplaceItemStatsComposer(ItemId, SpriteId, (Row != null ? Convert.ToInt32(Row["avgprice"]) : 0)));
        }
    }
}
