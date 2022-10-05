using WibboEmulator.Games.GameClients;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class GetCurrentQuestEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(GameClient Session, ClientPacket Packet) => WibboEnvironment.GetGame().GetQuestManager().GetCurrentQuest(Session);
    }
}