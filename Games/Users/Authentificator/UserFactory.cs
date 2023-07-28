namespace WibboEmulator.Games.Users.Authentificator;
using System.Data;
using WibboEmulator.Core;
using WibboEmulator.Core.Language;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Database.Interfaces;

public class UserFactory
{
    public static User GetUserData(IQueryAdapter dbClient, string sessionTicket, string ip, string machineid)
    {
        try
        {
            int userId;
            DataRow dUserInfo;
            DataRow dUserStats;
            var ignoreAllExpire = 0;
            var isFirstConnexionToday = false;

            dUserInfo = UserDao.GetOneByTicket(dbClient, sessionTicket);
            if (dUserInfo == null)
            {
                return null;
            }

            var isBanned = BanDao.IsBanned(dbClient, dUserInfo["username"].ToString(), ip, dUserInfo["ip_last"].ToString(), machineid);
            if (isBanned)
            {
                return null;
            }

            userId = Convert.ToInt32(dUserInfo["id"]);

            var client = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(userId);

            if (client != null)
            {
                return null;
            }

            var lastDailyCredits = (string)dUserInfo["lastdailycredits"];
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

            dUserStats = UserStatsDao.GetOne(dbClient, userId);

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

            return GenerateUser(dUserInfo, dUserStats, isFirstConnexionToday, ignoreAllExpire);
        }
        catch (Exception ex)
        {
            ExceptionLogger.HandleException(ex, "UserDataFactory.GetUserData");
            return null;
        }
    }

    public static User GetUserData(int userId)
    {
        DataRow dUserInfo;
        DataRow dUserStats;

        if (WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(userId) != null)
        {
            return null;
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            dUserInfo = UserDao.GetOne(dbClient, userId);
            if (dUserInfo == null)
            {
                return null;
            }

            dUserStats = UserStatsDao.GetOne(dbClient, userId);

            if (dUserStats == null)
            {
                UserStatsDao.Insert(dbClient, userId);
                dUserStats = UserStatsDao.GetOne(dbClient, userId);
            }
        }

        return GenerateUser(dUserInfo, dUserStats, false, 0);
    }

    public static User GenerateUser(DataRow dRow, DataRow dRow2, bool isFirstConnexionToday, int ignoreAllExpire)
    {
        var id = Convert.ToInt32(dRow["id"]);
        var username = (string)dRow["username"];
        var rank = Convert.ToInt32(dRow["rank"]);
        var motto = (string)dRow["motto"];
        var look = (string)dRow["look"];
        var gender = (string)dRow["gender"];
        var lastOnline = Convert.ToInt32(dRow["last_online"]);
        var credits = Convert.ToInt32(dRow["credits"]);
        var wibboPoints = Convert.ToInt32(dRow["vip_points"]);
        var limitCoins = Convert.ToInt32(dRow["limit_coins"]);
        var activityPoints = Convert.ToInt32(dRow["activity_points"]);
        var homeRoom = Convert.ToInt32(dRow["home_room"]);
        var accountCreated = Convert.ToInt32(dRow["account_created"]);
        var acceptTrading = Convert.ToBoolean(dRow["accept_trading"]);
        var ip = dRow["ip_last"].ToString();
        var hideInroom = Convert.ToBoolean(dRow["hide_inroom"]);
        var hideOnline = Convert.ToBoolean(dRow["hide_online"]);
        var mazoHighScore = Convert.ToInt32(dRow["mazoscore"]);
        var mazo = Convert.ToInt32(dRow["mazo"]);
        var gamePointsMonth = Convert.ToInt32(dRow["game_points_month"]);
        var clientVolume = (string)dRow["volume"];
        var nuxEnable = Convert.ToBoolean(dRow["nux_enable"]);
        var ignoreRoomInvite = Convert.ToBoolean(dRow["ignore_room_invite"]);
        var cameraFollowDisabled = Convert.ToBoolean(dRow["camera_follow_disabled"]);
        var machineId = (string)dRow["machine_id"];
        var langue = LanguageManager.ParseLanguage((string)dRow["langue"]);
        var bannerId = Convert.ToInt32(dRow["banner_id"]);

        var respect = Convert.ToInt32(dRow2["respect"]);
        var dailyRespectPoints = Convert.ToInt32(dRow2["daily_respect_points"]);
        var dailyPetRespectPoints = Convert.ToInt32(dRow2["daily_pet_respect_points"]);
        var hasFriendRequestsDisabled = Convert.ToBoolean(dRow["block_newfriends"]);
        var currentQuestID = Convert.ToInt32(dRow2["quest_id"]);
        var achievementPoints = Convert.ToInt32(dRow2["achievement_score"]);
        var favoriteGroup = Convert.ToInt32(dRow2["group_id"]);

        return new User(id, username, rank, motto, look, gender, credits, wibboPoints, limitCoins, activityPoints, homeRoom, respect, dailyRespectPoints, dailyPetRespectPoints, hasFriendRequestsDisabled, currentQuestID, achievementPoints, lastOnline, favoriteGroup, accountCreated, acceptTrading, ip, hideInroom, hideOnline, mazoHighScore, mazo, clientVolume, nuxEnable, machineId, isFirstConnexionToday, langue, ignoreAllExpire, ignoreRoomInvite, cameraFollowDisabled, gamePointsMonth, bannerId);
    }
}
