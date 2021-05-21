namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class QuestStartedMessageComposer : ServerPacket
    {
        public QuestStartedMessageComposer()
            : base(ServerPacketHeader.QuestStartedMessageComposer)
        {

        }
    }
}
