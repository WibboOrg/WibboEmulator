using Butterfly.Game.Clients;
using Butterfly.Game.Quests;

namespace Butterfly.Communication.Packets.Outgoing.Quests
{
    internal class QuestListComposer : ServerPacket
    {
        public QuestListComposer(Dictionary<string, Quest> quests, Client session, bool send)
            : base(ServerPacketHeader.QUESTS)
        {
            WriteInteger(quests.Count);
            foreach (KeyValuePair<string, Quest> keyValuePair in quests)
            {
                if (keyValuePair.Value != null)
                {
                    SerializeQuest(session, keyValuePair.Value, keyValuePair.Key);
                }
            }

            foreach (KeyValuePair<string, Quest> keyValuePair in quests)
            {
                if (keyValuePair.Value == null)
                {
                    SerializeQuest(session, keyValuePair.Value, keyValuePair.Key);
                }
            }

            WriteBoolean(send);
        }

        private void SerializeQuest(Client Session, Quest Quest, string Category)
        {
            int questsInCategory = ButterflyEnvironment.GetGame().GetQuestManager().GetAmountOfQuestsInCategory(Category);
            int i = Quest == null ? questsInCategory : Quest.Number - 1;
            int num = Quest == null ? 0 : Session.GetUser().GetQuestProgress(Quest.Id);
            if (Quest != null && Quest.IsCompleted(num))
            {
                i++;
            }

            WriteString(Category);
            WriteInteger(i);
            WriteInteger(questsInCategory);
            WriteInteger(0);
            WriteInteger(Quest == null ? 0 : Quest.Id);
            WriteBoolean(Quest != null && Session.GetUser().CurrentQuestId == Quest.Id);
            WriteString(Quest == null ? string.Empty : Quest.ActionName);
            WriteString(Quest == null ? string.Empty : Quest.DataBit);
            WriteInteger(Quest == null ? 0 : Quest.Reward);
            WriteString(Quest == null ? string.Empty : Quest.Name);
            WriteInteger(num);
            WriteInteger(Quest == null ? 0 : Quest.GoalData);
            WriteInteger(QuestTypeUtillity.GetIntValue(Category));
            WriteString("set_kuurna");
            WriteString("MAIN_CHAIN");
            WriteBoolean(true);
        }

    }
}
