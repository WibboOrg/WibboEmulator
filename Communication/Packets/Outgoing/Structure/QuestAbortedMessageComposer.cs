namespace Butterfly.Communication.Packets.Outgoing.Structure
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
