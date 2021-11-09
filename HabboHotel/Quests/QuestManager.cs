using Butterfly.Communication.Packets.Incoming;
using Butterfly.Communication.Packets.Outgoing.Quests;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Quests.Composer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Butterfly.HabboHotel.Quests
{
    public class QuestManager
    {
        private Dictionary<int, Quest> quests;
        private Dictionary<string, int> questCount;

        public void Init()
        {
            this.quests = new Dictionary<int, Quest>();
            this.questCount = new Dictionary<string, int>();
            this.ReloadQuests();
        }

        public void ReloadQuests()
        {
            this.quests.Clear();

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DataTable table = EmulatorQuestDao.GetAll(dbClient);
                foreach (DataRow dataRow in table.Rows)
                {
                    int num1 = Convert.ToInt32(dataRow["id"]);
                    string str = (string)dataRow["category"];
                    int Number = Convert.ToInt32(dataRow["series_number"]);
                    int num2 = Convert.ToInt32(dataRow["goal_type"]);
                    int GoalData = Convert.ToInt32(dataRow["goal_data"]);
                    string Name = (string)dataRow["name"];
                    int Reward = Convert.ToInt32(dataRow["reward"]);
                    string DataBit = (string)dataRow["data_bit"];
                    Quest quest = new Quest(num1, str, Number, (QuestType)num2, GoalData, Name, Reward, DataBit);
                    this.quests.Add(num1, quest);
                    this.AddToCounter(str);
                }
            }
        }

        private void AddToCounter(string category)
        {
            if (this.questCount.TryGetValue(category, out int num))
            {
                this.questCount[category] = num + 1;
            }
            else
            {
                this.questCount.Add(category, 1);
            }
        }

        public Quest GetQuest(int Id)
        {
            this.quests.TryGetValue(Id, out Quest quest);

            return quest;
        }

        public int GetAmountOfQuestsInCategory(string Category)
        {
            this.questCount.TryGetValue(Category, out int num);

            return num;
        }

        public void ProgressUserQuest(GameClient Session, QuestType QuestType, int EventData = 0)
        {
            if (Session == null || Session.GetHabbo() == null || Session.GetHabbo().CurrentQuestId <= 0)
            {
                return;
            }

            Quest quest = this.GetQuest(Session.GetHabbo().CurrentQuestId);
            if (quest == null || quest.GoalType != QuestType)
            {
                return;
            }

            int questProgress = Session.GetHabbo().GetQuestProgress(quest.Id);
            bool flag = false;
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
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                UserQuestDao.Update(dbClient, Session.GetHabbo().Id, quest.Id, progress);
            }

            Session.GetHabbo().quests[Session.GetHabbo().CurrentQuestId] = progress;
            Session.SendPacket(QuestStartedComposer.Compose(Session, quest));

            if (!flag)
            {
                return;
            }

            Session.GetHabbo().CurrentQuestId = 0;
            Session.GetHabbo().LastCompleted = quest.Id;
            Session.SendPacket(QuestCompletedComposer.Compose(Session, quest));
            Session.GetHabbo().Duckets += quest.Reward;
            Session.GetHabbo().UpdateActivityPointsBalance();
            this.GetList(Session, null);
        }

        public Quest GetNextQuestInSeries(string Category, int Number)
        {
            foreach (Quest quest in this.quests.Values)
            {
                if (quest.Category == Category && quest.Number == Number)
                {
                    return quest;
                }
            }

            return null;
        }

        public void GetList(GameClient Session, ClientPacket Message)
        {
            Session.SendPacket(QuestListComposer.Compose(Session, Enumerable.ToList<Quest>(this.quests.Values), Message != null));
        }

        public void ActivateQuest(GameClient Session, ClientPacket Message)
        {
            Quest quest = this.GetQuest(Message.PopInt());
            if (quest == null)
            {
                return;
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                UserQuestDao.Replace(dbClient, Session.GetHabbo().Id, quest.Id);
            }

            Session.GetHabbo().CurrentQuestId = quest.Id;
            this.GetList(Session, null);
            Session.SendPacket(QuestStartedComposer.Compose(Session, quest));
        }

        public void GetCurrentQuest(GameClient Session)
        {
            if (!Session.GetHabbo().InRoom)
            {
                return;
            }

            Quest quest = this.GetQuest(Session.GetHabbo().LastCompleted);
            Quest nextQuestInSeries = this.GetNextQuestInSeries(quest.Category, quest.Number + 1);
            if (nextQuestInSeries == null)
            {
                return;
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                UserQuestDao.Replace(dbClient, Session.GetHabbo().Id, nextQuestInSeries.Id);
            }

            Session.GetHabbo().CurrentQuestId = nextQuestInSeries.Id;
            this.GetList(Session, null);
            Session.SendPacket(QuestStartedComposer.Compose(Session, nextQuestInSeries));
        }

        public void CancelQuest(GameClient Session)
        {
            Quest quest = this.GetQuest(Session.GetHabbo().CurrentQuestId);
            if (quest == null)
            {
                return;
            }

            Session.GetHabbo().CurrentQuestId = 0;
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                UserQuestDao.Delete(dbClient, Session.GetHabbo().Id, quest.Id);
            }

            Session.SendPacket(new QuestAbortedMessageComposer());
            this.GetList(Session, null);
        }
    }
}
