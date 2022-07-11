using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Items;
using WibboEmulator.Game.Rooms;
using WibboEmulator.Game.Rooms.Trading;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
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
