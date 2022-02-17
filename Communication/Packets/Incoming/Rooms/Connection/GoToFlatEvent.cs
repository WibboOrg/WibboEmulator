using Butterfly.Communication.Packets.Outgoing.Rooms.Session;
using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GoToFlatEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
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
