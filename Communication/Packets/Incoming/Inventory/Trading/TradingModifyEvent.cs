using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;
using Wibbo.Game.Rooms.Trading;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class TradingModifyEvent : IPacketEvent
    {
        public double Delay => 0;

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

            userTrade.Unaccept(Session.GetUser().Id);
        }
    }
}
