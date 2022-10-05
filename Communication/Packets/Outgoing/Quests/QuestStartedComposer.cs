namespace WibboEmulator.Communication.Packets.Outgoing.Quests;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Quests;

internal class QuestStartedComposer : ServerPacket
{
    public QuestStartedComposer(GameClient session, Quest Quest)
        : base(ServerPacketHeader.QUEST)
    {
        var questsInCategory = WibboEnvironment.GetGame().GetQuestManager().GetAmountOfQuestsInCategory(Quest.Category);
        var i = Quest == null ? questsInCategory : Quest.Number - 1;
        var num = Quest == null ? 0 : session.GetUser().GetQuestProgress(Quest.Id);
        if (Quest != null && Quest.IsCompleted(num))
        {
            i++;
        }

        this.WriteString(Quest?.Category ?? string.Empty);
        this.WriteInteger(i);
        this.WriteInteger(questsInCategory);
        this.WriteInteger(0);
        this.WriteInteger(Quest == null ? 0 : Quest.Id);
        this.WriteBoolean(Quest != null && session.GetUser().CurrentQuestId == Quest.Id);
        this.WriteString(Quest == null ? string.Empty : Quest.ActionName);
        this.WriteString(Quest == null ? string.Empty : Quest.DataBit);
        this.WriteInteger(Quest == null ? 0 : Quest.Reward);
        this.WriteString(Quest == null ? string.Empty : Quest.Name);
        this.WriteInteger(num);
        this.WriteInteger(Quest == null ? 0 : Quest.GoalData);
        this.WriteInteger(QuestTypeUtillity.GetIntValue(Quest?.Category ?? ""));
        this.WriteString("set_kuurna");
        this.WriteString("MAIN_CHAIN");
        this.WriteBoolean(true);
    }
}
