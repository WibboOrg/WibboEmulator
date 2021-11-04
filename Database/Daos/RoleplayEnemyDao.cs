using System.Data;
using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class RoleplayEnemyDao
    {
        internal static DataTable GetAll(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT * FROM roleplay_enemy");
            return dbClient.GetTable();
        }

        internal static void InsertBot(IQueryAdapter dbClient, int botId)
        {
            dbClient.RunQuery("INSERT INTO roleplay_enemy (id, type) VALUES ('" + botId + "', 'bot')");
        }

        internal static void InsertPet(IQueryAdapter dbClient, int petId)
        {
            dbClient.RunQuery("INSERT INTO roleplay_enemy (id, type, weapon_far_id) VALUES ('" + petId + "', 'pet', '0');");
        }

        internal static void DeleteBot(IQueryAdapter dbClient, int botId)
        {
            dbClient.RunQuery("DELETE FROM roleplay_enemy WHERE id = '" + botId + "'");
        }

        internal static void DeletePet(IQueryAdapter dbClient, int petId)
        {
            dbClient.RunQuery("DELETE FROM roleplay_enemy WHERE id = '" + petId + "'");
        }

        internal static void UpdateHealth(IQueryAdapter dbClient, int rpId, int health)
        {
            dbClient.RunQuery("UPDATE roleplay_enemy SET health = '" + health + "' WHERE id = '" + rpId + "'");
        }

        internal static void UpdateWeaponFarId(IQueryAdapter dbClient, int rpId, int weaponFarId)
        {
            dbClient.RunQuery("UPDATE roleplay_enemy SET weapon_far_id = '" + weaponFarId + "' WHERE id = '" + rpId + "'");
        }

        internal static void UpdateWeaponCacId(IQueryAdapter dbClient, int rpId, int weaponCacId)
        {
            dbClient.RunQuery("UPDATE roleplay_enemy SET weapon_cac_id = '" + weaponCacId + "' WHERE id = '" + rpId + "'");
        }

        internal static void UpdateDeadTimer(IQueryAdapter dbClient, int rpId, int deadTimer)
        {
            dbClient.RunQuery("UPDATE roleplay_enemy SET dead_timer = '" + deadTimer + "' WHERE id = '" + rpId + "'");
        }

        internal static void UpdateLootItemId(IQueryAdapter dbClient, int rpId, int lootItemId)
        {
            dbClient.RunQuery("UPDATE roleplay_enemy SET loot_item_id = '" + lootItemId + "' WHERE id = '" + rpId + "'");
        }

        internal static void UpdateMoneyDrop(IQueryAdapter dbClient, int rpId, int moneyDrop)
        {
            dbClient.RunQuery("UPDATE roleplay_enemy SET money_drop = '" + moneyDrop + "' WHERE id = '" + rpId + "'");
        }

        internal static void UpdateTeamId(IQueryAdapter dbClient, int rpId, int teamId)
        {
            dbClient.RunQuery("UPDATE roleplay_enemy SET team_id = '" + teamId + "' WHERE id = '" + rpId + "'");
        }

        internal static void UpdateAggroDistance(IQueryAdapter dbClient, int rpId, int aggroDistance)
        {
            dbClient.RunQuery("UPDATE roleplay_enemy SET aggro_distance = '" + aggroDistance + "' WHERE id = '" + rpId + "'");
        }

        internal static void UpdateZoneDistance(IQueryAdapter dbClient, int rpId, int zoneDistance)
        {
            dbClient.RunQuery("UPDATE roleplay_enemy SET zone_distance = '" + zoneDistance + "' WHERE id = '" + rpId + "'");
        }

        internal static void UpdateResetPosition(IQueryAdapter dbClient, int rpId, bool resetPosition)
        {
            dbClient.RunQuery("UPDATE roleplay_enemy SET reset_position = '" + ButterflyEnvironment.BoolToEnum(resetPosition) + "' WHERE id = '" + rpId + "'");
        }

        internal static void UpdateLostAggroDistance(IQueryAdapter dbClient, int rpId, int lostAggroDistance)
        {
            dbClient.RunQuery("UPDATE roleplay_enemy SET lost_aggro_distance = '" + lostAggroDistance + "' WHERE id = '" + rpId + "'");
        }

        internal static void UpdateZombieMode(IQueryAdapter dbClient, int rpId, bool zombieMode)
        {
            dbClient.RunQuery("UPDATE roleplay_enemy SET zombie_mode = '" + ButterflyEnvironment.BoolToEnum(zombieMode) + "' WHERE id = '" + rpId + "'");
        }
    }
}