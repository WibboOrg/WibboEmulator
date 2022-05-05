using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms;
using Butterfly.Game.Rooms.Trading;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class TradingOfferItemEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
            if (room == null)
            {
                return;
            }

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
