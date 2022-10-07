namespace WibboEmulator.Communication.Packets.Outgoing.Quests;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Quests;

internal class QuestCompletedComposer : ServerPacket
{
    public QuestCompletedComposer(GameClient session, Quest quest)
        : base(ServerPacketHeader.QUEST_COMPLETED)
    {
        var questsInCategory = WibboEnvironment.GetGame().GetQuestManager().GetAmountOfQuestsInCategory(quest.Category);
        var i = quest.Number - 1;
        var num = session.GetUser().GetQuestProgress(quest.Id);
        if (quest.IsCompleted(num))
        {
            i++;
        }

        this.WriteString(quest.Category);
        this.WriteInteger(i);
        this.WriteInteger(questsInCategory);
        this.WriteInteger(0);
        this.WriteInteger(quest.Id);
        this.WriteBoolean(session.GetUser().CurrentQuestId == quest.Id);
        this.WriteString(quest.ActionName);
        this.WriteString(quest.DataBit);
        this.WriteInteger(quest.Reward);
        this.WriteString(quest.Name);
        this.WriteInteger(num);
        this.WriteInteger(quest.GoalData);
        this.WriteInteger(GetIntValue(quest.Category));
        this.WriteString("set_kuurna");
        this.WriteString("MAIN_CHAIN");
        this.WriteBoolean(true);
        this.WriteBoolean(true);
    }

    private static int GetIntValue(string questCategory) => questCategory switch
    {
        "room_builder" => 2,
        "social" => 3,
        "identity" => 4,
        "explore" => 5,
        "battleball" => 7,
        "freeze" => 8,
        _ => 0,
    };
}
