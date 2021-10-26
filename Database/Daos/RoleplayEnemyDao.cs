using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class RoleplayEnemyDao
    {
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT * FROM roleplay_enemy");
            DataTable table1 = dbClient.GetTable();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("INSERT INTO roleplay_enemy (id, type) VALUES ('" + BotId + "', 'bot')");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("INSERT INTO roleplay_enemy (id, type, weapon_far_id) VALUES ('" + PetId + "', 'pet', '0');");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE FROM roleplay_enemy WHERE id = '" + BotId + "'");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE FROM roleplay_enemy WHERE id = '" + PetId + "'");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE roleplay_enemy SET health = '" + ParamInt + "' WHERE id = '" + RPEnemyConfig.Id + "'");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE roleplay_enemy SET weapon_far_id = '" + ParamInt + "' WHERE id = '" + RPEnemyConfig.Id + "'");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE roleplay_enemy SET weapon_cac_id = '" + ParamInt + "' WHERE id = '" + RPEnemyConfig.Id + "'");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE roleplay_enemy SET dead_timer = '" + ParamInt + "' WHERE id = '" + RPEnemyConfig.Id + "'");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE roleplay_enemy SET loot_item_id = '" + ParamInt + "' WHERE id = '" + RPEnemyConfig.Id + "'");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE roleplay_enemy SET money_drop = '" + ParamInt + "' WHERE id = '" + RPEnemyConfig.Id + "'");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE roleplay_enemy SET team_id = '" + ParamInt + "' WHERE id = '" + RPEnemyConfig.Id + "'");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE roleplay_enemy SET aggro_distance = '" + ParamInt + "' WHERE id = '" + RPEnemyConfig.Id + "'");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE roleplay_enemy SET zone_distance = '" + ParamInt + "' WHERE id = '" + RPEnemyConfig.Id + "'");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE roleplay_enemy SET reset_position = '" + ButterflyEnvironment.BoolToEnum(RPEnemyConfig.ResetPosition) + "' WHERE id = '" + RPEnemyConfig.Id + "'");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE roleplay_enemy SET lost_aggro_distance = '" + ParamInt + "' WHERE id = '" + RPEnemyConfig.Id + "'");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE roleplay_enemy SET zombie_mode = '" + ButterflyEnvironment.BoolToEnum(RPEnemyConfig.ZombieMode) + "' WHERE id = '" + RPEnemyConfig.Id + "'");
        }
    }
}