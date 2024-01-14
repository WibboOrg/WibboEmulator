namespace WibboEmulator.Database.Daos;

using System.Data;
using Dapper;

internal sealed class BanDao
{
    internal static bool IsBanned(IDbConnection dbClient, string username, string ipOne, string ipTwo) => dbClient.QueryFirstOrDefault<int>(
        "SELECT `id` FROM `ban` WHERE `expire` > UNIX_TIMESTAMP() AND ((`bantype` = 'user' AND `value` = @username) OR (`bantype` = 'ip' AND `value` = @ipOne) OR (`bantype` = 'ip' AND `value` = @ipTwo)) LIMIT 1",
        new { username, ipOne, ipTwo }) > 0;

    internal static int GetOneIgnoreAll(IDbConnection dbClient, int userId) => dbClient.QueryFirstOrDefault<int>(
        "SELECT `expire` FROM `ban` WHERE `bantype` = 'ignoreall' AND `value` = @userId LIMIT 1",
        new { userId });

    internal static void InsertBan(IDbConnection dbClient, int expireTime, string banType, string userIdOrUsername, string reason, string modName) => dbClient.Execute(
        "INSERT INTO `ban` (`bantype`,`value`,`reason`,`expire`,`added_by`,`added_date`) VALUES (@banType, @userIdOrUsername, @reason, @expireTime, @modName, UNIX_TIMESTAMP())",
        new { expireTime, banType, userIdOrUsername, reason, modName });
}