using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class StartQuestEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            ButterflyEnvironment.GetGame().GetQuestManager().ActivateQuest(Session, Packet);
        }
    }
}
