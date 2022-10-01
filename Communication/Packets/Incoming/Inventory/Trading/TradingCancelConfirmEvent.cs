using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class TradingCancelConfirmEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetUser().CurrentRoomId, out Room room))
                return;

            room.TryStopTrade(Session.GetUser().Id);
        }
    }
}
