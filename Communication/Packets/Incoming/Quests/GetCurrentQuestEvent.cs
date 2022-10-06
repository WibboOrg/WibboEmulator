namespace WibboEmulator.Communication.Packets.Incoming.Quests;
using WibboEmulator.Games.GameClients;

internal class GetCurrentQuestEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket Packet) => WibboEnvironment.GetGame().GetQuestManager().GetCurrentQuest(session);
}