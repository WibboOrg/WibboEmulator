namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class QuestCompletedMessageComposer : ServerPacket
    {
        public QuestCompletedMessageComposer()
            : base(ServerPacketHeader.QuestCompletedMessageComposer)
        {

        }
    }
}
