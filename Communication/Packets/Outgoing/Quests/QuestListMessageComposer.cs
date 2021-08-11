namespace Butterfly.Communication.Packets.Outgoing.Quests
{
    internal class QuestListMessageComposer : ServerPacket
    {
        public QuestListMessageComposer()
            : base(ServerPacketHeader.QuestListMessageComposer)
        {

        }
    }
}
