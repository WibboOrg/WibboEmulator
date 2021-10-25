using Butterfly.Communication.Packets.Outgoing.Inventory.Achievements;
using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Communication.Packets.Outgoing.Users;

using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Data;

namespace Butterfly.HabboHotel.Achievements
{
    public class AchievementManager
    {
        public Dictionary<string, Achievement> Achievements;

        public AchievementManager()
        {
            this.Achievements = new Dictionary<string, Achievement>();
            this.LoadAchievements();
        }

        public void LoadAchievements()
        {
            this.Achievements.Clear();

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT id, category, group_name, level, reward_pixels, reward_points, progress_needed FROM achievements");
                foreach (DataRow dataRow in dbClient.GetTable().Rows)
                {
                    int Id = Convert.ToInt32(dataRow["id"]);
                    string Category = (string)dataRow["category"];
                    string GroupName = (string)dataRow["group_name"];

                    if (!GroupName.StartsWith("ACH_"))
                    {
                        GroupName = "ACH_" + GroupName;
                    }

                    AchievementLevel Level = new AchievementLevel(Convert.ToInt32(dataRow["level"]), Convert.ToInt32(dataRow["reward_pixels"]), Convert.ToInt32(dataRow["reward_points"]), Convert.ToInt32(dataRow["progress_needed"]));
                    if (!this.Achievements.ContainsKey(GroupName))
                    {
                        Achievement achievement = new Achievement(Id, GroupName, Category);
                        achievement.AddLevel(Level);
                        this.Achievements.Add(GroupName, achievement);
                    }
                    else
                    {
                        this.Achievements[GroupName].AddLevel(Level);
                    }
                }
            }
        }

        public void GetList(GameClient Session)
        {
            Session.SendPacket(new AchievementsMessageComposer(Session, this.Achievements.Values.ToList()));
        }

        public bool ProgressAchievement(GameClient Session, string AchievementGroup, int ProgressAmount)
        {
            if (!this.Achievements.ContainsKey(AchievementGroup))
            {
                return false;
            }

            Achievement AchievementData = null;

            AchievementData = this.Achievements[AchievementGroup];

            UserAchievement UserData = Session.GetHabbo().GetAchievementData(AchievementGroup);

            if (UserData == null)
            {
                UserData = new UserAchievement(AchievementGroup, 0, 0);
                Session.GetHabbo().Achievements.Add(AchievementGroup, UserData);
            }

            int TotalLevels = AchievementData.Levels.Count;

            if (UserData != null && UserData.Level == TotalLevels)
            {
                return false;
            }

            int TargetLevel = (UserData != null ? UserData.Level + 1 : 1);

            if (TargetLevel > TotalLevels)
            {
                TargetLevel = TotalLevels;
            }

            AchievementLevel TargetLevelData = AchievementData.Levels[TargetLevel];

            int NewProgress = (UserData != null ? UserData.Progress + ProgressAmount : ProgressAmount);
            int NewLevel = (UserData != null ? UserData.Level : 0);
            int NewTarget = NewLevel + 1;

            if (NewTarget > TotalLevels)
            {
                NewTarget = TotalLevels;
            }

            if (NewProgress >= TargetLevelData.Requirement)
            {
                NewLevel++;
                NewTarget++;

                int ProgressRemainder = NewProgress - TargetLevelData.Requirement;
                NewProgress = 0;

                Session.GetHabbo().GetBadgeComponent().GiveBadge(AchievementGroup + TargetLevel, true);
                Session.SendPacket(new ReceiveBadgeComposer(AchievementGroup + TargetLevel));

                if (NewTarget > TotalLevels)
                {
                    NewTarget = TotalLevels;
                }

                Session.GetHabbo().Duckets += TargetLevelData.RewardPixels;
                Session.GetHabbo().UpdateActivityPointsBalance();

                Session.SendPacket(new AchievementUnlockedMessageComposer(AchievementData, TargetLevel, TargetLevelData.RewardPoints, TargetLevelData.RewardPixels));

                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("REPLACE INTO user_achievement VALUES ('" + Session.GetHabbo().Id + "', @group, '" + NewLevel + "', '" + NewProgress + "')");
                    dbClient.AddParameter("group", AchievementGroup);
                    dbClient.RunQuery();
                    dbClient.RunQuery("UPDATE user_stats SET achievement_score = achievement_score + '" + TargetLevelData.RewardPoints + "' WHERE id = '" + Session.GetHabbo().Id + "'");
                }


                UserData.Level = NewLevel;
                UserData.Progress = NewProgress;

                Session.GetHabbo().AchievementPoints += TargetLevelData.RewardPoints;
                Session.GetHabbo().Duckets += TargetLevelData.RewardPixels;
                Session.GetHabbo().UpdateActivityPointsBalance();
                Session.SendPacket(new AchievementScoreComposer(Session.GetHabbo().AchievementPoints));


                if (Session.GetHabbo().CurrentRoom != null)
                {
                    RoomUser roomUserByHabbo = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
                    if (roomUserByHabbo != null)
                    {
                        Session.SendPacket(new UserChangeComposer(roomUserByHabbo, true));
                        Session.GetHabbo().CurrentRoom.SendPacket(new UserChangeComposer(roomUserByHabbo, false));
                    }
                }


                AchievementLevel NewLevelData = AchievementData.Levels[NewTarget];
                Session.SendPacket(new AchievementProgressedComposer(AchievementData, NewTarget, NewLevelData, TotalLevels, Session.GetHabbo().GetAchievementData(AchievementGroup)));

                return true;
            }
            else
            {
                UserData.Level = NewLevel;
                UserData.Progress = NewProgress;
                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("REPLACE INTO user_achievement VALUES ('" + Session.GetHabbo().Id + "', @group, '" + NewLevel + "', '" + NewProgress + "')");
                    dbClient.AddParameter("group", AchievementGroup);
                    dbClient.RunQuery();
                }

                Session.SendPacket(new AchievementProgressedComposer(AchievementData, TargetLevel, TargetLevelData,
                TotalLevels, Session.GetHabbo().GetAchievementData(AchievementGroup)));
            }

            return false;
        }

        public Achievement GetAchievement(string AchievementGroup)
        {
            if (this.Achievements.ContainsKey(AchievementGroup))
            {
                return this.Achievements[AchievementGroup];
            }

            return null;
        }
    }
}
