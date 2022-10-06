namespace WibboEmulator.Communication.Packets.Outgoing.Quests;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Quests;

internal class QuestCompletedComposer : ServerPacket
{
    public QuestCompletedComposer(GameClient session, Quest Quest)
        : base(ServerPacketHeader.QUEST_COMPLETED)
    {
        var questsInCategory = WibboEnvironment.GetGame().GetQuestManager().GetAmountOfQuestsInCategory(Quest.Category);
        var i = Quest.Number - 1;
        var num = session.GetUser().GetQuestProgress(Quest.Id);
        if (Quest.IsCompleted(num))
        {
            i++;
        }

        this.WriteString(Quest.Category);
        this.WriteInteger(i);
        this.WriteInteger(questsInCategory);
        this.WriteInteger(0);
        this.WriteInteger(Quest.Id);
        this.WriteBoolean(session.GetUser().CurrentQuestId == Quest.Id);
        this.WriteString(Quest.ActionName);
        this.WriteString(Quest.DataBit);
        this.WriteInteger(Quest.Reward);
        this.WriteString(Quest.Name);
        this.WriteInteger(num);
        this.WriteInteger(Quest.GoalData);
        this.WriteInteger(GetIntValue(Quest.Category));
        this.WriteString("set_kuurna");
        this.WriteString("MAIN_CHAIN");
        this.WriteBoolean(true);
        this.WriteBoolean(true);
    }

    private static int GetIntValue(string QuestCategory) => QuestCategory switch
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
