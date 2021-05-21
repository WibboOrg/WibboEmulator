namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class QuestListMessageComposer : ServerPacket
    {
        public QuestListMessageComposer()
            : base(ServerPacketHeader.QuestListMessageComposer)
        {

        }
    }
}
