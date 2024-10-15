namespace WibboEmulator.Games.Achievements;
using System.Data;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Achievements;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Emulator;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Users.Achievements;

public static class AchievementManager
{
    private static readonly Dictionary<string, AchievementData> Achievements = [];

    public static void Initialize(IDbConnection dbClient)
    {
        Achievements.Clear();

        var emulatorAchievementList = EmulatorAchievementDao.GetAll(dbClient);

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
            if (!Achievements.TryGetValue(groupName, out var value))
            {
                var achievement = new AchievementData(id, groupName, category);
                achievement.AddLevel(level);
                Achievements.Add(groupName, achievement);
            }
            else
            {
                value.AddLevel(level);
            }
        }
    }

    public static void GetList(GameClient Session) => Session.SendPacket(new AchievementsComposer(Session, [.. Achievements.Values]));

    public static bool ProgressAchievement(GameClient Session, string achievementGroup, int progressAmount)
    {
        if (!Achievements.TryGetValue(achievementGroup, out var achievementData))
        {
            return false;
        }

        var userData = Session.User.AchievementComponent.GetAchievementData(achievementGroup);

        if (userData == null)
        {
            userData = new UserAchievement(achievementGroup, 0, 0);
            Session.User.AchievementComponent.AddAchievement(userData);
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

            Session.User.BadgeComponent.GiveBadge(achievementGroup + targetLevel);

            if (newTarget > totalLevels)
            {
                newTarget = totalLevels;
            }

            Session.User.Duckets += targetLevelData.RewardPixels;
            Session.SendPacket(new ActivityPointNotificationComposer(Session.User.Duckets, 1));

            Session.SendPacket(new AchievementUnlockedComposer(achievementData, targetLevel, targetLevelData.RewardPoints, targetLevelData.RewardPixels));

            using (var dbClient = DatabaseManager.Connection)
            {
                UserAchievementDao.Replace(dbClient, Session.User.Id, newLevel, newProgress, achievementGroup);
                UserStatsDao.UpdateAchievementScore(dbClient, Session.User.Id, targetLevelData.RewardPoints);
            }

            if (userData != null)
            {
                userData.Level = newLevel;
                userData.Progress = newProgress;
            }

            Session.User.AchievementPoints += targetLevelData.RewardPoints;
            Session.User.Duckets += targetLevelData.RewardPixels;
            Session.SendPacket(new ActivityPointNotificationComposer(Session.User.Duckets, 1));
            Session.SendPacket(new AchievementScoreComposer(Session.User.AchievementPoints));


            if (Session.User.Room != null)
            {
                var roomUserByUserId = Session.User.Room.RoomUserManager.GetRoomUserByUserId(Session.User.Id);
                if (roomUserByUserId != null)
                {
                    Session.SendPacket(new UserChangeComposer(roomUserByUserId, true));
                    Session.User.Room.SendPacket(new UserChangeComposer(roomUserByUserId, false));
                }
            }


            var newLevelData = achievementData.Levels[newTarget];
            Session.SendPacket(new AchievementProgressedComposer(achievementData, newTarget, newLevelData, totalLevels, Session.User.AchievementComponent.GetAchievementData(achievementGroup)));

            return true;
        }
        else
        {
            if (userData != null)
            {
                userData.Level = newLevel;
                userData.Progress = newProgress;
            }

            using (var dbClient = DatabaseManager.Connection)
            {
                UserAchievementDao.Replace(dbClient, Session.User.Id, newLevel, newProgress, achievementGroup);
            }

            Session.SendPacket(new AchievementProgressedComposer(achievementData, targetLevel, targetLevelData,
            totalLevels, Session.User.AchievementComponent.GetAchievementData(achievementGroup)));
        }

        return false;
    }

    public static AchievementData GetAchievement(string achievementGroup)
    {
        if (Achievements.TryGetValue(achievementGroup, out var value))
        {
            return value;
        }

        return null;
    }
}
