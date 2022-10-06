namespace WibboEmulator.Games.Quests;
using System.Data;
using WibboEmulator.Communication.Packets.Incoming;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Communication.Packets.Outgoing.Quests;
using WibboEmulator.Database.Daos.Emulator;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.GameClients;

public class QuestManager
{
    private Dictionary<int, Quest> _quests;
    private Dictionary<string, int> _questCount;

    public void Init(IQueryAdapter dbClient)
    {
        this._quests = new Dictionary<int, Quest>();
        this._questCount = new Dictionary<string, int>();

        this.ReloadQuests(dbClient);
    }

    public void ReloadQuests(IQueryAdapter dbClient)
    {
        this._quests.Clear();

        var table = EmulatorQuestDao.GetAll(dbClient);
        foreach (DataRow dataRow in table.Rows)
        {
            var id = Convert.ToInt32(dataRow["id"]);
            var category = (string)dataRow["category"];
            var seriesNumber = Convert.ToInt32(dataRow["series_number"]);
            var goalType = Convert.ToInt32(dataRow["goal_type"]);
            var goalData = Convert.ToInt32(dataRow["goal_data"]);
            var name = (string)dataRow["name"];
            var reward = Convert.ToInt32(dataRow["reward"]);
            var dataBit = (string)dataRow["data_bit"];

            this._quests.Add(id, new Quest(id, category, seriesNumber, (QuestType)goalType, goalData, name, reward, dataBit));

            this.AddToCounter(category);
        }
    }

    private void AddToCounter(string category)
    {
        if (this._questCount.TryGetValue(category, out var num))
        {
            this._questCount[category] = num + 1;
        }
        else
        {
            this._questCount.Add(category, 1);
        }
    }

    public Quest GetQuest(int Id)
    {
        this._quests.TryGetValue(Id, out var quest);

        return quest;
    }

    public int GetAmountOfQuestsInCategory(string Category)
    {
        this._questCount.TryGetValue(Category, out var num);

        return num;
    }

    public void ProgressUserQuest(GameClient session, QuestType QuestType, int EventData = 0)
    {
        if (session == null || session.GetUser() == null || session.GetUser().CurrentQuestId <= 0)
        {
            return;
        }

        var quest = this.GetQuest(session.GetUser().CurrentQuestId);
        if (quest == null || quest.GoalType != QuestType)
        {
            return;
        }

        var questProgress = session.GetUser().GetQuestProgress(quest.Id);
        var flag = false;
        int progress;
        if (QuestType != QuestType.EXPLORE_FIND_ITEM)
        {
            progress = questProgress + 1;
            if (progress >= (long)quest.GoalData)
            {
                flag = true;
            }
        }
        else
        {
            if (EventData != quest.GoalData)
            {
                return;
            }

            progress = quest.GoalData;
            flag = true;
        }
        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            UserQuestDao.Update(dbClient, session.GetUser().Id, quest.Id, progress);
        }

        session.GetUser().Quests[session.GetUser().CurrentQuestId] = progress;
        session.SendPacket(new QuestStartedComposer(session, quest));

        if (!flag)
        {
            return;
        }

        session.GetUser().CurrentQuestId = 0;
        session.GetUser().LastCompleted = quest.Id;
        session.SendPacket(new QuestCompletedComposer(session, quest));
        session.GetUser().Duckets += quest.Reward;
        session.SendPacket(new ActivityPointNotificationComposer(session.GetUser().Duckets, 1));
        this.SendQuestList(session);
    }

    public Quest GetNextQuestInSeries(string Category, int Number)
    {
        foreach (var quest in this._quests.Values)
        {
            if (quest.Category == Category && quest.Number == Number)
            {
                return quest;
            }
        }

        return null;
    }

    public void SendQuestList(GameClient session, bool send = true)
    {
        var dictionary1 = new Dictionary<string, int>();
        var dictionary2 = new Dictionary<string, Quest>();

        foreach (var quest in this._quests.Values)
        {
            if (!dictionary1.ContainsKey(quest.Category))
            {
                dictionary1.Add(quest.Category, 1);
                dictionary2.Add(quest.Category, null);
            }
            if (quest.Number >= dictionary1[quest.Category])
            {
                var questProgress = session.GetUser().GetQuestProgress(quest.Id);
                if (session.GetUser().CurrentQuestId != quest.Id && questProgress >= (long)quest.GoalData)
                {
                    dictionary1[quest.Category] = quest.Number + 1;
                }
            }
        }

        foreach (var quest in this._quests.Values)
        {
            foreach (var keyValuePair in dictionary1)
            {
                if (quest.Category == keyValuePair.Key && quest.Number == keyValuePair.Value)
                {
                    dictionary2[keyValuePair.Key] = quest;
                    break;
                }
            }
        }

        session.SendPacket(new QuestListComposer(dictionary2, session, send));
    }

    public void ActivateQuest(GameClient session, ClientPacket Message)
    {
        var quest = this.GetQuest(Message.PopInt());
        if (quest == null)
        {
            return;
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            UserQuestDao.Replace(dbClient, session.GetUser().Id, quest.Id);
        }

        session.GetUser().CurrentQuestId = quest.Id;
        this.SendQuestList(session);
        session.SendPacket(new QuestStartedComposer(session, quest));
    }

    public void GetCurrentQuest(GameClient session)
    {
        if (!session.GetUser().InRoom)
        {
            return;
        }

        var quest = this.GetQuest(session.GetUser().LastCompleted);
        var nextQuestInSeries = this.GetNextQuestInSeries(quest.Category, quest.Number + 1);
        if (nextQuestInSeries == null)
        {
            return;
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            UserQuestDao.Replace(dbClient, session.GetUser().Id, nextQuestInSeries.Id);
        }

        session.GetUser().CurrentQuestId = nextQuestInSeries.Id;
        this.SendQuestList(session);
        session.SendPacket(new QuestStartedComposer(session, nextQuestInSeries));
    }

    public void CancelQuest(GameClient session)
    {
        var quest = this.GetQuest(session.GetUser().CurrentQuestId);
        if (quest == null)
        {
            return;
        }

        session.GetUser().CurrentQuestId = 0;
        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            UserQuestDao.Delete(dbClient, session.GetUser().Id, quest.Id);
        }

        session.SendPacket(new QuestAbortedComposer());
        this.SendQuestList(session);
    }
}
