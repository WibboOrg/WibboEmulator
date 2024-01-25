namespace WibboEmulator.Games.Achievements;
using System.Data;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Achievements;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Database.Daos.Emulator;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Users.Achievements;

public class AchievementManager
{
    private readonly Dictionary<string, AchievementData> _achievements;

    public AchievementManager() => this._achievements = new Dictionary<string, AchievementData>();

    public void Init(IDbConnection dbClient)
    {
        this._achievements.Clear();

        var emulatorAchievementList = EmulatorAchievementDao.GetAll(dbClient);

        if (emulatorAchievementList.Count == 0)
        {
            return;
        }

        foreach (var emulatorAchievement in emulatorAchievementList)
        {
            var id = emulatorAchievement.Id;
            var category = emulatorAchievement.Category;
            var groupName = emulatorAchievement.GroupName;

            if (!groupName.StartsWith("ACH_"))
            {
                groupName = "ACH_" + groupName;
            }

            var level = new AchievementLevel(emulatorAchievement.Level, emulatorAchievement.RewardPixels, emulatorAchievement.RewardPoints, emulatorAchievement.ProgressNeeded);
            if (!this._achievements.ContainsKey(groupName))
            {
                var achievement = new AchievementData(id, groupName, category);
                achievement.AddLevel(level);
                this._achievements.Add(groupName, achievement);
            }
            else
            {
                this._achievements[groupName].AddLevel(level);
            }
        }
    }

    public void GetList(GameClient session) => session.SendPacket(new AchievementsComposer(session, this._achievements.Values.ToList()));

    public bool ProgressAchievement(GameClient session, string achievementGroup, int progressAmount)
    {
        if (!this._achievements.TryGetValue(achievementGroup, out var achievementData))
        {
            return false;
        }

        var userData = session.User.AchievementComponent.GetAchievementData(achievementGroup);

        if (userData == null)
        {
            userData = new UserAchievement(achievementGroup, 0, 0);
            session.User.AchievementComponent.AddAchievement(userData);
        }

        var totalLevels = achievementData.Levels.Count;

        if (userData != null && userData.Level == totalLevels)
        {
            return false;
        }

        var targetLevel = userData != null ? userData.Level + 1 : 1;

        if (targetLevel > totalLevels)
        {
            targetLevel = totalLevels;
        }

        var targetLevelData = achievementData.Levels[targetLevel];

        var newProgress = userData != null ? userData.Progress + progressAmount : progressAmount;
        var newLevel = userData != null ? userData.Level : 0;
        var newTarget = newLevel + 1;

        if (newTarget > totalLevels)
        {
            newTarget = totalLevels;
        }

        if (newProgress >= targetLevelData.Requirement)
        {
            newLevel++;
            newTarget++;

            _ = newProgress - targetLevelData.Requirement;
            newProgress = 0;

            session.User.BadgeComponent.GiveBadge(achievementGroup + targetLevel);

            if (newTarget > totalLevels)
            {
                newTarget = totalLevels;
            }

            session.User.Duckets += targetLevelData.RewardPixels;
            session.SendPacket(new ActivityPointNotificationComposer(session.User.Duckets, 1));

            session.SendPacket(new AchievementUnlockedComposer(achievementData, targetLevel, targetLevelData.RewardPoints, targetLevelData.RewardPixels));

            using (var dbClient = WibboEnvironment.GetDatabaseManager().Connection())
            {
                UserAchievementDao.Replace(dbClient, session.User.Id, newLevel, newProgress, achievementGroup);
                UserStatsDao.UpdateAchievementScore(dbClient, session.User.Id, targetLevelData.RewardPoints);
            }

            if (userData != null)
            {
                userData.Level = newLevel;
                userData.Progress = newProgress;
            }

            session.User.AchievementPoints += targetLevelData.RewardPoints;
            session.User.Duckets += targetLevelData.RewardPixels;
            session.SendPacket(new ActivityPointNotificationComposer(session.User.Duckets, 1));
            session.SendPacket(new AchievementScoreComposer(session.User.AchievementPoints));


            if (session.User.CurrentRoom != null)
            {
                var roomUserByUserId = session.User.CurrentRoom.RoomUserManager.GetRoomUserByUserId(session.User.Id);
                if (roomUserByUserId != null)
                {
                    session.SendPacket(new UserChangeComposer(roomUserByUserId, true));
                    session.User.CurrentRoom.SendPacket(new UserChangeComposer(roomUserByUserId, false));
                }
            }


            var newLevelData = achievementData.Levels[newTarget];
            session.SendPacket(new AchievementProgressedComposer(achievementData, newTarget, newLevelData, totalLevels, session.User.AchievementComponent.GetAchievementData(achievementGroup)));

            return true;
        }
        else
        {
            if (userData != null)
            {
                userData.Level = newLevel;
                userData.Progress = newProgress;
            }

            using (var dbClient = WibboEnvironment.GetDatabaseManager().Connection())
            {
                UserAchievementDao.Replace(dbClient, session.User.Id, newLevel, newProgress, achievementGroup);
            }

            session.SendPacket(new AchievementProgressedComposer(achievementData, targetLevel, targetLevelData,
            totalLevels, session.User.AchievementComponent.GetAchievementData(achievementGroup)));
        }

        return false;
    }

    public AchievementData GetAchievement(string achievementGroup)
    {
        if (this._achievements.TryGetValue(achievementGroup, out var value))
        {
            return value;
        }

        return null;
    }
}
