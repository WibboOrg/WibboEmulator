using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Quests;

namespace WibboEmulator.Communication.Packets.Outgoing.Quests
{
    internal class QuestStartedComposer : ServerPacket
    {
        public QuestStartedComposer(GameClient Session, Quest Quest)
            : base(ServerPacketHeader.QUEST)
        {
            int questsInCategory = WibboEnvironment.GetGame().GetQuestManager().GetAmountOfQuestsInCategory(Quest.Category);
            int i = Quest == null ? questsInCategory : Quest.Number - 1;
            int num = Quest == null ? 0 : Session.GetUser().GetQuestProgress(Quest.Id);
            if (Quest != null && Quest.IsCompleted(num))
            {
                i++;
            }

            WriteString(Quest?.Category ?? string.Empty);
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
            WriteInteger(QuestTypeUtillity.GetIntValue(Quest?.Category ?? ""));
            WriteString("set_kuurna");
            WriteString("MAIN_CHAIN");
            WriteBoolean(true);
        }
    }
}
