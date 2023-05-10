namespace WibboEmulator.Database.Daos.User;
using System.Data;
using WibboEmulator.Database.Interfaces;

internal sealed class UserDao
{
    internal static string BuildUpdateQuery(int userId, int duckets, int credits) => "UPDATE `user` SET `user`.online = '0', `user`.last_online = '" + WibboEnvironment.GetUnixTimestamp() + "', activity_points = '" + duckets + "', credits = '" + credits + "' WHERE id = '" + userId + "';";

    internal static int GetIdByName(IQueryAdapter dbClient, string name)
    {
        dbClient.SetQuery("SELECT id FROM `user` WHERE username = @username LIMIT 1");
        dbClient.AddParameter("username", name);

        return dbClient.GetInteger();
    }

    internal static string GetNameById(IQueryAdapter dbClient, int userId)
    {
        dbClient.SetQuery("SELECT username FROM `user` WHERE id = @id LIMIT 1");
        dbClient.AddParameter("id", userId);

        return dbClient.GetString();
    }

    internal static int GetCredits(IQueryAdapter dbClient, int userId)
    {
        dbClient.SetQuery("SELECT credits FROM `user` WHERE id = @userid");
        dbClient.AddParameter("userid", userId);
        return dbClient.GetInteger();
    }

    internal static DataRow GetIdAndLangue(IQueryAdapter dbClient, string owner)
    {
        dbClient.SetQuery("SELECT id, langue FROM `user` WHERE username = @owner");
        dbClient.AddParameter("owner", owner);
        return dbClient.GetRow();
    }

    internal static DataRow GetOneInfo(IQueryAdapter dbClient, int userId)
    {
        dbClient.SetQuery("SELECT id, username, look, account_created, last_online, mail FROM `user` WHERE id = '" + userId + "'");
        return dbClient.GetRow();
    }

    internal static DataRow GetOneIdAndBlockNewFriend(IQueryAdapter dbClient, string username)
    {
        dbClient.SetQuery("SELECT id, block_newfriends FROM `user` WHERE username = @username");
        dbClient.AddParameter("username", username);
        return dbClient.GetRow();
    }

    internal static DataTable GetAllSearchUsers(IQueryAdapter dbClient, string search)
    {
        dbClient.SetQuery("SELECT id, username, look, motto, last_online FROM `user` WHERE username LIKE @search AND is_banned = '0' LIMIT 50");
        dbClient.AddParameter("search", search.Replace("%", "\\%").Replace("_", "\\_") + "%");
        return dbClient.GetTable();
    }

    internal static DataTable GetTop10ByGamePointMonth(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT id FROM `user` WHERE game_points_month > '0' ORDER BY game_points_month DESC LIMIT 10");
        return dbClient.GetTable();
    }

    internal static DataRow GetOneByTicket(IQueryAdapter dbClient, string sessionTicket)
    {
        dbClient.SetQuery("SELECT `id`, `username`, `auth_ticket`, `rank`, `credits`, `activity_points`, `look`, `gender`, `motto`, `account_created`, `last_online`, `online`, `ip_last`, `machine_id`, `home_room`, `block_newfriends`, `hide_online`, `hide_inroom`, `camera_follow_disabled`, `ignore_room_invite`, `last_offline`, `mois_vip`, `volume`, `vip_points`, `limit_coins`, `accept_trading`, `lastdailycredits`, `hide_gamealert`, `ipcountry`, `game_points`, `game_points_month`, `mazoscore`, `mazo`, `nux_enable`, `langue`, `run_points`, `run_points_month`, `is_banned` FROM `user` WHERE auth_ticket = @sso LIMIT 1");
        dbClient.AddParameter("sso", sessionTicket);
        return dbClient.GetRow();
    }

    internal static DataTable GetAllFriendShips(IQueryAdapter dbClient, int userId)
    {
        dbClient.SetQuery("SELECT `user`.id,`user`.username,`messenger_friendship`.relation FROM `user` JOIN `messenger_friendship` ON `user`.id = `messenger_friendship`.user_two_id WHERE `messenger_friendship`.user_one_id = '" + userId + "'");
        return dbClient.GetTable();
    }

