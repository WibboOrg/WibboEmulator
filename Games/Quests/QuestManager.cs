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
    private static readonly Dictionary<int, Quest> Quests = [];
    private static readonly Dictionary<string, int> QuestCount = [];

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

    public static void ProgressUserQuest(GameClient Session, QuestType questType, int eventData = 0)
    {
        if (Session == null || Session.User == null || Session.User.QuestId <= 0)
        {
            return;
        }

        var quest = GetQuest(Session.User.QuestId);
        if (quest == null || quest.GoalType != questType)
        {
            return;
        }

        var questProgress = Session.User.GetQuestProgress(quest.Id);
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
            UserQuestDao.Update(dbClient, Session.User.Id, quest.Id, progress);
        }

        Session.User.Quests[Session.User.QuestId] = progress;
        Session.SendPacket(new QuestStartedComposer(Session, quest));

        if (!flag)
        {
            return;
        }

        Session.User.QuestId = 0;
        Session.User.LastCompleted = quest.Id;
        Session.SendPacket(new QuestCompletedComposer(Session, quest));
        Session.User.Duckets += quest.Reward;
        Session.SendPacket(new ActivityPointNotificationComposer(Session.User.Duckets, 1));
        SendQuestList(Session);
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

    public static void SendQuestList(GameClient Session, bool send = true)
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
                var questProgress = Session.User.GetQuestProgress(quest.Id);
                if (Session.User.QuestId != quest.Id && questProgress >= (long)quest.GoalData)
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

        Session.SendPacket(new QuestListComposer(dictionary2, Session, send));
    }

    public static void ActivateQuest(GameClient Session, int questId)
    {
        var quest = GetQuest(questId);
        if (quest == null)
        {
            return;
        }

        using (var dbClient = DatabaseManager.Connection)
        {
            UserQuestDao.Replace(dbClient, Session.User.Id, quest.Id);
        }

        Session.User.QuestId = quest.Id;
        SendQuestList(Session);
        Session.SendPacket(new QuestStartedComposer(Session, quest));
    }

    public static void GetCurrentQuest(GameClient Session)
    {
        if (!Session.User.InRoom)
        {
            return;
        }

        var quest = GetQuest(Session.User.LastCompleted);
        var nextQuestInSeries = GetNextQuestInSeries(quest.Category, quest.Number + 1);
        if (nextQuestInSeries == null)
        {
            return;
        }

        using (var dbClient = DatabaseManager.Connection)
        {
            UserQuestDao.Replace(dbClient, Session.User.Id, nextQuestInSeries.Id);
        }

        Session.User.QuestId = nextQuestInSeries.Id;
        SendQuestList(Session);
        Session.SendPacket(new QuestStartedComposer(Session, nextQuestInSeries));
    }

    public static void CancelQuest(GameClient Session)
    {
        var quest = GetQuest(Session.User.QuestId);
        if (quest == null)
        {
            return;
        }

        Session.User.QuestId = 0;
        using (var dbClient = DatabaseManager.Connection)
        {
            UserQuestDao.Delete(dbClient, Session.User.Id, quest.Id);
        }

        Session.SendPacket(new QuestAbortedComposer());
        SendQuestList(Session);
    }
}
