using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GoToFlatEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
            {
                return;
            }

            if (!Session.GetHabbo().EnterRoom(Session.GetHabbo().CurrentRoom))
            {
                Session.SendPacket(new CloseConnectionComposer());
            }
        }
    }
}
