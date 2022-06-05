using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class TradingCancelEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetUser() == null || !Session.GetUser().InRoom)
            {
                return;
            }

            Room room = WibboEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
            if (room == null)
            {
                return;
            }

            room.TryStopTrade(Session.GetUser().Id);
        }
    }
}
