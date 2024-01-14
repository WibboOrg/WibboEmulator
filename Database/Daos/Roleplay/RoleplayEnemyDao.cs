namespace WibboEmulator.Database.Daos.Roleplay;
using System.Data;
using Dapper;

internal sealed class RoleplayEnemyDao
{
    internal static List<RoleplayEnemyEntity> GetAll(IDbConnection dbClient) => dbClient.Query<RoleplayEnemyEntity>(
        "SELECT `id`, `type`, `health`, `weapon_far_id`, `weapon_cac_id`, `dead_timer`, `loot_item_id`, `money_drop`, `drop_script_id`, `team_id`, `aggro_distance`, `zone_distance`, `reset_position`, `lost_aggro_distance`, `zombie_mode` FROM `roleplay_enemy`"
    ).ToList();

    internal static void Insert(IDbConnection dbClient, int botId, string type) => dbClient.Execute(
        "INSERT INTO roleplay_enemy (id, type) VALUES (@BotId, @Type)",
        new { BotId = botId, Type = type });

    internal static void Delete(IDbConnection dbClient, int id) => dbClient.Execute(
        "DELETE FROM `roleplay_enemy` WHERE id = '" + id + "'");

    internal static void UpdateHealth(IDbConnection dbClient, int rpId, int health) => dbClient.Execute(
        "UPDATE `roleplay_enemy` SET health = '" + health + "' WHERE id = '" + rpId + "'");

    internal static void UpdateWeaponFarId(IDbConnection dbClient, int rpId, int weaponFarId) => dbClient.Execute(
        "UPDATE `roleplay_enemy` SET weapon_far_id = '" + weaponFarId + "' WHERE id = '" + rpId + "'");

    internal static void UpdateWeaponCacId(IDbConnection dbClient, int rpId, int weaponCacId) => dbClient.Execute(
        "UPDATE `roleplay_enemy` SET weapon_cac_id = '" + weaponCacId + "' WHERE id = '" + rpId + "'");

    internal static void UpdateDeadTimer(IDbConnection dbClient, int rpId, int deadTimer) => dbClient.Execute(
        "UPDATE `roleplay_enemy` SET dead_timer = '" + deadTimer + "' WHERE id = '" + rpId + "'");

    internal static void UpdateLootItemId(IDbConnection dbClient, int rpId, int lootItemId) => dbClient.Execute(
        "UPDATE `roleplay_enemy` SET loot_item_id = '" + lootItemId + "' WHERE id = '" + rpId + "'");

    internal static void UpdateMoneyDrop(IDbConnection dbClient, int rpId, int moneyDrop) => dbClient.Execute(
        "UPDATE `roleplay_enemy` SET money_drop = '" + moneyDrop + "' WHERE id = '" + rpId + "'");

    internal static void UpdateTeamId(IDbConnection dbClient, int rpId, int teamId) => dbClient.Execute(
        "UPDATE `roleplay_enemy` SET team_id = '" + teamId + "' WHERE id = '" + rpId + "'");

    internal static void UpdateAggroDistance(IDbConnection dbClient, int rpId, int aggroDistance) => dbClient.Execute(
        "UPDATE `roleplay_enemy` SET aggro_distance = '" + aggroDistance + "' WHERE id = '" + rpId + "'");

    internal static void UpdateZoneDistance(IDbConnection dbClient, int rpId, int zoneDistance) => dbClient.Execute(
        "UPDATE `roleplay_enemy` SET zone_distance = '" + zoneDistance + "' WHERE id = '" + rpId + "'");

    internal static void UpdateResetPosition(IDbConnection dbClient, int rpId, bool resetPosition) => dbClient.Execute(
        "UPDATE `roleplay_enemy` SET reset_position = '" + (resetPosition ? "1" : "0") + "' WHERE id = '" + rpId + "'");

    internal static void UpdateLostAggroDistance(IDbConnection dbClient, int rpId, int lostAggroDistance) => dbClient.Execute(
        "UPDATE `roleplay_enemy` SET lost_aggro_distance = '" + lostAggroDistance + "' WHERE id = '" + rpId + "'");

    internal static void UpdateZombieMode(IDbConnection dbClient, int rpId, bool zombieMode) => dbClient.Execute(
        "UPDATE `roleplay_enemy` SET zombie_mode = '" + (zombieMode ? "1" : "0") + "' WHERE id = '" + rpId + "'");
}

public class RoleplayEnemyEntity
{
    public int Id { get; set; }
    public string Type { get; set; }
    public int Health { get; set; }
    public int WeaponFarId { get; set; }
    public int WeaponCacId { get; set; }
    public int DeadTimer { get; set; }
    public int LootItemId { get; set; }
    public int MoneyDrop { get; set; }
    public int DropScriptId { get; set; }
    public int TeamId { get; set; }
    public int AggroDistance { get; set; }
    public int ZoneDistance { get; set; }
    public bool ResetPosition { get; set; }
    public int LostAggroDistance { get; set; }
    public bool ZombieMode { get; set; }
}