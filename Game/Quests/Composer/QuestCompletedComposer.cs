using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Game.Clients;

namespace Butterfly.Game.Quests.Composer
{
    public class QuestCompletedComposer
    {
        public static ServerPacket Compose(Client Session, Quest Quest)
        {
            ServerPacket Message = new ServerPacket(ServerPacketHeader.QUEST_COMPLETED);
            QuestListComposer.SerializeQuest(Message, Session, Quest, Quest.Category);
            Message.WriteBoolean(true);

            return Message;
        }
    }
}
