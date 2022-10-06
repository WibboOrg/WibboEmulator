namespace WibboEmulator.Communication.Packets.Incoming.Quests;
using WibboEmulator.Games.GameClients;

internal class StartQuestEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet) => WibboEnvironment.GetGame().GetQuestManager().ActivateQuest(session, packet);
}
