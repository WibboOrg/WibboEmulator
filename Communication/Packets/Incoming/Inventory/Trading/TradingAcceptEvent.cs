using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Trading;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class TradingAcceptEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetUser().CurrentRoomId, out Room room))
                return;

            Trade userTrade = room.GetUserTrade(Session.GetUser().Id);
            if (userTrade == null)
            {
                return;
            }

            userTrade.Accept(Session.GetUser().Id);
        }
    }
}