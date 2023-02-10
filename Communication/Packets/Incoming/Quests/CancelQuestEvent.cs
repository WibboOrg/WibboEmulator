namespace WibboEmulator.Communication.Packets.Incoming.Quests;
using WibboEmulator.Games.GameClients;

internal sealed class CancelQuestEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet) => WibboEnvironment.GetGame().GetQuestManager().CancelQuest(session);
}
