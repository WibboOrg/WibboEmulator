namespace WibboEmulator.Database.Daos.User;

using System.Data;
using Dapper;

internal sealed class UserRoleplayDao
{
    internal static void Delete(IDbConnection dbClient, int userId, int roleplayId) => dbClient.Execute(
        "DELETE FROM `user_roleplay` WHERE user_id = '" + userId + "' AND roleplay_id = '" + roleplayId + "'");

    internal static void Update(IDbConnection dbClient, int userId, int roleplayId, int health, int energy, int money, int munition, int exp, int weaponGunId, int weaponCacId) => dbClient.Execute(
        "UPDATE `user_roleplay` SET `health` = '" + health + "', `energy` = '" + energy + "', `money` = '" + money + "', `munition` = '" + munition + "', `exp` = '" + exp + "', `weapon_far` = '" + weaponGunId + "', `weapon_cac` = '" + weaponCacId + "' WHERE `user_id` = '" + userId + "' AND `roleplay_id` = '" + roleplayId + "' LIMIT 1");

    internal static UserRoleplayEntity GetOne(IDbConnection dbClient, int userId, int roleplayId) => dbClient.QuerySingleOrDefault<UserRoleplayEntity>(
        "SELECT `user_id`, `roleplay_id`, `health`, `energy`, `money`, `munition`, `exp`, `weapon_far`, `weapon_cac` FROM `user_roleplay` WHERE `user_id` = '" + userId + "' AND `roleplay_id` = '" + roleplayId + "'");

    internal static void Insert(IDbConnection dbClient, int userId, int roleplayId) => dbClient.Execute(
        "INSERT INTO `user_roleplay` (`user_id`, `roleplay_id`) VALUES ('" + userId + "', '" + roleplayId + "')");
}

public class UserRoleplayEntity
{
    public int UserId { get; set; }
    public int RoleplayId { get; set; }
    public int Health { get; set; }
    public int Energy { get; set; }
    public int Money { get; set; }
    public int Munition { get; set; }
    public int Exp { get; set; }
    public int WeaponFar { get; set; }
    public int WeaponCac { get; set; }
    public int Hygiene { get; set; }
    public int Money1 { get; set; }
    public int Money2 { get; set; }
    public int Money3 { get; set; }
    public int Money4 { get; set; }
}
