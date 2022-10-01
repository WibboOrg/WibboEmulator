using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Trading;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class TradingOfferItemEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetUser().CurrentRoomId, out Room room))
                return;

            Trade userTrade = room.GetUserTrade(Session.GetUser().Id);
            Item userItem = Session.GetUser().GetInventoryComponent().GetItem(Packet.PopInt());
            if (userTrade == null || userItem == null)
            {
                return;
            }

            userTrade.OfferItem(Session.GetUser().Id, userItem);
        }
    }
}
