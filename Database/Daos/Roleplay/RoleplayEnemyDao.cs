namespace WibboEmulator.Database.Daos.Roleplay;
using System.Data;
using WibboEmulator.Database.Interfaces;

internal sealed class RoleplayEnemyDao
{
    internal static DataTable GetAll(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT `id`, `type`, `health`, `weapon_far_id`, `weapon_cac_id`, `dead_timer`, `loot_item_id`, `money_drop`, `drop_script_id`, `team_id`, `aggro_distance`, `zone_distance`, `reset_position`, `lost_aggro_distance`, `zombie_mode` FROM `roleplay_enemy`");
        return dbClient.GetTable();
    }

    internal static void Insert(IQueryAdapter dbClient, int botId, string type)
    {
        dbClient.SetQuery("INSERT INTO `roleplay_enemy` (id, type) VALUES ('" + botId + "', @type)");
        dbClient.AddParameter("type", type);
        dbClient.RunQuery();
    }

    internal static void Delete(IQueryAdapter dbClient, int id) => dbClient.RunQuery("DELETE FROM `roleplay_enemy` WHERE id = '" + id + "'");

    internal static void UpdateHealth(IQueryAdapter dbClient, int rpId, int health) => dbClient.RunQuery("UPDATE `roleplay_enemy` SET health = '" + health + "' WHERE id = '" + rpId + "'");

    internal static void UpdateWeaponFarId(IQueryAdapter dbClient, int rpId, int weaponFarId) => dbClient.RunQuery("UPDATE `roleplay_enemy` SET weapon_far_id = '" + weaponFarId + "' WHERE id = '" + rpId + "'");

    internal static void UpdateWeaponCacId(IQueryAdapter dbClient, int rpId, int weaponCacId) => dbClient.RunQuery("UPDATE `roleplay_enemy` SET weapon_cac_id = '" + weaponCacId + "' WHERE id = '" + rpId + "'");

    internal static void UpdateDeadTimer(IQueryAdapter dbClient, int rpId, int deadTimer) => dbClient.RunQuery("UPDATE `roleplay_enemy` SET dead_timer = '" + deadTimer + "' WHERE id = '" + rpId + "'");

    internal static void UpdateLootItemId(IQueryAdapter dbClient, int rpId, int lootItemId) => dbClient.RunQuery("UPDATE `roleplay_enemy` SET loot_item_id = '" + lootItemId + "' WHERE id = '" + rpId + "'");

    internal static void UpdateMoneyDrop(IQueryAdapter dbClient, int rpId, int moneyDrop) => dbClient.RunQuery("UPDATE `roleplay_enemy` SET money_drop = '" + moneyDrop + "' WHERE id = '" + rpId + "'");

    internal static void UpdateTeamId(IQueryAdapter dbClient, int rpId, int teamId) => dbClient.RunQuery("UPDATE `roleplay_enemy` SET team_id = '" + teamId + "' WHERE id = '" + rpId + "'");

    internal static void UpdateAggroDistance(IQueryAdapter dbClient, int rpId, int aggroDistance) => dbClient.RunQuery("UPDATE `roleplay_enemy` SET aggro_distance = '" + aggroDistance + "' WHERE id = '" + rpId + "'");

    internal static void UpdateZoneDistance(IQueryAdapter dbClient, int rpId, int zoneDistance) => dbClient.RunQuery("UPDATE `roleplay_enemy` SET zone_distance = '" + zoneDistance + "' WHERE id = '" + rpId + "'");

    internal static void UpdateResetPosition(IQueryAdapter dbClient, int rpId, bool resetPosition) => dbClient.RunQuery("UPDATE `roleplay_enemy` SET reset_position = '" + WibboEnvironment.BoolToEnum(resetPosition) + "' WHERE id = '" + rpId + "'");

    internal static void UpdateLostAggroDistance(IQueryAdapter dbClient, int rpId, int lostAggroDistance) => dbClient.RunQuery("UPDATE `roleplay_enemy` SET lost_aggro_distance = '" + lostAggroDistance + "' WHERE id = '" + rpId + "'");

    internal static void UpdateZombieMode(IQueryAdapter dbClient, int rpId, bool zombieMode) => dbClient.RunQuery("UPDATE `roleplay_enemy` SET zombie_mode = '" + WibboEnvironment.BoolToEnum(zombieMode) + "' WHERE id = '" + rpId + "'");
}