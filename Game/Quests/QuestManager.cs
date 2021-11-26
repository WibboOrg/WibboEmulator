using Butterfly.Communication.Packets.Incoming;
using Butterfly.Communication.Packets.Outgoing.Quests;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Butterfly.Game.Quests
{
    public class QuestManager
    {
        private Dictionary<int, Quest> _quests;
        private Dictionary<string, int> _questCount;

        public void Init()
        {
            this._quests = new Dictionary<int, Quest>();
            this._questCount = new Dictionary<string, int>();

            this.ReloadQuests();
        }

        public void ReloadQuests()
        {
            this._quests.Clear();

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DataTable table = EmulatorQuestDao.GetAll(dbClient);
                foreach (DataRow dataRow in table.Rows)
                {
                    int id = Convert.ToInt32(dataRow["id"]);
                    string category = (string)dataRow["category"];
                    int seriesNumber = Convert.ToInt32(dataRow["series_number"]);
                    int goalType = Convert.ToInt32(dataRow["goal_type"]);
                    int goalData = Convert.ToInt32(dataRow["goal_data"]);
                    string name = (string)dataRow["name"];
                    int reward = Convert.ToInt32(dataRow["reward"]);
                    string dataBit = (string)dataRow["data_bit"];

                    this._quests.Add(id, new Quest(id, category, seriesNumber, (QuestType)goalType, goalData, name, reward, dataBit));

                    this.AddToCounter(category);
                }
            }
        }

        private void AddToCounter(string category)
        {
            if (this._questCount.TryGetValue(category, out int num))
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
            this._quests.TryGetValue(Id, out Quest quest);

            return quest;
        }

        public int GetAmountOfQuestsInCategory(string Category)
        {
            this._questCount.TryGetValue(Category, out int num);

            return num;
        }

        public void ProgressUserQuest(Client Session, QuestType QuestType, int EventData = 0)
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

            Session.GetHabbo().Quests[Session.GetHabbo().CurrentQuestId] = progress;
            Session.SendPacket(Composer.QuestStartedComposer.Compose(Session, quest));

            if (!flag)
            {
                return;
            }

            Session.GetHabbo().CurrentQuestId = 0;
            Session.GetHabbo().LastCompleted = quest.Id;
            Session.SendPacket(Composer.QuestCompletedComposer.Compose(Session, quest));
            Session.GetHabbo().Duckets += quest.Reward;
            Session.GetHabbo().UpdateActivityPointsBalance();
            this.GetList(Session, null);
        }

        public Quest GetNextQuestInSeries(string Category, int Number)
        {
            foreach (Quest quest in this._quests.Values)
            {
                if (quest.Category == Category && quest.Number == Number)
                {
                    return quest;
                }
            }

            return null;
        }

        public void GetList(Client Session, ClientPacket Message)
        {
            Session.SendPacket(Composer.QuestListComposer.Compose(Session, Enumerable.ToList<Quest>(this._quests.Values), Message != null));
        }

        public void ActivateQuest(Client Session, ClientPacket Message)
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
            Session.SendPacket(Composer.QuestStartedComposer.Compose(Session, quest));
        }

        public void GetCurrentQuest(Client Session)
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
            Session.SendPacket(Composer.QuestStartedComposer.Compose(Session, nextQuestInSeries));
        }

        public void CancelQuest(Client Session)
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

            Session.SendPacket(new QuestAbortedComposer());
            this.GetList(Session, null);
        }
    }
}
