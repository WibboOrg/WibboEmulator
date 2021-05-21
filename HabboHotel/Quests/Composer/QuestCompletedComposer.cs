using Butterfly.Communication.Packets.Outgoing;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Quests.Composer
{
    public class QuestCompletedComposer
    {
        public static ServerPacket Compose(GameClient Session, Quest Quest)
        {
            ServerPacket Message = new ServerPacket(ServerPacketHeader.QuestCompletedMessageComposer);
            QuestListComposer.SerializeQuest(Message, Session, Quest, Quest.Category);
            Message.WriteBoolean(true);

            return Message;
        }
    }
}
