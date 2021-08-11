namespace Butterfly.Communication.Packets.Outgoing.Quests
{
    internal class QuestAbortedMessageComposer : ServerPacket
    {
        public QuestAbortedMessageComposer()
            : base(ServerPacketHeader.QuestAbortedMessageComposer)
        {
            this.WriteBoolean(false);
        }
    }
}
