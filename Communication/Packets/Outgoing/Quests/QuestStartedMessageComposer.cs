namespace Butterfly.Communication.Packets.Outgoing.Quests
{
    internal class QuestStartedMessageComposer : ServerPacket
    {
        public QuestStartedMessageComposer()
            : base(ServerPacketHeader.QuestStartedMessageComposer)
        {

        }
    }
}
