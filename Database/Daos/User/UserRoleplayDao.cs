namespace WibboEmulator.Database.Daos;
using System.Data;
using WibboEmulator.Database.Interfaces;

internal class UserRoleplayDao
{
    internal static void Delete(IQueryAdapter dbClient, int userId, int roleplayId) => dbClient.RunQuery("DELETE FROM `user_roleplay` WHERE user_id = '" + userId + "' AND roleplay_id = '" + roleplayId + "'");

    internal static void Update(IQueryAdapter dbClient, int userId, int roleplayId, int health, int energy, int money, int munition, int exp, int weaponGunId, int weaponCacId) => dbClient.RunQuery("UPDATE `user_roleplay` SET `health` = '" + health + "', `energy` = '" + energy + "', `money` = '" + money + "', `munition` = '" + munition + "', `exp` = '" + exp + "', `weapon_far` = '" + weaponGunId + "', `weapon_cac` = '" + weaponCacId + "' WHERE `user_id` = '" + userId + "' AND `roleplay_id` = '" + roleplayId + "' LIMIT 1");

    internal static DataRow GetOne(IQueryAdapter dbClient, int userId, int roleplayId)
    {
        dbClient.SetQuery("SELECT `user_id`, `roleplay_id`, `health`, `energy`, `money`, `munition`, `exp`, `weapon_far`, `weapon_cac` FROM `user_roleplay` WHERE `user_id` = '" + userId + "' AND `roleplay_id` = '" + roleplayId + "'");
        return dbClient.GetRow();
    }

    internal static void Insert(IQueryAdapter dbClient, int userId, int roleplayId) => dbClient.RunQuery("INSERT INTO `user_roleplay` (`user_id`, `roleplay_id`) VALUES ('" + userId + "', '" + roleplayId + "')");
}