namespace WibboEmulator.Games.Users.Authentificator;
using System.Data;
using WibboEmulator.Core;
using WibboEmulator.Database.Daos;

public class UserFactory
{
    public static User GetUserData(string sessionTicket, string ip, string machineid)
    {
        try
        {
            int userId;
            DataRow dUserInfo;
            DataRow dUserStats;
            double ignoreAllExpire = 0;
            var changeName = false;

            using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
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

                var ignoreAll = BanDao.GetOneIgnoreAll(dbClient, dUserInfo["username"].ToString());
                if (ignoreAll > 0)
                {
                    ignoreAllExpire = ignoreAll;
                }

                userId = Convert.ToInt32(dUserInfo["id"]);
                var username = (string)dUserInfo["username"];
                if (WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(userId) != null)
                {
                    WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(userId).Disconnect();
                    return null;
                }

                var lastDailyCredits = (string)dUserInfo["lastdailycredits"];
                var lastDaily = DateTime.Today.ToString("MM/dd");
                if (lastDailyCredits != lastDaily)
                {
                    UserDao.UpdateLastDailyCredits(dbClient, userId, lastDaily);
                    dUserInfo["credits"] = Convert.ToInt32(dUserInfo["credits"]) + 3000;

                    if (Convert.ToInt32(dUserInfo["rank"]) <= 1)
                    {
                        UserStatsDao.UpdateRespectPoint(dbClient, userId, 5);
                    }
                    else
                    {
                        UserStatsDao.UpdateRespectPoint(dbClient, userId, 20);
                    }

                    changeName = true;
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
            }

            return GenerateUser(dUserInfo, dUserStats, changeName, ignoreAllExpire);
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

    public static User GenerateUser(DataRow dRow, DataRow dRow2, bool ChangeName, double ignoreAllExpire)
    {
        var id = Convert.ToInt32(dRow["id"]);
        var username = (string)dRow["username"];
        var rank = Convert.ToInt32(dRow["rank"]);
        var motto = (string)dRow["motto"];
        var look = (string)dRow["look"];
        var gender = (string)dRow["gender"];
        var lastOnline = Convert.ToInt32(dRow["last_online"]);
        var credits = Convert.ToInt32(dRow["credits"]);
        var diamonds = Convert.ToInt32(dRow["vip_points"]);
        var limitCoins = Convert.ToInt32(dRow["limit_coins"]);
        var activityPoints = Convert.ToInt32(dRow["activity_points"]);
        var homeRoom = Convert.ToInt32(dRow["home_room"]);
        var accountCreated = Convert.ToInt32(dRow["account_created"]);
        var acceptTrading = WibboEnvironment.EnumToBool(dRow["accept_trading"].ToString());
        var ip = dRow["ip_last"].ToString();
        var hideInroom = WibboEnvironment.EnumToBool(dRow["hide_inroom"].ToString());
        var hideOnline = WibboEnvironment.EnumToBool(dRow["hide_online"].ToString());
        var mazoHighScore = Convert.ToInt32(dRow["mazoscore"]);
        var mazo = Convert.ToInt32(dRow["mazo"]);
        var clientVolume = (string)dRow["volume"];
        var nuxEnable = WibboEnvironment.EnumToBool(dRow["nux_enable"].ToString());
        var ignoreRoomInvite = WibboEnvironment.EnumToBool(dRow["ignore_room_invite"].ToString());
        var cameraFollowDisabled = WibboEnvironment.EnumToBool(dRow["camera_follow_disabled"].ToString());
        var machineId = (string)dRow["machine_id"];
        var langue = LanguageManager.ParseLanguage((string)dRow["langue"]);

        var respect = Convert.ToInt32(dRow2["respect"]);
        var dailyRespectPoints = Convert.ToInt32(dRow2["daily_respect_points"]);
        var dailyPetRespectPoints = Convert.ToInt32(dRow2["daily_pet_respect_points"]);
        var hasFriendRequestsDisabled = WibboEnvironment.EnumToBool(dRow["block_newfriends"].ToString());
        var currentQuestID = Convert.ToInt32(dRow2["quest_id"]);
        var achievementPoints = Convert.ToInt32(dRow2["achievement_score"]);
        var favoriteGroup = Convert.ToInt32(dRow2["group_id"]);

        return new User(id, username, rank, motto, look, gender, credits, diamonds, limitCoins, activityPoints, homeRoom, respect, dailyRespectPoints, dailyPetRespectPoints, hasFriendRequestsDisabled, currentQuestID, achievementPoints, lastOnline, favoriteGroup, accountCreated, acceptTrading, ip, hideInroom, hideOnline, mazoHighScore, mazo, clientVolume, nuxEnable, machineId, ChangeName, langue, ignoreAllExpire, ignoreRoomInvite, cameraFollowDisabled);
    }
}
