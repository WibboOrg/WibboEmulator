using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Trading;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class TradeOfferMultipleItemsEvent : IPacketEvent
    {
        public double Delay => 500;

        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetUser().CurrentRoomId, out Room room))
                return;

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