    internal static DataTable GetAllFriendRequests(IQueryAdapter dbClient, int userId)
    {
        dbClient.SetQuery("SELECT `messenger_request`.from_id,`messenger_request`.to_id,`user`.username FROM `user` JOIN `messenger_request` ON `user`.id = `messenger_request`.from_id WHERE `messenger_request`.to_id = '" + userId + "'");
        return dbClient.GetTable();
    }

    internal static DataRow GetOne(IQueryAdapter dbClient, int userId)
    {
        dbClient.SetQuery("SELECT `id`, `username`, `auth_ticket`, `rank`, `credits`, `activity_points`, `look`, `gender`, `motto`, `account_created`, `last_online`, `online`, `ip_last`, `machine_id`, `home_room`, `block_newfriends`, `hide_online`, `hide_inroom`, `camera_follow_disabled`, `ignore_room_invite`, `last_offline`, `mois_vip`, `volume`, `vip_points`, `limit_coins`, `accept_trading`, `lastdailycredits`, `hide_gamealert`, `ipcountry`, `game_points`, `game_points_month`, `mazoscore`, `mazo`, `nux_enable`, `langue`, `run_points`, `run_points_month`, `is_banned` FROM `user` WHERE id = @id LIMIT 1");
        dbClient.AddParameter("id", userId);
        return dbClient.GetRow();
    }

    internal static DataTable GetAllFriendRelation(IQueryAdapter dbClient, int userId)
    {
        dbClient.SetQuery("SELECT `user`.`id`, `messenger_friendship`.`relation` FROM `user` JOIN `messenger_friendship` ON `user`.`id` = `messenger_friendship`.user_two_id WHERE `messenger_friendship`.user_one_id = '" + userId + "' AND `messenger_friendship`.relation != '0'");
        return dbClient.GetTable();
    }

    internal static DataRow GetOneVolume(IQueryAdapter dbClient, int userId)
    {
        dbClient.SetQuery("SELECT `volume` FROM `user` WHERE `id` = '" + userId + "'");
        return dbClient.GetRow();
    }

    internal static void UpdateRemoveLimitCoins(IQueryAdapter dbClient, int userId, int points) => dbClient.RunQuery("UPDATE `user` SET `limit_coins` = `limit_coins` - '" + points + "' WHERE `id` = '" + userId + "'");

    internal static void UpdateRemovePoints(IQueryAdapter dbClient, int userId, int points) => dbClient.RunQuery("UPDATE `user` SET `vip_points` = `vip_points` - '" + points + "' WHERE `id` = '" + userId + "'");

    internal static void UpdateAddPoints(IQueryAdapter dbClient, int userId, int points) => dbClient.RunQuery("UPDATE `user` SET `vip_points` = `vip_points` + '" + points + "' WHERE `id` = '" + userId + "'");

    internal static void UpdateHomeRoom(IQueryAdapter dbClient, int userId, int roomId) => dbClient.RunQuery("UPDATE `user` SET `home_room` = '" + roomId + "' WHERE `id` = '" + userId + "'");

    internal static void UpdateNuxEnable(IQueryAdapter dbClient, int userId, int roomId) => dbClient.RunQuery("UPDATE `user` SET `nux_enable` = '0', `home_room` = '" + roomId + "' WHERE `id` = '" + userId + "'");

    internal static void UpdateMotto(IQueryAdapter dbClient, int userId, string motto)
    {
        dbClient.SetQuery("UPDATE `user` SET `motto` = @motto WHERE `id` = '" + userId + "'");
        dbClient.AddParameter("motto", motto);
        dbClient.RunQuery();
    }

    internal static void UpdateAddMonthPremium(IQueryAdapter dbClient, int userId) => dbClient.RunQuery("UPDATE `user` SET `mois_vip` = mois_vip + '1' WHERE `id` = '" + userId + "' LIMIT 1");

    internal static void UpdateRank(IQueryAdapter dbClient, int userId, int rank) => dbClient.RunQuery("UPDATE `user` SET `rank` = '" + rank + "' WHERE `id` = '" + userId + "' LIMIT 1");

    internal static void UpdateIgnoreRoomInvites(IQueryAdapter dbClient, int userId, bool flag) => dbClient.RunQuery("UPDATE `user` SET `ignore_room_invite` = '" + WibboEnvironment.BoolToEnum(flag) + "' WHERE `id` = '" + userId + "' LIMIT 1");

