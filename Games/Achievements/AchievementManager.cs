namespace WibboEmulator.Games.Achievements;
using System.Data;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Achievements;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Users.Achievements;

public class AchievementManager
{
    private readonly Dictionary<string, AchievementData> _achievements;

    public AchievementManager() => this._achievements = new Dictionary<string, AchievementData>();

    public void Init(IQueryAdapter dbClient)
    {
        this._achievements.Clear();

        var table = EmulatorAchievementDao.GetAll(dbClient);
        foreach (DataRow dataRow in table.Rows)
        {
            var Id = Convert.ToInt32(dataRow["id"]);
            var Category = (string)dataRow["category"];
            var GroupName = (string)dataRow["group_name"];

            if (!GroupName.StartsWith("ACH_"))
            {
                GroupName = "ACH_" + GroupName;
            }

            var Level = new AchievementLevel(Convert.ToInt32(dataRow["level"]), Convert.ToInt32(dataRow["reward_pixels"]), Convert.ToInt32(dataRow["reward_points"]), Convert.ToInt32(dataRow["progress_needed"]));
            if (!this._achievements.ContainsKey(GroupName))
            {
                var achievement = new AchievementData(Id, GroupName, Category);
                achievement.AddLevel(Level);
                this._achievements.Add(GroupName, achievement);
            }
            else
            {
                this._achievements[GroupName].AddLevel(Level);
            }
        }
    }

    public void GetList(GameClient session) => session.SendPacket(new AchievementsComposer(session, this._achievements.Values.ToList()));

    public bool ProgressAchievement(GameClient session, string AchievementGroup, int ProgressAmount)
    {
        if (!this._achievements.ContainsKey(AchievementGroup))
        {
            return false;
        }

        var AchievementData = this._achievements[AchievementGroup];

        var userData = session.GetUser().GetAchievementComponent().GetAchievementData(AchievementGroup);

        if (userData == null)
        {
            userData = new UserAchievement(AchievementGroup, 0, 0);
            session.GetUser().GetAchievementComponent().AddAchievement(userData);
        }

        var TotalLevels = AchievementData.Levels.Count;

        if (userData != null && userData.Level == TotalLevels)
        {
            return false;
        }

        var TargetLevel = userData != null ? userData.Level + 1 : 1;

        if (TargetLevel > TotalLevels)
        {
            TargetLevel = TotalLevels;
        }

        var TargetLevelData = AchievementData.Levels[TargetLevel];

        var NewProgress = userData != null ? userData.Progress + ProgressAmount : ProgressAmount;
        var NewLevel = userData != null ? userData.Level : 0;
        var NewTarget = NewLevel + 1;

        if (NewTarget > TotalLevels)
        {
            NewTarget = TotalLevels;
        }

        if (NewProgress >= TargetLevelData.Requirement)
        {
            NewLevel++;
            NewTarget++;

            var ProgressRemainder = NewProgress - TargetLevelData.Requirement;
            NewProgress = 0;

            session.GetUser().GetBadgeComponent().GiveBadge(AchievementGroup + TargetLevel, true);
            session.SendPacket(new ReceiveBadgeComposer(AchievementGroup + TargetLevel));

            if (NewTarget > TotalLevels)
            {
                NewTarget = TotalLevels;
            }

            session.GetUser().Duckets += TargetLevelData.RewardPixels;
            session.SendPacket(new ActivityPointNotificationComposer(session.GetUser().Duckets, 1));

            session.SendPacket(new AchievementUnlockedComposer(AchievementData, TargetLevel, TargetLevelData.RewardPoints, TargetLevelData.RewardPixels));

            using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                UserAchievementDao.Replace(dbClient, session.GetUser().Id, NewLevel, NewProgress, AchievementGroup);
                UserStatsDao.UpdateAchievementScore(dbClient, session.GetUser().Id, TargetLevelData.RewardPoints);
            }

            if (userData != null)
            {
                userData.Level = NewLevel;
                userData.Progress = NewProgress;
            }

            session.GetUser().AchievementPoints += TargetLevelData.RewardPoints;
            session.GetUser().Duckets += TargetLevelData.RewardPixels;
            session.SendPacket(new ActivityPointNotificationComposer(session.GetUser().Duckets, 1));
            session.SendPacket(new AchievementScoreComposer(session.GetUser().AchievementPoints));


            if (session.GetUser().CurrentRoom != null)
            {
                var roomUserByUserId = session.GetUser().CurrentRoom.GetRoomUserManager().GetRoomUserByUserId(session.GetUser().Id);
                if (roomUserByUserId != null)
                {
                    session.SendPacket(new UserChangeComposer(roomUserByUserId, true));
                    session.GetUser().CurrentRoom.SendPacket(new UserChangeComposer(roomUserByUserId, false));
                }
            }


            var NewLevelData = AchievementData.Levels[NewTarget];
            session.SendPacket(new AchievementProgressedComposer(AchievementData, NewTarget, NewLevelData, TotalLevels, session.GetUser().GetAchievementComponent().GetAchievementData(AchievementGroup)));

            return true;
        }
        else
        {
            if (userData != null)
            {
                userData.Level = NewLevel;
                userData.Progress = NewProgress;
            }

            using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                UserAchievementDao.Replace(dbClient, session.GetUser().Id, NewLevel, NewProgress, AchievementGroup);
            }

            session.SendPacket(new AchievementProgressedComposer(AchievementData, TargetLevel, TargetLevelData,
            TotalLevels, session.GetUser().GetAchievementComponent().GetAchievementData(AchievementGroup)));
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
