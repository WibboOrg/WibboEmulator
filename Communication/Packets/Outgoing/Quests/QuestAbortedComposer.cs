namespace WibboEmulator.Communication.Packets.Outgoing.Quests;

internal sealed class QuestAbortedComposer : ServerPacket
{
    public QuestAbortedComposer()
        : base(ServerPacketHeader.QUEST_CANCELLED) => this.WriteBoolean(false);
}