    internal static void UpdateCameraFollowDisabled(IQueryAdapter dbClient, int userId, bool flag) => dbClient.RunQuery("UPDATE `user` SET `camera_follow_disabled` = '" + WibboEnvironment.BoolToEnum(flag) + "' WHERE `id` = '" + userId + "' LIMIT 1");

    internal static void UpdateVolume(IQueryAdapter dbClient, int userId, int volume1, int volume2, int volume3) => dbClient.RunQuery("UPDATE `user` SET `volume` = '" + volume1 + "," + volume2 + "," + volume3 + "' WHERE `id` = '" + userId + "' LIMIT 1");

    internal static void UpdateName(IQueryAdapter dbClient, int userId, string username)
    {
        dbClient.SetQuery("UPDATE `user` SET `username` = @newname WHERE `id` = @userid");
        dbClient.AddParameter("newname", username);
        dbClient.AddParameter("userid", userId);
        dbClient.RunQuery();
    }

    internal static void UpdateLookAndGender(IQueryAdapter dbClient, int userId, string look, string gender)
    {
        dbClient.SetQuery("UPDATE `user` SET `look` = @look, `gender` = @gender WHERE `id` = '" + userId + "'");
        dbClient.AddParameter("look", look);
        dbClient.AddParameter("gender", gender);
        dbClient.RunQuery();
    }

    internal static void UpdateLook(IQueryAdapter dbClient, int userId, string look)
    {
        dbClient.SetQuery("UPDATE `user` SET `look` = @look WHERE `id` = '" + userId + "'");
        dbClient.AddParameter("look", look);
        dbClient.RunQuery();
    }

    internal static void UpdateAllOnline(IQueryAdapter dbClient) => dbClient.RunQuery("UPDATE `user` SET `online` = '0' WHERE `online` = '1'");

    internal static void UpdateAllTicket(IQueryAdapter dbClient) => dbClient.RunQuery("UPDATE `user` SET `auth_ticket` = '' WHERE `auth_ticket` != ''");

    internal static void UpdateMachineId(IQueryAdapter dbClient, int userId, string machineId)
    {
        dbClient.SetQuery("UPDATE `user` SET `machine_id` = @machineid WHERE `id` = '" + userId + "'");
        dbClient.AddParameter("machineid", machineId);
        dbClient.RunQuery();
    }

    internal static void UpdateAddGamePoints(IQueryAdapter dbClient, int userId) => dbClient.RunQuery("UPDATE `user` SET `game_points` = `game_points` + 1, `game_points_month` = `game_points_month` + 1 WHERE `id` = '" + userId + "'");

    internal static void UpdateMazoScore(IQueryAdapter dbClient, int userId, int score) => dbClient.RunQuery("UPDATE `user` SET `mazoscore` = '" + score + "' WHERE `id` = '" + userId + "'");

    internal static void UpdateMazo(IQueryAdapter dbClient, int userId, int score) => dbClient.RunQuery("UPDATE `user` SET `mazo` = '" + score + "' WHERE `id` = '" + userId + "'");

    internal static void UpdateAddRunPoints(IQueryAdapter dbClient, int userId) => dbClient.RunQuery("UPDATE `user` SET `run_points` = `run_points` + 1, `run_points_month` = `run_points_month` + 1 WHERE `id` = '" + userId + "'");

    internal static void UpdateOffline(IQueryAdapter dbClient, int userId, int duckets, int credits) => dbClient.RunQuery("UPDATE `user` SET `online` = '0', `last_online` = '" + WibboEnvironment.GetUnixTimestamp() + "', `activity_points` = '" + duckets + "', `credits` = '" + credits + "' WHERE `id` = '" + userId + "'");

    internal static void UpdateLastDailyCredits(IQueryAdapter dbClient, int userId, string lastDailyCredits) => dbClient.RunQuery("UPDATE `user` SET `lastdailycredits` = '" + lastDailyCredits + "' WHERE `id` = '" + userId + "'");

    internal static void UpdateOnline(IQueryAdapter dbClient, int userId) => dbClient.RunQuery("UPDATE `user` SET `online` = '1', `auth_ticket` = ''  WHERE `id` = '" + userId + "'");

    internal static void UpdateIsBanned(IQueryAdapter dbClient, int userId) => dbClient.RunQuery("UPDATE `user` SET `is_banned` = '1' WHERE `id` = '" + userId + "'");
}
