namespace Butterfly.Communication.Packets.Outgoing.Quests
{
    internal class QuestCompletedComposer : ServerPacket
    {
        public QuestCompletedComposer()
            : base(ServerPacketHeader.QUEST_COMPLETED)
        {

        }
    }
}
