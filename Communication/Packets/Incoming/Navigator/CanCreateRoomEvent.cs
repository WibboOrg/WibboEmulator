using Butterfly.Communication.Packets.Outgoing.Navigator;

using Butterfly.HabboHotel.GameClients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class CanCreateRoomEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Session.SendPacket(new CanCreateRoomComposer(false, 200));
        }
    }
}