namespace WibboEmulator.Communication.Packets.Incoming.Quests;
using WibboEmulator.Games.GameClients;

internal class GetQuestListEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket Packet) => WibboEnvironment.GetGame().GetQuestManager().SendQuestList(session, false);
}
