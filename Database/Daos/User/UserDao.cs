namespace WibboEmulator.Database.Daos.User;
using System.Data;
using Dapper;

internal sealed class UserDao
{
    internal static int GetIdByName(IDbConnection dbClient, string name) => dbClient.ExecuteScalar<int>(
        "SELECT id FROM `user` WHERE username = @Username LIMIT 1",
        new { Username = name });

    internal static string GetNameById(IDbConnection dbClient, int userId) => dbClient.ExecuteScalar<string>(
        "SELECT username FROM `user` WHERE id = @userId LIMIT 1",
        new { userId });

    internal static int GetCredits(IDbConnection dbClient, int userId) => dbClient.ExecuteScalar<int>(
        "SELECT credits FROM `user` WHERE id = @UserId",
        new { UserId = userId });

    internal static UserEntity GetOneIdAndBlockNewFriend(IDbConnection dbClient, string username) => dbClient.QuerySingleOrDefault<UserEntity>(
        "SELECT id, block_newfriends FROM `user` WHERE username = @UserName",
        new { UserName = username });

    internal static List<UserEntity> GetAllSearchUsers(IDbConnection dbClient, string search) => dbClient.Query<UserEntity>(
        "SELECT id, username, look, motto, last_online FROM `user` WHERE username LIKE @Search AND is_banned = '0' LIMIT 50",
        new { Search = search.Replace("%", "\\%").Replace("_", "\\_") + "%" }
    ).ToList();

    internal static List<int> GetTop10ByGamePointMonth(IDbConnection dbClient) => dbClient.Query<int>(
        "SELECT id FROM `user` WHERE game_points_month > '0' AND is_banned = '0' AND rank < '6' ORDER BY game_points_month DESC LIMIT 10"
    ).ToList();

    internal static UserEntity GetOneByTicket(IDbConnection dbClient, string ticket) => dbClient.QuerySingleOrDefault<UserEntity>(
        "SELECT `id`, `username`, `auth_ticket`, `rank`, `credits`, `activity_points`, `look`, `gender`, `motto`, `account_created`, `last_online`, `online`, `ip_last`, `home_room`, `block_newfriends`, `hide_online`, `hide_inroom`, `camera_follow_disabled`, `ignore_room_invite`, `last_offline`, `mois_vip`, `volume`, `vip_points`, `limit_coins`, `accept_trading`, `lastdailycredits`, `hide_gamealert`, `ipcountry`, `game_points`, `game_points_month`, `mazoscore`, `mazo`, `nux_enable`, `langue`, `run_points`, `run_points_month`, `is_banned`, `banner_id` FROM `user` WHERE auth_ticket = @SsoTicket LIMIT 1",
        new { SsoTicket = ticket });

    internal static UserEntity GetOne(IDbConnection dbClient, int userId) => dbClient.QuerySingleOrDefault<UserEntity>(
        "SELECT `id`, `username`, `auth_ticket`, `rank`, `credits`, `activity_points`, `look`, `gender`, `motto`, `account_created`, `last_online`, `online`, `ip_last`, `home_room`, `block_newfriends`, `hide_online`, `hide_inroom`, `camera_follow_disabled`, `ignore_room_invite`, `last_offline`, `mois_vip`, `volume`, `vip_points`, `limit_coins`, `accept_trading`, `lastdailycredits`, `hide_gamealert`, `ipcountry`, `game_points`, `game_points_month`, `mazoscore`, `mazo`, `nux_enable`, `langue`, `run_points`, `run_points_month`, `is_banned`, `banner_id` FROM `user` WHERE id = @Id LIMIT 1",
        new { Id = userId });

    internal static void UpdateRemoveLimitCoins(IDbConnection dbClient, int userId, int points) => dbClient.Execute(
        "UPDATE `user` SET `limit_coins` = `limit_coins` - '" + points + "' WHERE `id` = '" + userId + "'");

    internal static void UpdateRemovePoints(IDbConnection dbClient, int userId, int points) => dbClient.Execute(
        "UPDATE `user` SET `vip_points` = `vip_points` - '" + points + "' WHERE `id` = '" + userId + "'");

    internal static void UpdateAddPoints(IDbConnection dbClient, int userId, int points) => dbClient.Execute(
        "UPDATE `user` SET `vip_points` = `vip_points` + '" + points + "' WHERE `id` = '" + userId + "'");

    internal static void UpdateHomeRoom(IDbConnection dbClient, int userId, int roomId) => dbClient.Execute(
        "UPDATE `user` SET `home_room` = '" + roomId + "' WHERE `id` = '" + userId + "'");

    internal static void UpdateNuxEnable(IDbConnection dbClient, int userId, int roomId) => dbClient.Execute(
        "UPDATE `user` SET `nux_enable` = '0', `home_room` = '" + roomId + "' WHERE `id` = '" + userId + "'");

    internal static void UpdateMotto(IDbConnection dbClient, int userId, string motto) => dbClient.Execute(
        "UPDATE user SET motto = @Motto WHERE id = @UserId",
        new { Motto = motto, UserId = userId });

    internal static void UpdateAddMonthPremium(IDbConnection dbClient, int userId) => dbClient.Execute(
        "UPDATE `user` SET `mois_vip` = mois_vip + '1' WHERE `id` = '" + userId + "' LIMIT 1");

    internal static void UpdateRank(IDbConnection dbClient, int userId, int rank) => dbClient.Execute(
        "UPDATE `user` SET `rank` = '" + rank + "' WHERE `id` = '" + userId + "' LIMIT 1");

