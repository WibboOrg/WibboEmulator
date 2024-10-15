namespace WibboEmulator.Communication.Packets.Incoming.Quests;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Quests;

internal sealed class CancelQuestEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet) => QuestManager.CancelQuest(Session);
}
