namespace Butterfly.Communication.Packets.Outgoing.Quests
{
    internal class QuestStartedComposer : ServerPacket
    {
        public QuestStartedComposer()
            : base(ServerPacketHeader.QUEST)
        {

        }
    }
}
