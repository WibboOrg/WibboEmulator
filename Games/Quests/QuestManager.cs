namespace WibboEmulator.Games.Quests;
using System.Data;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Communication.Packets.Outgoing.Quests;
using WibboEmulator.Database.Daos.Emulator;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;

public class QuestManager
{
    private readonly Dictionary<int, Quest> _quests;
    private readonly Dictionary<string, int> _questCount;

    public QuestManager()
    {
        this._quests = new Dictionary<int, Quest>();
        this._questCount = new Dictionary<string, int>();
    }

    public void Initialize(IDbConnection dbClient)
    {
        this._quests.Clear();
        this._questCount.Clear();

        var emulatorQuestList = EmulatorQuestDao.GetAll(dbClient);
        if (emulatorQuestList.Count == 0)
        {
            return;
        }

        foreach (var emulatorQuest in emulatorQuestList)
        {
            var id = emulatorQuest.Id;
            var category = emulatorQuest.Category;
            var seriesNumber = emulatorQuest.SeriesNumber;
            var goalType = emulatorQuest.GoalType;
            var goalData = emulatorQuest.GoalData;
            var name = emulatorQuest.Name;
            var reward = emulatorQuest.Reward;
            var dataBit = emulatorQuest.DataBit;

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

    public Quest GetQuest(int id)
    {
        if (this._quests.TryGetValue(id, out var quest))
        {
            return quest;
        }

        return null;
    }

    public int GetAmountOfQuestsInCategory(string category)
    {
        if (this._questCount.TryGetValue(category, out var num))
        {
            return num;
        }
        return 0;
    }

    public void ProgressUserQuest(GameClient session, QuestType questType, int eventData = 0)
    {
        if (session == null || session.User == null || session.User.CurrentQuestId <= 0)
        {
            return;
        }

        var quest = this.GetQuest(session.User.CurrentQuestId);
        if (quest == null || quest.GoalType != questType)
        {
            return;
        }

        var questProgress = session.User.GetQuestProgress(quest.Id);
        var flag = false;
        int progress;
        if (questType != QuestType.ExploreFindItem)
        {
            progress = questProgress + 1;
            if (progress >= (long)quest.GoalData)
            {
                flag = true;
            }
        }
        else
        {
            if (eventData != quest.GoalData)
            {
                return;
            }

            progress = quest.GoalData;
            flag = true;
        }
        using (var dbClient = WibboEnvironment.GetDatabaseManager().Connection())
        {
            UserQuestDao.Update(dbClient, session.User.Id, quest.Id, progress);
        }

        session.User.Quests[session.User.CurrentQuestId] = progress;
        session.SendPacket(new QuestStartedComposer(session, quest));

        if (!flag)
        {
            return;
        }

        session.User.CurrentQuestId = 0;
        session.User.LastCompleted = quest.Id;
        session.SendPacket(new QuestCompletedComposer(session, quest));
        session.User.Duckets += quest.Reward;
        session.SendPacket(new ActivityPointNotificationComposer(session.User.Duckets, 1));
        this.SendQuestList(session);
    }

    public Quest GetNextQuestInSeries(string category, int number)
    {
        foreach (var quest in this._quests.Values)
        {
            if (quest.Category == category && quest.Number == number)
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
            if (!dictionary1.TryGetValue(quest.Category, out var questCategory))
            {
                dictionary1.Add(quest.Category, 1);
                dictionary2.Add(quest.Category, null);
            }
            if (quest.Number >= questCategory)
            {
                var questProgress = session.User.GetQuestProgress(quest.Id);
                if (session.User.CurrentQuestId != quest.Id && questProgress >= (long)quest.GoalData)
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

    public void ActivateQuest(GameClient session, int questId)
    {
        var quest = this.GetQuest(questId);
        if (quest == null)
        {
            return;
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().Connection())
        {
            UserQuestDao.Replace(dbClient, session.User.Id, quest.Id);
        }

        session.User.CurrentQuestId = quest.Id;
        this.SendQuestList(session);
        session.SendPacket(new QuestStartedComposer(session, quest));
    }

    public void GetCurrentQuest(GameClient session)
    {
        if (!session.User.InRoom)
        {
            return;
        }

        var quest = this.GetQuest(session.User.LastCompleted);
        var nextQuestInSeries = this.GetNextQuestInSeries(quest.Category, quest.Number + 1);
        if (nextQuestInSeries == null)
        {
            return;
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().Connection())
        {
            UserQuestDao.Replace(dbClient, session.User.Id, nextQuestInSeries.Id);
        }

        session.User.CurrentQuestId = nextQuestInSeries.Id;
        this.SendQuestList(session);
        session.SendPacket(new QuestStartedComposer(session, nextQuestInSeries));
    }

    public void CancelQuest(GameClient session)
    {
        var quest = this.GetQuest(session.User.CurrentQuestId);
        if (quest == null)
        {
            return;
        }

        session.User.CurrentQuestId = 0;
        using (var dbClient = WibboEnvironment.GetDatabaseManager().Connection())
        {
            UserQuestDao.Delete(dbClient, session.User.Id, quest.Id);
        }

        session.SendPacket(new QuestAbortedComposer());
        this.SendQuestList(session);
    }
}
