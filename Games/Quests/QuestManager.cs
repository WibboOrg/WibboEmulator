namespace WibboEmulator.Games.Quests;
using System.Data;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Communication.Packets.Outgoing.Quests;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Emulator;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;

public static class QuestManager
{
    private static readonly Dictionary<int, Quest> Quests = new();
    private static readonly Dictionary<string, int> QuestCount = new();

    public static void Initialize(IDbConnection dbClient)
    {
        Quests.Clear();
        QuestCount.Clear();

        var emulatorQuestList = EmulatorQuestDao.GetAll(dbClient);

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

            Quests.Add(id, new Quest(id, category, seriesNumber, (QuestType)goalType, goalData, name, reward, dataBit));

            AddToCounter(category);
        }
    }

    private static void AddToCounter(string category)
    {
        if (QuestCount.TryGetValue(category, out var num))
        {
            QuestCount[category] = num + 1;
        }
        else
        {
            QuestCount.Add(category, 1);
        }
    }

    public static Quest GetQuest(int id)
    {
        if (Quests.TryGetValue(id, out var quest))
        {
            return quest;
        }

        return null;
    }

    public static int GetAmountOfQuestsInCategory(string category)
    {
        if (QuestCount.TryGetValue(category, out var num))
        {
            return num;
        }
        return 0;
    }

    public static void ProgressUserQuest(GameClient session, QuestType questType, int eventData = 0)
    {
        if (session == null || session.User == null || session.User.QuestId <= 0)
        {
            return;
        }

        var quest = GetQuest(session.User.QuestId);
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
        using (var dbClient = DatabaseManager.Connection)
        {
            UserQuestDao.Update(dbClient, session.User.Id, quest.Id, progress);
        }

        session.User.Quests[session.User.QuestId] = progress;
        session.SendPacket(new QuestStartedComposer(session, quest));

        if (!flag)
        {
            return;
        }

        session.User.QuestId = 0;
        session.User.LastCompleted = quest.Id;
        session.SendPacket(new QuestCompletedComposer(session, quest));
        session.User.Duckets += quest.Reward;
        session.SendPacket(new ActivityPointNotificationComposer(session.User.Duckets, 1));
        SendQuestList(session);
    }

    public static Quest GetNextQuestInSeries(string category, int number)
    {
        foreach (var quest in Quests.Values)
        {
            if (quest.Category == category && quest.Number == number)
            {
                return quest;
            }
        }

        return null;
    }

    public static void SendQuestList(GameClient session, bool send = true)
    {
        var dictionary1 = new Dictionary<string, int>();
        var dictionary2 = new Dictionary<string, Quest>();

        foreach (var quest in Quests.Values)
        {
            if (!dictionary1.TryGetValue(quest.Category, out var questCategory))
            {
                dictionary1.Add(quest.Category, 1);
                dictionary2.Add(quest.Category, null);
            }
            if (quest.Number >= questCategory)
            {
                var questProgress = session.User.GetQuestProgress(quest.Id);
                if (session.User.QuestId != quest.Id && questProgress >= (long)quest.GoalData)
                {
                    dictionary1[quest.Category] = quest.Number + 1;
                }
            }
        }

        foreach (var quest in Quests.Values)
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

    public static void ActivateQuest(GameClient session, int questId)
    {
        var quest = GetQuest(questId);
        if (quest == null)
        {
            return;
        }

        using (var dbClient = DatabaseManager.Connection)
        {
            UserQuestDao.Replace(dbClient, session.User.Id, quest.Id);
        }

        session.User.QuestId = quest.Id;
        SendQuestList(session);
        session.SendPacket(new QuestStartedComposer(session, quest));
    }

    public static void GetCurrentQuest(GameClient session)
    {
        if (!session.User.InRoom)
        {
            return;
        }

        var quest = GetQuest(session.User.LastCompleted);
        var nextQuestInSeries = GetNextQuestInSeries(quest.Category, quest.Number + 1);
        if (nextQuestInSeries == null)
        {
            return;
        }

        using (var dbClient = DatabaseManager.Connection)
        {
            UserQuestDao.Replace(dbClient, session.User.Id, nextQuestInSeries.Id);
        }

        session.User.QuestId = nextQuestInSeries.Id;
        SendQuestList(session);
        session.SendPacket(new QuestStartedComposer(session, nextQuestInSeries));
    }

    public static void CancelQuest(GameClient session)
    {
        var quest = GetQuest(session.User.QuestId);
        if (quest == null)
        {
            return;
        }

        session.User.QuestId = 0;
        using (var dbClient = DatabaseManager.Connection)
        {
            UserQuestDao.Delete(dbClient, session.User.Id, quest.Id);
        }

        session.SendPacket(new QuestAbortedComposer());
        SendQuestList(session);
    }
}
