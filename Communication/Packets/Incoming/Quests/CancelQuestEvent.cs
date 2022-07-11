using WibboEmulator.Game.Clients;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class CancelQuestEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            WibboEnvironment.GetGame().GetQuestManager().CancelQuest(Session);
        }
    }
}
