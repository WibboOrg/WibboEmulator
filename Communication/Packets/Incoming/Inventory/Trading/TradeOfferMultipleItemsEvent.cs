using Wibbo.Game.Clients;
using Wibbo.Game.Items;
using Wibbo.Game.Rooms;
using Wibbo.Game.Rooms.Trading;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class TradeOfferMultipleItemsEvent : IPacketEvent
    {
        public double Delay => 500;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Room room = WibboEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
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
