namespace WibboEmulator.Communication.Packets.Outgoing.Quests;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Quests;

internal sealed class QuestListComposer : ServerPacket
{
    public QuestListComposer(Dictionary<string, Quest> quests, GameClient session, bool send)
        : base(ServerPacketHeader.QUESTS)
    {
        this.WriteInteger(quests.Count);
        foreach (var keyValuePair in quests)
        {
            if (keyValuePair.Value != null)
            {
                this.SerializeQuest(session, keyValuePair.Value, keyValuePair.Key);
            }
        }

        foreach (var keyValuePair in quests)
        {
            if (keyValuePair.Value == null)
            {
                this.SerializeQuest(session, keyValuePair.Value, keyValuePair.Key);
            }
        }

        this.WriteBoolean(send);
    }

    private void SerializeQuest(GameClient session, Quest quest, string category)
    {
        var questsInCategory = QuestManager.GetAmountOfQuestsInCategory(category);
        var i = quest == null ? questsInCategory : quest.Number - 1;
        var num = quest == null ? 0 : session.User.GetQuestProgress(quest.Id);
        if (quest != null && quest.IsCompleted(num))
        {
            i++;
        }

        this.WriteString(category);
        this.WriteInteger(i);
        this.WriteInteger(questsInCategory);
        this.WriteInteger(0);
        this.WriteInteger(quest == null ? 0 : quest.Id);
        this.WriteBoolean(quest != null && session.User.QuestId == quest.Id);
        this.WriteString(quest == null ? string.Empty : quest.ActionName);
        this.WriteString(quest == null ? string.Empty : quest.DataBit);
        this.WriteInteger(quest == null ? 0 : quest.Reward);
        this.WriteString(quest == null ? string.Empty : quest.Name);
        this.WriteInteger(num);
        this.WriteInteger(quest == null ? 0 : quest.GoalData);
        this.WriteInteger(QuestTypeUtillity.GetIntValue(category));
        this.WriteString("set_kuurna");
        this.WriteString("MAIN_CHAIN");
        this.WriteBoolean(true);
    }
}
