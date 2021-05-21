using Butterfly.HabboHotel.GameClients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class StartQuestEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            ButterflyEnvironment.GetGame().GetQuestManager().ActivateQuest(Session, Packet);
        }
    }
}
