using Butterfly.Communication.Packets.Incoming;
using Butterfly.Communication.Packets.Outgoing.Inventory.Purse;
using Butterfly.Communication.Packets.Outgoing.Quests;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using System;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.Game.Quests
{
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
            Session.SendPacket(new QuestStartedComposer(Session, quest));

            if (!flag)
            {
                return;
            }

            Session.GetHabbo().CurrentQuestId = 0;
            Session.GetHabbo().LastCompleted = quest.Id;
            Session.SendPacket(new QuestCompletedComposer(Session, quest));
            Session.GetHabbo().Duckets += quest.Reward;
            Session.SendPacket(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, 1));
            this.SendQuestList(Session);
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

        public void SendQuestList(Client Session, bool send = true)
        {
            Dictionary<string, int> dictionary1 = new Dictionary<string, int>();
            Dictionary<string, Quest> dictionary2 = new Dictionary<string, Quest>();

            foreach (Quest quest in this._quests.Values)
            {
                if (!dictionary1.ContainsKey(quest.Category))
                {
                    dictionary1.Add(quest.Category, 1);
                    dictionary2.Add(quest.Category, null);
                }
                if (quest.Number >= dictionary1[quest.Category])
                {
                    int questProgress = Session.GetHabbo().GetQuestProgress(quest.Id);
                    if (Session.GetHabbo().CurrentQuestId != quest.Id && questProgress >= (long)quest.GoalData)
                    {
                        dictionary1[quest.Category] = quest.Number + 1;
                    }
                }
            }

            foreach (Quest quest in this._quests.Values)
            {
                foreach (KeyValuePair<string, int> keyValuePair in dictionary1)
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
            this.SendQuestList(Session);
            Session.SendPacket(new QuestStartedComposer(Session, quest));
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
            this.SendQuestList(Session);
            Session.SendPacket(new QuestStartedComposer(Session, nextQuestInSeries));
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
            this.SendQuestList(Session);
        }
    }
}
