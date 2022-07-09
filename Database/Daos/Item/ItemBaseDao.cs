
using Wibbo.Database.Interfaces;
using System.Data;

namespace Wibbo.Database.Daos
{
    class ItemBaseDao
    {
        internal static DataTable GetAll(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT id, sprite_id, item_name, type, width, length, stack_height, can_stack, is_walkable, can_sit, allow_recycle, allow_trade, allow_gift, allow_inventory_stack, interaction_type, interaction_modes_count, vending_ids, height_adjustable, effect_id, is_rare, rarity_level FROM `item_base`");
            return dbClient.GetTable();
        }
    }
}