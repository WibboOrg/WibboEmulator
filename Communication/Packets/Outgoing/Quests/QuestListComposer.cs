namespace Butterfly.Communication.Packets.Outgoing.Quests
{
    internal class QuestListComposer : ServerPacket
    {
        public QuestListComposer()
            : base(ServerPacketHeader.QuestListMessageComposer)
        {

        }
    }
}