    internal static void UpdateIgnoreRoomInvites(IDbConnection dbClient, int userId, bool flag) => dbClient.Execute(
        "UPDATE `user` SET `ignore_room_invite` = '" + (flag ? "1" : "0") + "' WHERE `id` = '" + userId + "' LIMIT 1");

    internal static void UpdateCameraFollowDisabled(IDbConnection dbClient, int userId, bool flag) => dbClient.Execute(
        "UPDATE `user` SET `camera_follow_disabled` = '" + (flag ? "1" : "0") + "' WHERE `id` = '" + userId + "' LIMIT 1");

    internal static void UpdateVolume(IDbConnection dbClient, int userId, int volume1, int volume2, int volume3) => dbClient.Execute(
        "UPDATE `user` SET `volume` = '" + volume1 + "," + volume2 + "," + volume3 + "' WHERE `id` = '" + userId + "' LIMIT 1");

    internal static void UpdateName(IDbConnection dbClient, int userId, string username) => dbClient.Execute(
        "UPDATE user SET username = @NewName WHERE id = @UserId",
        new { NewName = username, UserId = userId });

    internal static void UpdateLookAndGender(IDbConnection dbClient, int userId, string look, string gender) => dbClient.Execute(
        "UPDATE user SET look = @Look, gender = @Gender WHERE id = @UserId",
        new { Look = look, Gender = gender, UserId = userId });

    internal static void UpdateAllOnline(IDbConnection dbClient) => dbClient.Execute(
        "UPDATE `user` SET `online` = '0' WHERE `online` = '1'");

    internal static void UpdateAllTicket(IDbConnection dbClient) => dbClient.Execute(
        "UPDATE `user` SET `auth_ticket` = '' WHERE `auth_ticket` != ''");

    internal static void UpdateAddGamePoints(IDbConnection dbClient, int userId) => dbClient.Execute(
        "UPDATE `user` SET `game_points` = `game_points` + 1, `game_points_month` = `game_points_month` + 1 WHERE `id` = '" + userId + "'");

    internal static void UpdateMazoScore(IDbConnection dbClient, int userId, int score) => dbClient.Execute(
        "UPDATE `user` SET `mazoscore` = '" + score + "' WHERE `id` = '" + userId + "'");

    internal static void UpdateMazo(IDbConnection dbClient, int userId, int score) => dbClient.Execute(
        "UPDATE `user` SET `mazo` = '" + score + "' WHERE `id` = '" + userId + "'");

    internal static void UpdateAddRunPoints(IDbConnection dbClient, int userId) => dbClient.Execute(
        "UPDATE `user` SET `run_points` = `run_points` + 1, `run_points_month` = `run_points_month` + 1 WHERE `id` = '" + userId + "'");

    internal static void UpdateOffline(IDbConnection dbClient, int userId, int duckets, int credits, string look, int bannerId) => dbClient.Execute(
        "UPDATE `user` SET `online` = '0', `last_online` = '" + WibboEnvironment.GetUnixTimestamp() + "', `activity_points` = '" + duckets + "', `credits` = '" + credits + "', `look` = @Look, `banner_id` = '" + bannerId + "' WHERE `id` = '" + userId + "'",
        new { Look = look });

    internal static void UpdateLastDailyCredits(IDbConnection dbClient, int userId, string lastDailyCredits) => dbClient.Execute(
        "UPDATE `user` SET `lastdailycredits` = '" + lastDailyCredits + "' WHERE `id` = '" + userId + "'");

    internal static void UpdateOnline(IDbConnection dbClient, int userId) => dbClient.Execute(
        "UPDATE `user` SET `online` = '1', `auth_ticket` = ''  WHERE `id` = '" + userId + "'");

    internal static void UpdateIsBanned(IDbConnection dbClient, int userId) => dbClient.Execute(
        "UPDATE `user` SET `is_banned` = '1' WHERE `id` = '" + userId + "'");
}

public class UserEntity
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Mail { get; set; }
    public string AuthTicket { get; set; }
    public int Rank { get; set; }
    public int Credits { get; set; }
    public int ActivityPoints { get; set; }
    public string Look { get; set; }
    public string Gender { get; set; }
    public string Motto { get; set; }
    public int AccountCreated { get; set; }
    public int LastOnline { get; set; }
    public bool Online { get; set; }
    public string IpLast { get; set; }
    public int HomeRoom { get; set; }
    public bool BlockNewFriends { get; set; }
    public bool HideOnline { get; set; }
    public bool HideInRoom { get; set; }
    public int LastOffline { get; set; }
    public int MoisVip { get; set; }
    public string Volume { get; set; }
    public int VipPoints { get; set; }
    public int LimitCoins { get; set; }
    public bool AcceptTrading { get; set; }
    public bool CameraFollowDisabled { get; set; }
    public bool IgnoreRoomInvite { get; set; }
    public string LastDailyCredits { get; set; }
    public bool HideGameAlert { get; set; }
    public string IpCountry { get; set; }
    public int GamePoints { get; set; }
    public int GamePointsMonth { get; set; }
    public int MazoScore { get; set; }
    public int Mazo { get; set; }
    public bool NuxEnable { get; set; }
    public string Langue { get; set; }
    public int RunPoints { get; set; }
    public int RunPointsMonth { get; set; }
    public bool IsBanned { get; set; }
    public int BannerId { get; set; }
}
