using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class TradingAcceptEvent : IPacketEvent
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
            if (userTrade == null)
            {
                return;
            }

            userTrade.Accept(Session.GetUser().Id);
        }
    }
}