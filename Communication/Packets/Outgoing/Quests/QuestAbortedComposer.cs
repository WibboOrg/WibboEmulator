namespace Butterfly.Communication.Packets.Outgoing.Quests
{
    internal class QuestAbortedComposer : ServerPacket
    {
        public QuestAbortedComposer()
            : base(ServerPacketHeader.QuestAbortedMessageComposer)
        {
            this.WriteBoolean(false);
        }
    }
}
