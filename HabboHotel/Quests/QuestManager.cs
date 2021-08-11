using Butterfly.Communication.Packets.Incoming;
using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.Avatar;
using Butterfly.Communication.Packets.Outgoing.BuildersClub;
using Butterfly.Communication.Packets.Outgoing.Camera;
using Butterfly.Communication.Packets.Outgoing.Catalog;
using Butterfly.Communication.Packets.Outgoing.GameCenter;
using Butterfly.Communication.Packets.Outgoing.Groups;
using Butterfly.Communication.Packets.Outgoing.Handshake;
using Butterfly.Communication.Packets.Outgoing.Help;
using Butterfly.Communication.Packets.Outgoing.Inventory;
using Butterfly.Communication.Packets.Outgoing.Inventory.Achievements;
using Butterfly.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using Butterfly.Communication.Packets.Outgoing.Inventory.Badges;
using Butterfly.Communication.Packets.Outgoing.Inventory.Bots;
using Butterfly.Communication.Packets.Outgoing.Inventory.Furni;
using Butterfly.Communication.Packets.Outgoing.Inventory.Pets;
using Butterfly.Communication.Packets.Outgoing.Inventory.Purse;
using Butterfly.Communication.Packets.Outgoing.Inventory.Trading;
using Butterfly.Communication.Packets.Outgoing.LandingView;
using Butterfly.Communication.Packets.Outgoing.MarketPlace;
using Butterfly.Communication.Packets.Outgoing.Messenger;
using Butterfly.Communication.Packets.Outgoing.Misc;
using Butterfly.Communication.Packets.Outgoing.Moderation;
using Butterfly.Communication.Packets.Outgoing.Navigator;
using Butterfly.Communication.Packets.Outgoing.Navigator.New;
using Butterfly.Communication.Packets.Outgoing.Notifications;
using Butterfly.Communication.Packets.Outgoing.Pets;
using Butterfly.Communication.Packets.Outgoing.Quests;
using Butterfly.Communication.Packets.Outgoing.Rooms;
using Butterfly.Communication.Packets.Outgoing.Rooms.Action;
using Butterfly.Communication.Packets.Outgoing.Rooms.AI.Bots;
using Butterfly.Communication.Packets.Outgoing.Rooms.AI.Pets;
using Butterfly.Communication.Packets.Outgoing.Rooms.Avatar;
using Butterfly.Communication.Packets.Outgoing.Rooms.Chat;
using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Communication.Packets.Outgoing.Rooms.FloorPlan;
using Butterfly.Communication.Packets.Outgoing.Rooms.Freeze;
using Butterfly.Communication.Packets.Outgoing.Rooms.Furni;
using Butterfly.Communication.Packets.Outgoing.Rooms.Furni.Furni;
using Butterfly.Communication.Packets.Outgoing.Rooms.Furni.Moodlight;
using Butterfly.Communication.Packets.Outgoing.Rooms.Furni.Stickys;
using Butterfly.Communication.Packets.Outgoing.Rooms.Furni.Wired;
using Butterfly.Communication.Packets.Outgoing.Rooms.Notifications;
using Butterfly.Communication.Packets.Outgoing.Rooms.Permissions;
using Butterfly.Communication.Packets.Outgoing.Rooms.Session;
using Butterfly.Communication.Packets.Outgoing.Rooms.Settings;
using Butterfly.Communication.Packets.Outgoing.Rooms.Wireds;
using Butterfly.Communication.Packets.Outgoing.Sound;
using Butterfly.Communication.Packets.Outgoing.Users;
using Butterfly.Communication.Packets.Outgoing.WebSocket;
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
                dbClient.SetQuery("SELECT * FROM quests");
                foreach (DataRow dataRow in dbClient.GetTable().Rows)
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
            int num;
            if (QuestType != QuestType.EXPLORE_FIND_ITEM)
            {
                num = questProgress + 1;
                if (num >= (long)quest.GoalData)
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

                num = quest.GoalData;
                flag = true;
            }
            using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.RunQuery("UPDATE user_quests SET progress = " + num + " WHERE user_id = " + Session.GetHabbo().Id + " AND quest_id =  " + quest.Id);
            }

            Session.GetHabbo().quests[Session.GetHabbo().CurrentQuestId] = num;
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

            using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.RunQuery("REPLACE INTO user_quests VALUES (" + Session.GetHabbo().Id + ", " + quest.Id + ", 0)");
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

            using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.RunQuery("REPLACE INTO user_quests VALUES (" + Session.GetHabbo().Id + ", " + nextQuestInSeries.Id + ", 0)");
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
            using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.RunQuery("DELETE FROM user_quests WHERE user_id = '" + Session.GetHabbo().Id + "' AND quest_id = '" + quest.Id + "';");
            }

            Session.SendPacket(new QuestAbortedMessageComposer());
            this.GetList(Session, null);
        }
    }
}
