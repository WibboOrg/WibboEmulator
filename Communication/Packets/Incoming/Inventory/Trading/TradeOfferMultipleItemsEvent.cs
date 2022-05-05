using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms;
using Butterfly.Game.Rooms.Trading;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class TradeOfferMultipleItemsEvent : IPacketEvent
    {
        public double Delay => 500;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
            if (room == null)
            {
                return;
            }

            Trade userTrade = room.GetUserTrade(Session.GetUser().Id);
            if (userTrade == null)
            {
                return;
            }

            int ItemCount = Packet.PopInt();
            for (int i = 0; i < ItemCount; i++)
            {
                int ItemId = Packet.PopInt();
                Item userItem = Session.GetUser().GetInventoryComponent().GetItem(ItemId);
                if (userItem == null)
                {
                    continue;
                }

                userTrade.OfferItem(Session.GetUser().Id, userItem, false);
            }

            userTrade.UpdateTradeWindow();
        }
    }
}
