using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class TradeOfferMultipleItemsEvent : IPacketEvent
    {
        public double Delay => 500;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null)
            {
                return;
            }

            Trade userTrade = room.GetUserTrade(Session.GetHabbo().Id);
            if (userTrade == null)
            {
                return;
            }

            int ItemCount = Packet.PopInt();
            for (int i = 0; i < ItemCount; i++)
            {
                int ItemId = Packet.PopInt();
                Item userItem = Session.GetHabbo().GetInventoryComponent().GetItem(ItemId);
                if (userItem == null)
                {
                    continue;
                }

                userTrade.OfferItem(Session.GetHabbo().Id, userItem, false);
            }

            userTrade.UpdateTradeWindow();
        }
    }
}
