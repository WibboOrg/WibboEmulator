namespace WibboEmulator.Games.Users.Authentificator;
using System.Data;
using WibboEmulator.Core.Language;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;

public class UserFactory
{
    public static User GetUserData(IDbConnection dbClient, string sessionTicket, string ip)
    {
        var ignoreAllExpire = 0;
        var isFirstConnexionToday = false;

        var user = UserDao.GetOneByTicket(dbClient, sessionTicket);
        if (user == null)
        {
            return null;
        }

        var isBanned = BanDao.IsBanned(dbClient, user.Username, ip, user.IpLast);
        if (isBanned)
        {
            return null;
        }

        var userId = user.Id;

        var client = GameClientManager.GetClientByUserID(userId);

        if (client != null)
        {
            return null;
        }

        var lastDailyCredits = user.LastDailyCredits;
        var lastDaily = DateTime.Today.ToString("MM/dd");
        if (lastDailyCredits != lastDaily)
        {
            UserDao.UpdateLastDailyCredits(dbClient, userId, lastDaily);
            isFirstConnexionToday = true;
        }

        if (!sessionTicket.StartsWith("monticket"))
        {
            UserDao.UpdateOnline(dbClient, userId);
        }

        var dUserStats = UserStatsDao.GetOne(dbClient, userId);

        if (dUserStats == null)
        {
            UserStatsDao.Insert(dbClient, userId);
            dUserStats = UserStatsDao.GetOne(dbClient, userId);
        }

        var ignoreAll = BanDao.GetOneIgnoreAll(dbClient, userId);
        if (ignoreAll > 0)
        {
            ignoreAllExpire = ignoreAll;
        }

        return GenerateUser(user, dUserStats, isFirstConnexionToday, ignoreAllExpire);
    }

    public static User GetUserData(IDbConnection dbClient, int userId)
    {
        if (GameClientManager.GetClientByUserID(userId) != null)
        {
            return null;
        }

        var dUserInfo = UserDao.GetOne(dbClient, userId);
        if (dUserInfo == null)
        {
            return null;
        }

        var dUserStats = UserStatsDao.GetOne(dbClient, userId);

        if (dUserStats == null)
        {
            UserStatsDao.Insert(dbClient, userId);
            dUserStats = UserStatsDao.GetOne(dbClient, userId);
        }

        return GenerateUser(dUserInfo, dUserStats, false, 0);
    }

    public static User GenerateUser(UserEntity user, UserStatsEntity userStats, bool isFirstConnexionToday, int ignoreAllExpire)
    {
        var id = user.Id;
        var username = user.Username;
        var haveMail = !string.IsNullOrEmpty(user.Mail);
        var rank = user.Rank;
        var motto = user.Motto;
        var look = user.Look;
        var gender = user.Gender;
        var lastOnline = user.LastOnline;
        var credits = user.Credits;
        var wibboPoints = user.VipPoints;
        var limitCoins = user.LimitCoins;
        var activityPoints = user.ActivityPoints;
        var homeRoom = user.HomeRoom;
        var accountCreated = user.AccountCreated;
        var acceptTrading = user.AcceptTrading;
        var ip = user.IpLast;
        var hideInroom = user.HideInRoom;
        var hideOnline = user.HideOnline;
        var mazoHighScore = user.MazoScore;
        var mazo = user.Mazo;
        var gamePointsMonth = user.GamePointsMonth;
        var clientVolume = user.Volume;
        var nuxEnable = user.NuxEnable;
        var ignoreRoomInvite = user.IgnoreRoomInvite;
        var cameraFollowDisabled = user.CameraFollowDisabled;
        var hasFriendRequestsDisabled = user.BlockNewFriends;
        var langue = LanguageManager.ParseLanguage(user.Langue);
        var bannerId = user.BannerId;

        var respect = userStats.Respect;
        var dailyRespectPoints = userStats.DailyRespectPoints;
        var dailyPetRespectPoints = userStats.DailyPetRespectPoints;
        var currentQuestID = userStats.QuestId;
        var achievementPoints = userStats.AchievementScore;
        var favoriteGroup = userStats.GroupId;

        return new User(id, username, rank, motto, look, gender, credits, wibboPoints, limitCoins, activityPoints, homeRoom, respect, dailyRespectPoints, dailyPetRespectPoints, hasFriendRequestsDisabled, currentQuestID, achievementPoints, lastOnline, favoriteGroup, accountCreated, acceptTrading, ip, hideInroom, hideOnline, mazoHighScore, mazo, clientVolume, nuxEnable, isFirstConnexionToday, langue, ignoreAllExpire, ignoreRoomInvite, cameraFollowDisabled, gamePointsMonth, bannerId, haveMail);
    }
}
