namespace Butterfly.Communication.Packets.Outgoing.Quests
{
    internal class QuestCompletedMessageComposer : ServerPacket
    {
        public QuestCompletedMessageComposer()
            : base(ServerPacketHeader.QuestCompletedMessageComposer)
        {

        }
    }
}
