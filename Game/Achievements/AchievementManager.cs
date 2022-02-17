using Butterfly.Communication.Packets.Outgoing.Inventory.Achievements;
using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Communication.Packets.Outgoing.Users;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Butterfly.Game.Users.Achievements;
using Butterfly.Communication.Packets.Outgoing.Inventory.Purse;

namespace Butterfly.Game.Achievements
{
    public class AchievementManager
    {
        private readonly Dictionary<string, AchievementData> _achievements;

        public AchievementManager()
        {
            this._achievements = new Dictionary<string, AchievementData>();
        }

        public void Init()
        {
            this._achievements.Clear();

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DataTable table = EmulatorAchievementDao.GetAll(dbClient);
                foreach (DataRow dataRow in table.Rows)
                {
                    int Id = Convert.ToInt32(dataRow["id"]);
                    string Category = (string)dataRow["category"];
                    string GroupName = (string)dataRow["group_name"];

                    if (!GroupName.StartsWith("ACH_"))
                    {
                        GroupName = "ACH_" + GroupName;
                    }

                    AchievementLevel Level = new AchievementLevel(Convert.ToInt32(dataRow["level"]), Convert.ToInt32(dataRow["reward_pixels"]), Convert.ToInt32(dataRow["reward_points"]), Convert.ToInt32(dataRow["progress_needed"]));
                    if (!this._achievements.ContainsKey(GroupName))
                    {
                        AchievementData achievement = new AchievementData(Id, GroupName, Category);
                        achievement.AddLevel(Level);
                        this._achievements.Add(GroupName, achievement);
                    }
                    else
                    {
                        this._achievements[GroupName].AddLevel(Level);
                    }
                }
            }
        }

        public void GetList(Client Session)
        {
            Session.SendPacket(new AchievementsComposer(Session, this._achievements.Values.ToList()));
        }

        public bool ProgressAchievement(Client Session, string AchievementGroup, int ProgressAmount)
        {
            if (!this._achievements.ContainsKey(AchievementGroup))
            {
                return false;
            }

            AchievementData AchievementData = this._achievements[AchievementGroup];

            UserAchievement UserData = Session.GetHabbo().GetAchievementComponent().GetAchievementData(AchievementGroup);

            if (UserData == null)
            {
                UserData = new UserAchievement(AchievementGroup, 0, 0);
                Session.GetHabbo().GetAchievementComponent().AddAchievement(UserData);
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
                Session.SendPacket(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, 1));

                Session.SendPacket(new AchievementUnlockedComposer(AchievementData, TargetLevel, TargetLevelData.RewardPoints, TargetLevelData.RewardPixels));

                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    UserAchievementDao.Replace(dbClient, Session.GetHabbo().Id, NewLevel, NewProgress, AchievementGroup);
                    UserStatsDao.UpdateAchievementScore(dbClient, Session.GetHabbo().Id, TargetLevelData.RewardPoints);
                }


                UserData.Level = NewLevel;
                UserData.Progress = NewProgress;

                Session.GetHabbo().AchievementPoints += TargetLevelData.RewardPoints;
                Session.GetHabbo().Duckets += TargetLevelData.RewardPixels;
                Session.SendPacket(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, 1));
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
                Session.SendPacket(new AchievementProgressedComposer(AchievementData, NewTarget, NewLevelData, TotalLevels, Session.GetHabbo().GetAchievementComponent().GetAchievementData(AchievementGroup)));

                return true;
            }
            else
            {
                UserData.Level = NewLevel;
                UserData.Progress = NewProgress;
                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    UserAchievementDao.Replace(dbClient, Session.GetHabbo().Id, NewLevel, NewProgress, AchievementGroup);
                }

                Session.SendPacket(new AchievementProgressedComposer(AchievementData, TargetLevel, TargetLevelData,
                TotalLevels, Session.GetHabbo().GetAchievementComponent().GetAchievementData(AchievementGroup)));
            }

            return false;
        }

        public AchievementData GetAchievement(string AchievementGroup)
        {
            if (this._achievements.ContainsKey(AchievementGroup))
            {
                return this._achievements[AchievementGroup];
            }

            return null;
        }
    }
}
