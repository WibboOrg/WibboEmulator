using WibboEmulator.Games.Clients;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class StartQuestEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            WibboEnvironment.GetGame().GetQuestManager().ActivateQuest(Session, Packet);
        }
    }
}
