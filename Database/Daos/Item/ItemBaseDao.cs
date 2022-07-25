
using WibboEmulator.Database.Interfaces;
using System.Data;

namespace WibboEmulator.Database.Daos
{
    class ItemBaseDao
    {
        internal static DataTable GetAll(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT id, sprite_id, item_name, type, width, length, stack_height, can_stack, is_walkable, can_sit, allow_recycle, allow_trade, allow_gift, allow_inventory_stack, interaction_type, interaction_modes_count, vending_ids, height_adjustable, effect_id, is_rare, rarity_level, item_stat.amount FROM `item_base` LEFT JOIN item_stat ON item_base.id = item_stat.base_id");
            return dbClient.GetTable();
        }
    }
}