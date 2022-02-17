using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetQuestListEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            ButterflyEnvironment.GetGame().GetQuestManager().SendQuestList(Session, false);
        }
    }
}
